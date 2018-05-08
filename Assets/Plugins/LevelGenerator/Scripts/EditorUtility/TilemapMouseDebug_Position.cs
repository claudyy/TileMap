using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_Position : TilemapMouseDebug_Base {

    public override string TileText(BaseLevelTile tile) {
        return tile.x + " / " + tile.y;
    }
}
