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

    public void Init(LevelTilemap level)
    {
        if (data == null) {
            behavior = null;
        } else {
            behavior = data.GetBehavior();
            behavior.Init(level,this);
        }
    }
    public void UpdateTile(LevelTilemap level)
    {
        if(behavior != null)
            behavior.Update(level,this);
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

    public bool OverrideColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return false;
        return behavior.OverrideColor(this,level);
    }

    public Color GetColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return Color.white;
        return behavior.GetColor(this,level);
    }
}
