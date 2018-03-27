using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FixedRoom_Standart : BaseFixedRoom {

}
[CreateAssetMenu(menuName = "TileMap/LevelGenerator/FixedRoom/Standart")]
public class FixedRoomData_Standart : BaseFixedRoomData {
    public override BaseLevelStructure GetStructure() {
        var r = new FixedRoom_Standart();
        r.Init(this);
        return r;
    }

}
