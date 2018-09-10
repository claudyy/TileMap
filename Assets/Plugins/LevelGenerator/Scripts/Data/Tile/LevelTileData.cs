using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ClaudeFehlen.ItemSystem;
using UnityEngine.Tilemaps;
public class BaseTileBehvior
{

    public virtual void Init(LevelTilemap level, BaseLevelTile tile)
    {
    }
    public virtual void Update(LevelTilemap level, BaseLevelTile tile,float deltaTime)
    {

    }
    public virtual void UpdateInView(LevelTilemap level, BaseLevelTile tile, float deltaTime) {

    }
    public virtual TileBase GetTile(BaseLevelTile tile, LevelTilemap level) {
        return tile.data.GetTile();
    }

    public virtual bool OverrideColor(BaseLevelTile levelTile, LevelTilemap level) {
        return false;
    }

    public virtual Color GetColor(BaseLevelTile levelTile, LevelTilemap level) {
        return Color.white;
    }
    public virtual void OnErase(BaseLevelTile levelTile, LevelTilemap level) {

    }

    public virtual bool NeedUpdate() {
        return false;
    }

    public virtual bool NeedUpdateView() {
        return false;
    }

    public virtual void OnRemove(BaseLevelTile levelTile, LevelTilemap level) {
    }
    public virtual void OnDestry(BaseLevelTile levelTile, LevelTilemap level) {

    }
    public virtual void OnDamage(LevelTilemap level, TileDamageType type, int damage) {
    }
}
[CreateAssetMenu(menuName ="Data/LevelGeneration/TileData")]
public class LevelTileData : ScriptableObject{

    public TileBase tile;
    public bool isWalkable;
    public Color editorColor;
    public virtual BaseTileBehvior GetBehavior()
    {
        return new BaseTileBehvior();
    }
    public virtual TileBase GetTile() {
        return tile;
    }
    public virtual void Init(LevelTilemap map,BaseLevelTile tile) {

    }
    public virtual void Remove(LevelTilemap map, BaseLevelTile tile) {

    }
    public virtual string DebugInfo() {
        return name.ToString();
    }
}
