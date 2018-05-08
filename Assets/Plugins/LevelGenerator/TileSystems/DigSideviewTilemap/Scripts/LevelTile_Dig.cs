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

    internal int GetWaterAmount() {
        return 0;
    }

    internal int GetHeat() {
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
    public float HealthPercent {
        get {
            return (float)currentHealth / MaxHealth;
        }
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
        lightSource.onRemove += RemoveLight;
        AddLightPath(level, lightSource);
        viewNeedUpdate = true;

    }
    void AddLightPath(LevelTilemap level, BaseLevelTile lightSource) {
        light += CalculateLight(level, lightSource, true);
    }
    void RecaclucateLightPathPre(LevelTilemap level, BaseLevelTile dontNeeded, BaseLevelTile lightSource) {
        light -= CalculateLight(level, lightSource);
    }
    void RecaclucateLightPathPost(LevelTilemap level, BaseLevelTile dontNeeded, BaseLevelTile lightSource) {
        light += CalculateLight(level, lightSource);
    }
    void RemoveLightPath(LevelTilemap level, BaseLevelTile lightSource) {
        light -= CalculateLight(level, lightSource, true, true);
    }
    Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>> lightPreActions = new Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>>();
    Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>> lightPostActions = new Dictionary<BaseLevelTile, Action<LevelTilemap, BaseLevelTile>>();
    int CalculateLight(LevelTilemap level, BaseLevelTile lightSource, bool addActions = false, bool removeAction = false) {
        var sourceData = lightSource.data as LevelTileData_Dig;
        if (sourceData == null)
            return 0;
        var light = sourceData.emitLight;
        var dir = (new Vector2(x, y) - new Vector2(lightSource.x, lightSource.y));
        var dist = dir.magnitude;
        dir.Normalize();
        var start = lightSource.pos;
        BaseLevelTile pathTile = null;
        if (addActions) {
            if (removeAction == false) {
                lightPreActions.Add(lightSource, (LevelTilemap, BaseLevelTile) => RecaclucateLightPathPre(level, null, lightSource));
                lightPostActions.Add(lightSource, (LevelTilemap, BaseLevelTile) => RecaclucateLightPathPost(level, null, lightSource));
            } 
        } 

        for (int i = 0; i < (int)dist; i++) {
            pathTile = level.GetITile((int)(start.x + .5f + dir.x * i), (int)(start.y + .5f + dir.y * i));
            if (pathTile == this || pathTile == lightSource)
                continue;
            var pathData = pathTile.data as LevelTileData_Dig;
            if (pathData == null)
                continue;
            if (addActions) {
                if (removeAction) {
                    pathTile.onRemove -= lightPreActions[lightSource];
                    pathTile.onInit -= lightPostActions[lightSource];
                } else {
                    pathTile.onRemove += lightPreActions[lightSource];
                    pathTile.onInit += lightPostActions[lightSource];
                }

            }

            light = (int)(light * 1 - pathData.heatAbsorption);
        }
        if (addActions) {
            if (removeAction) { 
                lightPreActions.Remove(lightSource);
                lightPostActions.Remove(lightSource);
            }
        }
        return light;

    }
    void RemoveLight(LevelTilemap level, BaseLevelTile lightSource) {
        RemoveLightPath(level, lightSource);
        lightSource.onRemove -= RemoveLight;
        viewNeedUpdate = true;

    }
    #endregion
}
