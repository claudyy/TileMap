using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FixedRoom_Entry : BaseFixedRoom {
}
[CreateAssetMenu(menuName = "TileMap/LevelGenerator/FixedRoom/Entry")]
public class FixedRoomData_Entry : BaseFixedRoomData {
    public override BaseLevelStructure GetStructure() {
        var r = new FixedRoom_Entry();
        r.Init(this);
        return r;
    }
}
