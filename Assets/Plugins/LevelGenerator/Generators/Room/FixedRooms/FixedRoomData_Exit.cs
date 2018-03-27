﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FixedRoom_Exit : BaseFixedRoom {

}
[CreateAssetMenu(menuName = "TileMap/LevelGenerator/FixedRoom/Exit")]
public class FixedRoomData_Exit : BaseFixedRoomData {
    public override BaseLevelStructure GetStructure() {
        var r = new FixedRoom_Exit();
        r.Init(this);
        return r;
    }
}
