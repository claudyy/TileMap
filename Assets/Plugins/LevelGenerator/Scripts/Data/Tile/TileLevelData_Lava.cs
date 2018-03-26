using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileBehavior_Lava : TileBehavior_Liquid {
    TileLevelData_Lava lavaData;
    public override void Init(LevelTilemap level, LevelTile tile) {
        base.Init(level, tile);
        lavaData = tile.data as TileLevelData_Lava;
    }

    protected override TileLevelData_Liquid GetData() {
        return lavaData;
    }
    protected override void FillOtherLiquid(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        if (otherTile.behavior is TileBehavior_Water) {
            level.OverrideTile(otherTile.pos.x, otherTile.pos.y, lavaData.touchWater);
            liquidAmount = -1;
            return;
        }
        var oil = otherTile.behavior as TileBehavior_Oil;
        if (oil != null) {
            level.Explosion(otherTile.pos.x, otherTile.pos.y, 4);
            liquidAmount = 0;
            return;
        }
        base.FillOtherLiquid(other, otherTile, level);
    }
    protected override void FillLiquidBelow(TileBehavior_Liquid other,LevelTile otherTile, LevelTilemap level) {
        if(otherTile.behavior is TileBehavior_Water) {
            level.OverrideTile(otherTile.pos.x, otherTile.pos.y, lavaData.touchWater);
            liquidAmount = -1;
            return;
        }
        var oil = otherTile.behavior as TileBehavior_Oil;
        if (oil != null) {
            level.Explosion(otherTile.pos.x, otherTile.pos.y, 4);
            liquidAmount = 0;
            return;
        }
        base.FillOtherLiquid(other, otherTile, level);
    }
    protected override bool CanConnectOtherLiquid(LevelTile tile) {
        if (tile.behavior is TileBehavior_Lava)
            return true;
        return tile.behavior is TileBehavior_Water;
    }
    public override bool CanFlowToPos(int x, int y, LevelTilemap level) {
        if (level.GetTile(x,y).behavior is TileBehavior_Water)
            return true;
        return base.CanFlowToPos(x, y, level);
    }
}
[CreateAssetMenu(menuName = "LevelGenerator/Tile/Lava")]
public class TileLevelData_Lava : TileLevelData_Liquid {

    public TileLevelData touchWater;
    public override BaseTileBehvior GetBehavior() {
        return new TileBehavior_Lava();
    }
}
