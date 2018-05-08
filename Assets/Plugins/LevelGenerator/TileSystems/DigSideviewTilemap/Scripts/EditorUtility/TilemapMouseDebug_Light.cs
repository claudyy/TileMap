using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_Light : TilemapMouseDebug_Base {

    public override string TileText(BaseLevelTile tile) {
        var digTile = tile as LevelTile_Dig;
        if(digTile == null)
             return base.TileText(tile);
        return "Light: "+ digTile.GetLight().ToString()+" InSunLight: "+digTile.inSunLight.ToString();
    }
}
