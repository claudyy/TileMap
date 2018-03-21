using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileBehavior_Water : TileBehavior_Liquid {
    TileLevelData_Water waterData;
    public float polluted;
    public float maxPolluted = 10;
    public override void Init(LevelTilemap level, LevelTile tile) {
        base.Init(level, tile);
        waterData = tile.data as TileLevelData_Water;
    }
    protected override TileLevelData_Liquid GetData() {
        return waterData;
    }
    protected override void FillOtherLiquid(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        var water = otherTile.behavior as TileBehavior_Water;
        if (water != null) {
            float p = Mathf.Clamp(1, 0, water.maxPolluted - water.polluted);
            water.polluted += p;
            polluted -= p/2;
        }
        base.FillOtherLiquid(other, otherTile, level);
    }
    protected override void FillLiquidBelow(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        var water = otherTile.behavior as TileBehavior_Water;
        if (water != null) {
            float p = Mathf.Clamp(1, 0, water.maxPolluted - water.polluted);
            water.polluted += p;
            polluted -= p/2;
        }
        base.FillLiquidBelow(other, otherTile, level);
    }
    protected override bool CanConnectOtherLiquid(LevelTile tile) {
        return tile.behavior is TileBehavior_Water;
    }
    public override Color GetColor(LevelTile tile, LevelTilemap level) {
        var c = base.GetColor(tile, level);
        c.a = polluted / maxPolluted;
        return c;
    }
}
[CreateAssetMenu(menuName = "LevelGenerator/Tile/Water")]
public class TileLevelData_Water : TileLevelData_Liquid {
    public override BaseTileBehvior GetBehavior() {
        return new TileBehavior_Water();
    }
}
