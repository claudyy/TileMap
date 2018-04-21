using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseRoom : BaseLevelStructure {
    /*
    public Structure_BaseRoomData roomData;
    public int sizeX { get { return roomData.sizeX; } }
    public int sizeY { get { return roomData.sizeY; } }


    public List<bool> TopDoorPos;
    public int topDoorCount = 0;
    public List<bool> BottomDoorPos;
    public int bottomDoorCount = 0;

    public List<bool> LeftDoorPos;
    public int leftDoorCount = 0;
    public List<bool> RightDoorPos;
    public int rightDoorCount = 0;
    public int door;
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
        roomData = data as Structure_BaseRoomData;
        TopDoorPos= new List<bool>(new bool[roomData.sizeX]);
        BottomDoorPos = new List<bool>(new bool[roomData.sizeX]);
        LeftDoorPos = new List<bool>(new bool[roomData.sizeY]);
        RightDoorPos = new List<bool>(new bool[roomData.sizeY]);
    }
    public override IEnumerator Generate(GeneratorMapData map) {
        base.Generate(map);
        int startX = posX;
        int startY = posY;
        for (int x = 0; x < roomData.sizeX; x++) {
            for (int y = 0; y < roomData.sizeY; y++) {
                if(y == 0) {
                    if(BottomDoorPos[x] == true)
                        map.OverrideTile(startX + x, startY + y, roomData.floor);
                    else
                        map.OverrideTile(startX + x, startY + y, roomData.wall);

                    continue;
                }
                if (x == 0) {
                    if (LeftDoorPos[y] == true)
                        map.OverrideTile(startX + x, startY + y, roomData.floor);
                    else
                        map.OverrideTile(startX + x, startY + y, roomData.wall);

                    continue;
                }
                if (x == roomData.sizeX-1) {
                    if (RightDoorPos[y] == true)
                        map.OverrideTile(startX + x, startY + y, roomData.floor);
                    else
                        map.OverrideTile(startX + x, startY + y, roomData.wall);

                    continue;
                }
                if (y == roomData.sizeY - 1) {
                    if (TopDoorPos[x] == true)
                        map.OverrideTile(startX + x, startY + y, roomData.floor);
                    else
                        map.OverrideTile(startX + x, startY + y, roomData.wall);

                    continue;
                }
                map.OverrideTile(startX + x, startY + y, roomData.roomTileList[x * y]);
                yield return null;
            }
        }
    }
    public void SetDoorTop(int x) {
        TopDoorPos[x-1] = true;
        TopDoorPos[x] = true;
        TopDoorPos[x+1] = true;
        topDoorCount++;
    }
    public void SetDoorBottom(int x) {
        BottomDoorPos[x - 1] = true;
        BottomDoorPos[x] = true;
        BottomDoorPos[x + 1] = true;
        bottomDoorCount++;
    }
    public void SetDoorLeft(int y) {
        LeftDoorPos[y - 1] = true;
        LeftDoorPos[y] = true;
        LeftDoorPos[y + 1] = true;
        leftDoorCount++;
    }
    public void SetDoorRight(int y) {
        RightDoorPos[y - 1] = true;
        RightDoorPos[y] = true;
        RightDoorPos[y + 1] = true;
        rightDoorCount++;
    }
    public override List<Bounds> GetBounds() {
        var b = new Bounds();
        b.SetMinMax(new Vector3(posX, posY), new Vector3(posX + sizeX, posY+sizeY));
        return new List<Bounds>(1) {b};
    }
    */
}

[CreateAssetMenu(menuName = "TileMap/LevelGenerator/BaseRoom")]
public class Structure_BaseRoomData : BaseLevelStructureData {
    /*
    public TileLevelData[] roomTileList;
    public int sizeX;
    public int sizeY;
    public const int doorSize = 3;
    public TileLevelData wall;
    public TileLevelData floor;
    public TileLevelData fillData;

    public void UpdateSize() {
        roomTileList = new TileLevelData[FixedRoomGenerator.fixedRoomSizeX * FixedRoomGenerator.fixedRoomSizeY];
        sizeX = FixedRoomGenerator.fixedRoomSizeX;
        sizeY = FixedRoomGenerator.fixedRoomSizeY;
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                roomTileList[x + y * sizeX] = fillData;
            }
        }
        topPossibleDoors = new List<bool>();
        for (int i = 0; i < sizeX; i++) {
            if (PossibleDoorTop(i))
                topPossibleDoors.Add(true);
            else
                topPossibleDoors.Add(false);
        }
        bottomPossibleDoors = new List<bool>();
        for (int i = 0; i < sizeX; i++) {
            if (PossibleDoorBottom(i))
                bottomPossibleDoors.Add(true);
            else
                bottomPossibleDoors.Add(false);
        }
    }
    public override BaseLevelStructure GetStructure() {
        var r = new BaseRoom();
        r.Init(this);
        return r;
    }
    public void Overwrite(TileLevelData[] tiles) {
        if(tiles.Length == sizeX * sizeY) {
            Debug.LogWarning("Wrong size");
            return;
        }
        roomTileList = new TileLevelData[sizeX * sizeY];
        for (int i = 0; i < tiles.Length; i++) {
            roomTileList[i] = tiles[i];
        }
    }
    [HideInInspector]
    public List<bool> topPossibleDoors;
    public List<bool> GetTopPossibleDoors(TileLevelData floor) {
        return topPossibleDoors;
    }
    public List<Vector2Int> GetTopPossibleDoorsPos() {
        var list = new List<Vector2Int>();
        for (int i = 0; i < sizeX; i++) {
            if (PossibleDoorTop(i) == false)
                list.Add(new Vector2Int(i,sizeY-1));
        }
        return list;
    }
    public bool PossibleDoorTop(int x) {
        if (x - 2 < 0)
            return false;
        if (x + 2 >= sizeX)
            return false;
        if(roomTileList[TopStart() + x-1] == floor && roomTileList[TopStart() + x] == floor && roomTileList[TopStart() + x +1] == floor)
            return true;
        return false;
    }
    int TopStart() {
        return (sizeY - 1) * sizeX;
    }
    [HideInInspector]
    public List<bool> bottomPossibleDoors;
    public List<bool> GetBottomPossibleDoors(TileLevelData floor) {
       
        return bottomPossibleDoors;
    }
    public List<Vector2Int> GetBottomPossibleDoorsPos() {
        var list = new List<Vector2Int>();
        for (int i = 0; i < sizeX; i++) {
            if (PossibleDoorBottom(i) == false)
                list.Add(new Vector2Int(i, 0));
        }
        return list;
    }
    public bool PossibleDoorBottom(int x) {
        if (x - 2 < 0)
            return false;
        if (x + 2 >= sizeX)
            return false;
        if (roomTileList[x - 1] == floor && roomTileList[x] == floor && roomTileList[x + 1] == floor)
            return true;
        return false;
    }
    [HideInInspector]
    public List<bool> leftPossibleDoors;
    public List<bool> GetLeftPossibleDoors(TileLevelData floor) {
        var leftPossibleDoors = new List<bool>();
        for (int y = 0; y < sizeY; y++) {
            if (PossibleDoorLeft(y))
                leftPossibleDoors.Add(true);
            else
                leftPossibleDoors.Add(false);
        }
        return leftPossibleDoors;
    }
    public bool PossibleDoorLeft(int y) {
        if (y - 2 < 0)
            return false;
        if (y + 2 >= sizeY)
            return false;
        if (roomTileList[PosToIndex(0, y) - 1] == floor && roomTileList[PosToIndex(0, y)] == floor && roomTileList[PosToIndex(0, y) + 1] == floor)
            return true;
        return false;
    }
    [HideInInspector]
    public List<bool> rightPossibleDoors;
    public List<bool> GetRightPossibleDoors(TileLevelData floor) {
        var rightPossibleDoors = new List<bool>();
        for (int y = 0; y < sizeY; y++) {
            if (PossibleDoorRight(y))
                rightPossibleDoors.Add(true);
            else
                rightPossibleDoors.Add(false);
        }
        return rightPossibleDoors;
    }
    public bool PossibleDoorRight(int y) {
        if (y - 2 < 0)
            return false;
        if (y + 2 >= sizeY)
            return false;
        if (roomTileList[PosToIndex(sizeX - 1, y) - 1] == floor && roomTileList[PosToIndex(sizeX - 1, y) + y] == floor && roomTileList[PosToIndex(sizeX - 1, y) + y + 1] == floor)
            return true;
        return false;
    }
    public int RightStart() {
        return (sizeX - 1) * sizeY;
    }
    public int PosToIndex(int x,int y) {
        return x + y * sizeX;
    }
    */
}
