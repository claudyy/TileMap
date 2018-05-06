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
    public int currentHealth;
    public int heat;
    public bool inSunLight;
    public float sunLight;
    float light;
    public override void Init(LevelTilemap level, LevelTileData data, Vector3Int pos) {
        base.Init(level, data, pos);
        currentHealth = MaxHealth;
    }
}
