using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ClaudeFehlen.ItemSystem;
using UnityEngine.Tilemaps;
public class BaseTileBehvior
{
    float lastUpdateTime = 0;
    public float deltaTime;
    float inViewlastUpdateTime = 0;
    public float inViewdeltaTime;
    public virtual void Init(LevelTilemap level, BaseTile tile)
    {
    }
    public virtual void Update(LevelTilemap level, BaseTile tile)
    {
        deltaTime = Time.time - lastUpdateTime;
        lastUpdateTime = Time.time;
    }
    public virtual void UpdateInView(LevelTilemap level, BaseTile tile) {
        inViewdeltaTime = Time.time - inViewlastUpdateTime;
        inViewlastUpdateTime = Time.time;
    }
    public virtual TileBase GetTile(BaseTile tile, LevelTilemap level) {
        return tile.data.GetTile();
    }

    public virtual bool OverrideColor(BaseTile levelTile, LevelTilemap level) {
        return false;
    }

    public virtual Color GetColor(BaseTile levelTile, LevelTilemap level) {
        return Color.white;
    }
    public virtual void OnErase(BaseTile levelTile, LevelTilemap level) {

    }

    public virtual bool NeedUpdate() {
        return false;
    }

    public virtual bool NeedUpdateView() {
        return false;
    }

    public virtual void OnRemove(BaseTile levelTile, LevelTilemap level) {
    }
    public virtual void OnDestry(BaseTile levelTile, LevelTilemap level) {

    }
    public virtual void OnDamage(LevelTilemap level, TileDamageType type, int damage) {
    }
}
[CreateAssetMenu(menuName ="Data/LevelGeneration/TileData")]
public class TileLevelData : ScriptableObject{

    public TileBase tile;
    public Color editorColor;
    public virtual BaseTileBehvior GetBehavior()
    {
        return new BaseTileBehvior();
    }
    public virtual TileBase GetTile() {
        return tile;
    }
    public virtual void Init(LevelTilemap map,BaseTile tile) {

    }
}
