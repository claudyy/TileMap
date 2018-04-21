using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITile {
    TileLevelData data { get; set; }
    int tileID { get; set; }
    BaseTileBehvior behavior { get; set; }
    Vector2Int pos { get; set; }
    bool needUpdate { get; set; }

    void Init(LevelTilemap level);
    void UpdateBehaviorTile(LevelTilemap level);
    void UpdateBehaviorInViewTile(LevelTilemap level);
    bool IsEmpty();
    void OverrideData(LevelTilemap level, TileLevelData data);
    bool HaveBehavior();
    bool HaveBehaviorViewUpdate();
    bool HaveBehaviorUpdate();
    bool OverrideColor(LevelTilemap level);
    void Destroy(LevelTilemap level);
    void Remove(LevelTilemap level);
    Color GetColor(LevelTilemap level);
}
