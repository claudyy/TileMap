using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ClaudeFehlen.Tileset.Pathfinding {

    public class Grid : MonoBehaviour {
        
        public LevelTilemap tileMap;
        public bool displayGridGizmos;
        public Vector2 gridWorldSize;
        public TerrainType[] walkableRegions;
        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
        LayerMask walkableMask;

        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        int penaltyMin = int.MaxValue;
        int penaltyMax = int.MinValue;
        Vector3 worldBottomLeft;
        void Awake() {
            


            foreach (TerrainType region in walkableRegions) {
                walkableMask.value |= region.terrainMask.value;
                walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
            }
            tileMap.onLoaded += CreateGrid;
            //CreateGrid();
        }

        public int MaxSize {
            get {
                return gridSizeX * gridSizeY;
            }
        }

        void CreateGrid(LevelTilemap tileMap) {
            nodeDiameter = 1;
            int minX = 0;
            int minY = 0;
            int maxX = tileMap.sizeX;
            int maxY = tileMap.sizeY;
            gridSizeX = Mathf.Abs(minX) + Mathf.Abs(maxX);
            gridSizeY = Mathf.Abs(minY) + Mathf.Abs(maxY);
            gridWorldSize.x = gridSizeX * nodeDiameter;
            gridWorldSize.y = gridSizeY * nodeDiameter;
            grid = new Node[gridSizeX, gridSizeY];
            worldBottomLeft = transform.position + new Vector3(minX,minY);
            int indexX = 0;
            int indexY = 0;
            for (int x = minX; x < maxX; x++) {
                for (int y = minY; y < maxY; y++) {
                    var data = tileMap.GetITile(x, y).data;
                    grid[indexX, indexY] = new Node(data, worldBottomLeft + new Vector3(indexX + .5f, indexY + .5f,0), indexX, indexY, 0);
                    
                    indexY++;
                }
                indexX++;
                indexY = 0;
            }
            //BlurPenaltyMap(3);

        }

        void BlurPenaltyMap(int blurSize) {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++) {
                for (int x = -kernelExtents; x <= kernelExtents; x++) {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++) {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++) {
                for (int y = -kernelExtents; y <= kernelExtents; y++) {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++) {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty = blurredPenalty;

                    if (blurredPenalty > penaltyMax) {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin) {
                        penaltyMin = blurredPenalty;
                    }
                }
            }

        }

        public List<Node> GetNeighbours(Node node) {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    //dont use corners
                    if ((x != 0 && y != 0))
                        continue;
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }


        public Node NodeFromWorldPoint(Vector3 worldPosition) {
            float percentX = (transform.InverseTransformVector(worldPosition).x) / gridWorldSize.x;
            float percentY = (transform.InverseTransformVector(worldPosition).y) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            //Debug.Log("worldPosition " + (transform.InverseTransformVector(worldPosition).x) + " / " + (transform.InverseTransformVector(worldPosition).y));

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            //Debug.Log(x + " / " + y);
            return grid[x, y];
        }
        public Node FindNearesPoint(Vector3 point) {
            var node = NodeFromWorldPoint(point);
            if (node.walkable)
                return node;
            int x = node.gridX;
            int y = node.gridY;
            for (int i = 1; i < 20; i++) {
                if (grid[x + i, y].walkable)
                    return grid[x + i, y];
                if (grid[x + i, y + i].walkable)
                    return grid[x + i, y + i];
                if (grid[x + i, y - i].walkable)
                    return grid[x + i, y - i];
                if (grid[x - i, y].walkable)
                    return grid[x - i, y];
                if (grid[x - i, y + i].walkable)
                    return grid[x - i, y + i];
                if (grid[x - i, y - i].walkable)
                    return grid[x - i, y - i];
                if (grid[x, y + i].walkable)
                    return grid[x, y + i];
                if (grid[x, y - i].walkable)
                    return grid[x, y - i];
            }
            return node;
        }
        void OnDrawGizmos() {
            Gizmos.DrawWireCube(worldBottomLeft + new Vector3(gridSizeX/2, gridSizeY/2), new Vector3(gridSizeX, gridSizeY));
            if (grid != null && displayGridGizmos) {
                foreach (Node n in grid) {

                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                    Gizmos.DrawCube(n.worldPosition + new Vector3(.5f,.5f), Vector3.one * (nodeDiameter));
                    //UnityEditor.Handles.Label(n.worldPosition + new Vector3(.5f, .5f), n.gridX + " : " + n.gridY);
                }
            }
        }

        [System.Serializable]
        public class TerrainType {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }


    }

}