using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClaudeFehlen.Tileset.Pathfinding;
public class BaseFixedRoom : BaseRoom{
    /*
    public static BaseFixedRoom EmptyRoom(int x,int y) {
        var r = new BaseFixedRoom();
        r.fixedX = x;
        r.fixedY = y;
        r.roomData = null;
        return r;
    }
    public bool IsEmpty() {
        return roomData == null;
    }
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
    }
    public int fixedX;
    public int fixedY;


    public void SetFixedPos(int fixedX, int fixedY) {
        this.fixedX = fixedX;
        this.fixedY = fixedY;
        posX = fixedX * roomData.sizeX;
        posY = fixedY * roomData.sizeY;
    }

    public int CompareTo(BaseFixedRoom other) {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
    int heapIndex;
    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }
    public int gCost;
    public int hCost;
    public int fCost {
        get {
            return gCost + hCost;
        }
    }
    public BaseFixedRoom parent;
    */
}
public class BaseFixedRoomData : Structure_BaseRoomData {
    /*
    public override BaseLevelStructure GetStructure() {
        return new BaseFixedRoom();
    }
    */
}
