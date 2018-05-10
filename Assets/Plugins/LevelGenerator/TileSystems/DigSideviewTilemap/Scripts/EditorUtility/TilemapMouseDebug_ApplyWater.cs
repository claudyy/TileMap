using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_ApplyWater : TilemapMouseDebug_ActionBase {
    public TileDamageType type;
    public int waterAmout;
    public override void ActionOnTile(BaseLevelTile tile) {
        base.ActionOnTile(tile);
        var digTilemap = target as Tilemap_Dig;
        var digTile = tile as LevelTile_Dig;
        if(waterAmout>0)
            digTile.AddWater(target, waterAmout);
        else
            digTile.RemoveWater(target, -waterAmout);
    }
}
