using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBehavior_Liquid : BaseTileBehvior {
    public TileLevelData_Liquid data;
    public int liquidAmount;
    float curDelay;
    bool erase;
    float delayedErase;
    float delayedEraseMax = 1;
    public override void Init(LevelTilemap level, LevelTile tile) {
        base.Init(level, tile);
        data = tile.data as TileLevelData_Liquid;
        liquidAmount = data.maxWater;
        curDelay = data.flowSpeed * 2;
        delayedErase = delayedEraseMax;
    }
    public bool IsFull() {
        return liquidAmount >= data.maxWater;
    }
    public override void Update(LevelTilemap level, LevelTile tile) {
        base.Update(level, tile);
        curDelay -= deltaTime;

        if (DelayErase(level, tile))
            return;

        if (curDelay >= 0)
            return;
        curDelay = data.flowSpeed;
        if(liquidAmount > 0) {
            if (CanFlowToPos(tile.pos.x, tile.pos.y - 1, level) == true) {
                FlowDown(tile.pos.x, tile.pos.y - 1, level, tile);
            } else {
                int dir = (int)Mathf.Sign(Random.Range(-1, 1));
                Flow(dir, level, tile);
            }
        }
    }
    bool DelayErase(LevelTilemap level, LevelTile tile) {
        if (liquidAmount > 0) {
            erase = false;
            delayedErase = delayedEraseMax;
        }
        if (erase == false)
            return false;
        delayedErase -= deltaTime;
        if (delayedErase >= 0)
            return false;
        
        level.Erase(tile.pos.x, tile.pos.y);
        return true;
    }
    void Flow(int dirX, LevelTilemap level, LevelTile tile) {
        int targetX = tile.pos.x + dirX;
        int targetY = tile.pos.y;
        if (CanFlowToPos(targetX, targetY,level) == false)
            return;
        if (liquidAmount <= data.stickAmount)
            return;
        var otherWater = level.GetTile(targetX, targetY).behavior as TileBehavior_Liquid;

        if (otherWater == null) {
            if(liquidAmount > 0) {
                level.OverrideTile(targetX, targetY, GetData());
                otherWater = level.GetTile(targetX, targetY).behavior as TileBehavior_Liquid;
                otherWater.liquidAmount = 0;
                FillOtherLiquid(otherWater, level.GetTile(targetX, targetY), level);
            }
        } else {
            if (NeighbourHasLessWater(otherWater)) {
                FillOtherLiquid(otherWater, level.GetTile(targetX, targetY), level);
            } 

        }

        if (liquidAmount <= 0)
            StartErase();
    }
    void FlowDown(int x, int y, LevelTilemap level, LevelTile tile) {
        if (CanFlowToPos(x, y, level) == false)
            return;
        if (liquidAmount <= 0)
            return;
        var otherWater = level.GetTile(x, y).behavior as TileBehavior_Liquid;


        if (otherWater == null) {
            level.OverrideTile(x, y, GetData());
            otherWater = level.GetTile(x, y).behavior as TileBehavior_Liquid;
            otherWater.liquidAmount = 0;
            FillOtherLiquid(otherWater, level.GetTile(x, y), level);
        } else {
            FillLiquidBelow(otherWater, level.GetTile(x, y), level);
        }

        if (liquidAmount <= 0)
            StartErase();
    }
    protected virtual bool CanConnectOtherLiquid(LevelTile level) {
        return true;
    }
    protected virtual TileLevelData_Liquid GetData() {
        return data;
    }

    bool NeighbourHasLessWater(TileBehavior_Liquid neighbour) {
        return liquidAmount - neighbour.liquidAmount > data.flowAmount;
    }
    //nx ny is neighbours position
    bool NeighbourNeighboursHasLessWater(int dirX,int nx,int ny,LevelTilemap level) {
        TileBehavior_Liquid otherWater = null;

        for (int i = 0; i < 4; i++) {
            otherWater = level.GetTile(nx + dirX, ny).behavior as TileBehavior_Liquid;
            if(otherWater != null) {
                if (NeighbourHasLessWater(otherWater))
                    return true;
            }
        }
        return false;
    }
    
    void StartErase() {
        erase = true;
        delayedErase = delayedEraseMax;
    }
    public virtual bool CanFlowToPos(int x, int y, LevelTilemap level) {
        if (level.IsEmpty(x, y))
            return true;
        var liquid = GetLiquidTile(x, y, level);
        if (liquid != null && CanConnectOtherLiquid(level.GetTile(x,y)) && liquid.IsFull() == false)
            return true;
        return false;
    }
    bool isLiquidTile(int x, int y, LevelTilemap level) {
        return level.GetTile(x, y).data is TileLevelData_Liquid;
    }
    TileBehavior_Liquid GetLiquidTile(int x, int y, LevelTilemap level) {
        return level.GetTile(x, y).behavior as TileBehavior_Liquid;
    }
    protected virtual void FillOtherLiquid(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        int value = Mathf.Clamp(data.flowAmount, 0, liquidAmount);
        liquidAmount -= value;
        other.liquidAmount += value;
    }
    protected virtual void FillLiquidBelow(TileBehavior_Liquid other, LevelTile otherTile, LevelTilemap level) {
        if (other.liquidAmount < data.maxWater) {
            int value = Mathf.Clamp(data.flowAmount, 0, liquidAmount);
            liquidAmount -= value;
            other.liquidAmount += value;
        }
    }
    public float PercentageFilled() {
        return ((float)liquidAmount / data.maxWater);
    }
    public float PercentageFilled(TileBehavior_Liquid water) {
        return ((float)water.liquidAmount / water.data.maxWater);
    }
    public override Tile GetTile(LevelTile tile,LevelTilemap level) {
        return data.tile;
    }
    public override bool OverrideColor(LevelTile tile, LevelTilemap level) {
        return true;
    }
    public float PercentageFilledDisplay(LevelTile tile, LevelTilemap level) {
        if(level.IsEmpty(tile.pos.x, tile.pos.y - 1)== false)
            return Mathf.Lerp(delayedErase / delayedEraseMax, PercentageFilled(), 0.8f);
        return Mathf.Lerp(delayedErase / delayedEraseMax , PercentageFilled(),0.2f);
    }
    public override Color GetColor(LevelTile tile, LevelTilemap level) {
        var startValue = PercentageFilledDisplay(tile, level);
        Color c = new Color(startValue, startValue, startValue, 0);

        //left
        var water = GetLiquidTile(tile.pos.x - 1, tile.pos.y, level);
        if (water != null) {
            c.r = (water.PercentageFilledDisplay(tile, level) + startValue)/2;
        } else {
            if (level.IsEmpty(tile.pos.x - 1, tile.pos.y) == false && level.IsEmpty(tile.pos.x + 1, tile.pos.y) == true)
                c.r = 1;
        }
        water = null;

        //right
        water = GetLiquidTile(tile.pos.x + 1, tile.pos.y, level);
        if (water != null) {
            c.g = (PercentageFilledDisplay(tile, level) + startValue) / 2;
        }
        else {
            if (level.IsEmpty(tile.pos.x + 1, tile.pos.y) == false && level.IsEmpty(tile.pos.x - 1, tile.pos.y) == true)
                c.g = 1;
        }
        water = null;

        water = GetLiquidTile(tile.pos.x, tile.pos.y + 1, level);
        if (water != null)
            c.b = (water.PercentageFilledDisplay(tile, level));
        water = null;


        //c.a = startValue;
        c.r = Mathf.Clamp(c.r, 0, 0.97f);
        c.g = Mathf.Clamp(c.g, 0, 0.97f);
        c.b = Mathf.Clamp(c.b, 0, 0.97f);
        c.a = Mathf.Clamp(c.a, 0, 0.97f);
        return c;
    }
}
public class TileLevelData_Liquid : TileLevelData {
    public int maxWater = 10;
    public float flowSpeed;
    public int flowAmount;
    public int stickAmount;
    public override BaseTileBehvior GetBehavior() {
        return new TileBehavior_Liquid();
    }
}
