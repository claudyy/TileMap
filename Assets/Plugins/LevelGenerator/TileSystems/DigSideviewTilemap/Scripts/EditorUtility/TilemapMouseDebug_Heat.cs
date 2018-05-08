using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_Heat : TilemapMouseDebug_Base {

    public override string TileText(BaseLevelTile tile) {
        var digTile = tile as LevelTile_Dig;
        if (digTile == null)
            return base.TileText(tile);
        return "Heat: " + digTile.GetHeat().ToString();

    }
}
