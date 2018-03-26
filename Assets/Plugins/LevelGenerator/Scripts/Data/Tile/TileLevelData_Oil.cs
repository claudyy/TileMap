using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileBehavior_Oil : TileBehavior_Liquid {

    protected override void FillLiquidBelow(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        var water = otherTile.behavior as TileBehavior_Water;
        if (water != null) {
            if(water.polluted < water.maxPolluted) {
                water.polluted += 1;
                liquidAmount -= 1;
            }
            return;
        }
        var lava = otherTile.behavior as TileBehavior_Lava;
        if (lava != null) {
            level.Explosion(otherTile.pos.x, otherTile.pos.y, 4);
            liquidAmount = 0;
            return;
        }
        base.FillLiquidBelow(other, otherTile, level);
    }
    protected override void FillOtherLiquid(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        var water = otherTile.behavior as TileBehavior_Water;
        if (water != null) {
            if (water.polluted < water.maxPolluted) {
                water.polluted += 1;
                liquidAmount -= 1;
            }
            return;
        }
        var lava = otherTile.behavior as TileBehavior_Lava;
        if (lava != null) {
            level.Explosion(otherTile.pos.x, otherTile.pos.y, 4);
            liquidAmount = 0;
            return;
        }
        base.FillOtherLiquid(other, otherTile, level);
    }
}
[CreateAssetMenu(menuName = "LevelGenerator/Tile/Oil")]
public class TileLevelData_Oil : TileLevelData_Liquid {
    public override BaseTileBehvior GetBehavior() {
        return new TileBehavior_Oil();
    }
}
