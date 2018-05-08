using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_Action_OverrideTile : TilemapMouseDebug_ActionBase {
    public LevelTileData data;
    public override void ActionOnTile(BaseLevelTile tile) {
        base.ActionOnTile(tile);
        target.OverrideTile(tile.pos, data);
    }
}
