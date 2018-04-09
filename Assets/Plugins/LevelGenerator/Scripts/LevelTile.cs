using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[System.Serializable]
public class LevelTile {
    const int defaultTileID = 29;
    public int tileID;
    public int health;

    public TileLevelData data;
    public BaseTileBehvior behavior;
    public Vector2Int pos;

    public bool needUpdate ;
    public LevelTile()
    {
        data = null;
        tileID = defaultTileID;
    }
    public LevelTile(Vector2Int pos)
    {
        tileID = defaultTileID;
        data = null;
        this.pos = pos;
    }
    public LevelTile(Vector3Int pos, TileLevelData data,LevelTilemap level) {
        tileID = defaultTileID;
        this.pos = new Vector2Int(pos.x,pos.y);
        OverrideData(level, data);
    }
    public void Init(LevelTilemap level)
    {
        if (data == null) {
            behavior = null;
        } else {
            behavior = data.GetBehavior();
            behavior.Init(level,this);
        }
        needUpdate = true;
    }
    public void UpdateBehaviorTile(LevelTilemap level)
    {
        if(behavior != null)
            behavior.Update(level,this);
    }
    public void UpdateBehaviorInViewTile(LevelTilemap level) {
        if (behavior != null)
            behavior.UpdateInView(level, this);
    }

    public bool IsEmpty()
    {
        if (data == null)
            return true;
        return data.tile == null;
    }

    public void OverrideData(LevelTilemap level,TileLevelData data)
    {
        this.data = data;
        if(data != null) {
            health = data.health;
        }
        //tileID = data.tileID;
        
        Init(level);
    }
    static LevelTile _Empty;
    public static LevelTile Empty
    {
        get
        {
            if (_Empty == null)
                _Empty = new LevelTile();
            return _Empty;
        }
    }

    public bool HaveBehavior() {
        return behavior != null;
    }
    public bool HaveBehaviorViewUpdate() {
        if (HaveBehavior() == false)
            return false;
        return behavior.NeedUpdateView();
    }
    public bool HaveBehaviorUpdate() {
        if (HaveBehavior() == false)
            return false;
        return behavior.NeedUpdate();
    }
    public bool OverrideColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return false;
        return behavior.OverrideColor(this,level);
    }
    public virtual void ApplyDamage(LevelTilemap level,TileDamageType type, int damage) {
        if (HaveBehavior())
            behavior.OnDamage(this,level, type, damage);
        if (health == -1)
            return;
        if (data == null || data.type != type)
            return;
        if (health > damage) {
            health -= damage;
            return;
        }
        Destroy(level);
    }
    public virtual void Destroy(LevelTilemap level) {
        if (HaveBehavior())
            behavior.OnDestry(this, level);
        Remove(level);
        level.DestroyWall(new Vector2Int((int)pos.x, (int)pos.y));
    }
    public virtual void Remove(LevelTilemap level) {
        if (HaveBehavior())
            behavior.OnRemove(this,level);
    }
    public Color GetColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return Color.white;
        return behavior.GetColor(this,level);
    }
}
