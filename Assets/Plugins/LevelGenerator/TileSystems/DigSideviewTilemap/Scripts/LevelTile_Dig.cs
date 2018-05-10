using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile_Dig : BaseLevelTile {
    public int MaxHealth {
        get {
            var digData = data as LevelTileData_Dig;
            if (digData == null)
                return 0;
            return digData.maxHealth;
        }
    }

    public int GetWaterAmount() {
        return liquidAmount;
    }

    public int GetHeat() {
        return heat + (inSunLight == true ?  10 : 0);
    }

    public int currentHealth;

    public float GetLight(bool ignoreSunLight=false) {
        var l = light + (ignoreSunLight? 0 : sunLight);
        return Mathf.Clamp01(l/10);
    }

    public int heat;
    public bool inSunLight;
    public float sunLight;
    float light;


    public bool isWalkable {
        get {
            if (data != null && (data is LevelTileData_Dig))
                return (data as LevelTileData_Dig).isWalkable;
            return true;
        }
    }
    public float HealthPercent {
        get {
            return (float)currentHealth / MaxHealth;
        }
    }
    public override bool HaveBehaviorViewUpdate() {
        if (HasWater())
            return true;
        return base.HaveBehaviorViewUpdate();
    }
    public override void Init(LevelTilemap level, LevelTileData data, Vector3Int pos) {
        base.Init(level, data, pos);
        currentHealth = MaxHealth;
        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;

        heat += digData.baseHeat;
        light += digData.baseLight;

        if (digData.emitHeat != 0) {
            EmitHeat(level, digData);
        }
        if (digData.emitLight != 0) {
            EmitLight(level, digData);
        }

    }
    public override void UpdateBehaviorInViewTile(LevelTilemap level) {
        base.UpdateBehaviorInViewTile(level);
        if (HasWater())
            UpdateWater(level);
    }
    public override void Remove(LevelTilemap level) {
        base.Remove(level);

        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;
        heat -= digData.baseHeat;
        light -= digData.baseLight;
    }
    public virtual void ApplyDamage(LevelTilemap level, TileDamageType type, int damage) {

        if (currentHealth == -1)
            return;
        currentHealth -= damage;
        if (currentHealth > 0)
            return;
        level.DestroyTile(this);
    }
    public override string DebugInfo() {
        if (MaxHealth == 0)
            return "";
        return "Health" + HealthPercent;
    }
    #region Water
    float curWaterDelay;
    public const float flowSpeed =.1f;
    int liquidAmount;
    bool erase;
    float delayedErase;
    public const float delayedEraseMax =1;
    public const int stickAmount = 10;
    public const int flowAmount = 5;
    public const int maxWater = 100;
    public bool IsFull() {
        return liquidAmount >= maxWater;
    }
    public bool HasWater() {
        return liquidAmount > 0;
    }
    bool DelayErase(LevelTilemap level, LevelTile_Dig tile) {
        if (liquidAmount > 0) {
            erase = false;
            delayedErase = delayedEraseMax;
        }
        if (erase == false)
            return false;
        delayedErase -= inViewdeltaTime;
        if (delayedErase >= 0)
            return false;
        return true;
    }
    void UpdateWater(LevelTilemap level) {

        if (DelayErase(level, this))
            return;
        curWaterDelay -= inViewdeltaTime;
        if (curWaterDelay >= 0)
            return;
        curWaterDelay = flowSpeed;
        if (liquidAmount > 0) {
            if (CanFlowToPos(x, y - 1, level) == true) {
                FlowDown(x, y - 1, level, this);
            } else {
                int dir = (int)Mathf.Sign(UnityEngine.Random.Range(-1, 1));
                Flow(dir, level, this);
            }
        }
        viewNeedUpdate = true;
    }
    void Flow(int dirX, LevelTilemap level, LevelTile_Dig tile) {
        int targetX = tile.pos.x + dirX;
        int targetY = tile.pos.y;
        if (CanFlowToPos(targetX, targetY, level) == false)
            return;
        if (liquidAmount <= stickAmount)
            return;

        FillOtherLiquid(tile, level.GetTile<LevelTile_Dig>(targetX, targetY), level);


        //if (liquidAmount <= 0)
        //    StartErase();
    }
    void FlowDown(int x, int y, LevelTilemap level, LevelTile_Dig tile) {
        if (CanFlowToPos(x, y, level) == false)
            return;
        if (liquidAmount <= 0)
            return;

        FillOtherLiquid(tile, level.GetTile<LevelTile_Dig>(x, y), level);

    }



    bool NeighbourHasLessWater(LevelTile_Dig neighbour) {
        return liquidAmount - neighbour.liquidAmount > flowAmount;
    }

    bool CanFlowToPos(int x, int y, LevelTilemap level) {
        if (level.IsPosOutOfBound(x, y))
            return false;

        var target = level.GetTile<LevelTile_Dig>(x, y);
        if (target.isWalkable) {
            if(target.IsFull() == false)
                return true;
        }

        return false;
    }
    protected void FillOtherLiquid(LevelTile_Dig self, LevelTile_Dig otherTile, LevelTilemap level) {
        int value = Mathf.Clamp(flowAmount, 0, liquidAmount);
        otherTile.AddWater(level, value);
        self.RemoveWater(level, value);
    }
    public void AddWater(LevelTilemap level, int amount) {
        if (HasWater()) {
            liquidAmount += amount;
        } else {
            liquidAmount = amount;
            level.GetITile(x, y - 1).viewNeedUpdate = true;
        }
        viewNeedUpdate = true;
        level.ChangedTile(this);
    }
    public void RemoveWater(LevelTilemap level, int amount) {
        if (HasWater()) {
            liquidAmount = Mathf.Max(liquidAmount - amount, 0);
            if (liquidAmount <= 0) {
                erase = true;
                delayedErase = .4f;
                level.GetITile(x, y - 1).viewNeedUpdate = true;
            }
            level.ChangedTile(this);
        }
        viewNeedUpdate = true;
    }
    #endregion
    #region Heat
    public void EmitHeat(LevelTilemap level, LevelTileData_Dig digData) {
        var startX = x;
        var startY = y;
        for (int i = 1; i < 1 + digData.emitHeatRange; i++) {
            for (int x = -i; x <= i; x++) {
                for (int y = -i; y <= i; y++) {

                    if (Mathf.Abs(x) < i && Mathf.Abs(y) < i)
                        continue;
                    if (level.IsPosOutOfBound(startX + x, startY + y))
                        continue;
                    level.GetTile<LevelTile_Dig>(startX + x, startY + y).AddHeat(level,this);
                }
            }
        }
    }
    public void AddHeat(LevelTilemap level, BaseLevelTile heatSource) {
        heatSource.onRemove += RemoveHeat;
        AddHeatPath(level,heatSource);
        viewNeedUpdate = true;
    }
    void AddHeatPath(LevelTilemap level, BaseLevelTile heatSource) {
        heat += CalculateHeat(level, heatSource, true);
    }
    void RecaclucateHeatPathPre(LevelTilemap level, BaseLevelTile dontNeeded, BaseLevelTile heatSource) {
        heat -= CalculateHeat(level, heatSource);
    }
    void RecaclucateHeatPathPost(LevelTilemap level, BaseLevelTile dontNeeded, BaseLevelTile heatSource) {
        heat += CalculateHeat(level, heatSource);
    }
    void RemoveHeatPath(LevelTilemap level, BaseLevelTile heatSource) {
        heat -= CalculateHeat(level, heatSource ,true , true);
    }
    Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>> heatPreActions = new Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>>();
    Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>> heatPostActions = new Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>>();

    int CalculateHeat(LevelTilemap level, BaseLevelTile heatSource, bool addActions = false, bool removeAction = false) {
        var sourceData = heatSource.data as LevelTileData_Dig;
        if (sourceData == null)
            return 0;
        var heat = sourceData.emitHeat;
        var dir = (new Vector2(x, y) - new Vector2(heatSource.x, heatSource.y));
        var dist = dir.magnitude;
        dir.Normalize();
        var start = heatSource.pos;
        BaseLevelTile pathTile= null;

        if (addActions) {
            if (removeAction == false) {
                heatPreActions.Add(heatSource, (LevelTilemap, BaseLevelTile) => RecaclucateHeatPathPre(level, null, heatSource));
                heatPostActions.Add(heatSource, (LevelTilemap, BaseLevelTile) => RecaclucateHeatPathPost(level, null, heatSource));
            }
        }


        for (int i = 0; i < (int)dist; i++) {
            pathTile = level.GetITile((int)(start.x +.5f + dir.x*i), (int)(start.y + .5f + dir.y * i));
            if (pathTile == this || pathTile == heatSource)
                continue;
            var pathData = pathTile.data as LevelTileData_Dig;
            if (pathData == null)
                continue;
            if (addActions) {
                if (addActions) {
                    if (removeAction) {
                        pathTile.onRemove -= heatPreActions[heatSource];
                        pathTile.onInit -= heatPostActions[heatSource];
                    } else {
                        pathTile.onRemove += heatPreActions[heatSource];
                        pathTile.onInit += heatPostActions[heatSource];
                    }

                }

            }

            heat =(int)(heat *1 - pathData.heatAbsorption);
        }

        if (addActions) {
            if (removeAction) {
                heatPreActions.Remove(heatSource);
                heatPostActions.Remove(heatSource);
            }
        }


        return heat;

    }
    void RemoveHeat(LevelTilemap level,BaseLevelTile heatSource) {
        RemoveHeatPath(level, heatSource);
        heatSource.onRemove -= RemoveHeat;
        viewNeedUpdate = true;

    }
    #endregion
    #region Light
    Dictionary<int, LevelTile_Dig> lightSources = new Dictionary<int, LevelTile_Dig>();
    public void EmitLight(LevelTilemap level, LevelTileData_Dig digData) {
        var startX = x;
        var startY = y;
        for (int i = 1; i < 1 + digData.emitLightRange; i++) {
            for (int x = -i; x <= i; x++) {
                for (int y = -i; y <= i; y++) {

                    if (Mathf.Abs(x) < i && Mathf.Abs(y) < i)
                        continue;
                    if (level.IsPosOutOfBound(startX + x, startY + y))
                        continue;
                    level.GetTile<LevelTile_Dig>(startX + x, startY + y).AddLight(level, this);
                }
            }
        }
    }
    public void AddLight(LevelTilemap level, BaseLevelTile lightSource) {
        //lightSources.Add(lightSource.x + lightSource.y * level.sizeX, lightSource as LevelTile_Dig);
        light += CalcuateLight(lightSource as LevelTile_Dig);
        viewNeedUpdate = true;
        lightSource.onRemove += RemoveLight;
    }
    public void RemoveLight(LevelTilemap level, BaseLevelTile lightSource) {
        //lightSources.Remove(lightSource.x + lightSource.y * level.sizeX);
        light -= CalcuateLight(lightSource as LevelTile_Dig);
        viewNeedUpdate = true;
        lightSource.onRemove -= RemoveLight;
    }
    float CalcuateLight(LevelTile_Dig lightSource) {
        var x = Mathf.Abs(this.x - lightSource.pos.x);
        var y = Mathf.Abs(this.y - lightSource.pos.y);
        var dis = Mathf.Sqrt(x * x + y * y);
        var c = Mathf.Clamp01((1 + dis) / (lightSource.data as LevelTileData_Dig).emitLightRange);
        return (1 - c) * (lightSource.data as LevelTileData_Dig).emitLight;
    }

   
    #endregion
}
