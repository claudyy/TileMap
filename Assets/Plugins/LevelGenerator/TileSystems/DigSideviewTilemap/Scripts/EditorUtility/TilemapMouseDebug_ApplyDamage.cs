using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_ApplyDamage : TilemapMouseDebug_ActionBase {
    public TileDamageType type;
    public int damage;
    public override void ActionOnTile(BaseLevelTile tile) {
        base.ActionOnTile(tile);
        var digTilemap = target as Tilemap_Dig;
        digTilemap.ApplyDamage(tile.pos, Vector3.up, type, damage);
    }
}
