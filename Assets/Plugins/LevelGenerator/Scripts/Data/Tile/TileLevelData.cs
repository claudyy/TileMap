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
    public virtual void Init(LevelTilemap level, LevelTile tile)
    {
    }
    public virtual void Update(LevelTilemap level, LevelTile tile)
    {
        deltaTime = Time.time - lastUpdateTime;
        lastUpdateTime = Time.time;
    }
    public virtual void UpdateInView(LevelTilemap level, LevelTile tile) {
        inViewdeltaTime = Time.time - inViewlastUpdateTime;
        inViewlastUpdateTime = Time.time;
    }
    public virtual Tile GetTile(LevelTile tile, LevelTilemap level) {
        return tile.data.GetTile();
    }

    public virtual bool OverrideColor(LevelTile levelTile, LevelTilemap level) {
        return false;
    }

    public virtual Color GetColor(LevelTile levelTile, LevelTilemap level) {
        return Color.white;
    }
    public virtual void OnErase(LevelTile levelTile, LevelTilemap level) {

    }

    public virtual bool NeedUpdate() {
        return false;
    }

    public virtual bool NeedUpdateView() {
        return false;
    }

    public virtual void OnRemove(LevelTile levelTile, LevelTilemap level) {
    }
    public virtual void OnDestry(LevelTile levelTile, LevelTilemap level) {

    }
    public virtual void OnDamage(LevelTilemap level, TileDamageType type, int damage) {
    }
}
[CreateAssetMenu(menuName ="Data/LevelGeneration/TileData")]
public class TileLevelData : ScriptableObject{

    public Tile tile;
    public TileBase ruleTile;
    public int health = -1;
    //[EnumFlags]
    public TileDamageType type;
    //public BaseItemData dropItem;
    public virtual BaseTileBehvior GetBehavior()
    {
        return new BaseTileBehvior();
    }
    public virtual Tile GetTile() {
        return tile;
    }
}
