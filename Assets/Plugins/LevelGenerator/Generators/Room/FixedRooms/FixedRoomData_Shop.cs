﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FixedRoom_Shop : BaseFixedRoom {

}
[CreateAssetMenu(menuName = "TileMap/LevelGenerator/Shop")]
public class FixedRoomData_Shop : BaseFixedRoomData {
    public override BaseLevelStructure GetStructure() {
        var r = new FixedRoom_Shop();
        r.Init(this);
        return r;
    }
}
