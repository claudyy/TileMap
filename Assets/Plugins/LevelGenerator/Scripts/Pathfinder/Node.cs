using UnityEngine;
using System.Collections;
namespace ClaudeFehlen.Tileset.Pathfinding {

    public class Node : IHeapItem<Node> {
        public LevelTileData tileData;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public int movementPenalty;

        public int gCost;
        public int hCost;
        public Node parent;
        int heapIndex;
        public bool walkable {
            get {
                if (tileData == null)
                    return true;
                return tileData.isWalkable;
            }
        }
        public Node(LevelTileData _tileData, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
            tileData = _tileData;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            movementPenalty = _penalty;
        }

        public int fCost {
            get {
                return gCost + hCost;
            }
        }

        public int HeapIndex {
            get {
                return heapIndex;
            }
            set {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare) {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }
}