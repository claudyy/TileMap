using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class BaseLevelTile {


    public LevelTileData data;
    public BaseTileBehvior behavior;
    public Vector3Int pos;
    public int x;
    public int y;

    public bool viewNeedUpdate;
    BaseTileMapGameobject go;
    public BaseLevelTile linkedTile;
    public Action<LevelTilemap,BaseLevelTile> onRemove;
    public Action<LevelTilemap,BaseLevelTile> onInit;
    public void Init(LevelTilemap level, LevelTileData data, int x, int y) {
        Init(level, data, new Vector3Int(x, y, 0));
    }
    public virtual void Init(LevelTilemap level, LevelTileData data, Vector3Int pos) {
        this.data = data;
        this.pos = pos;
        x = pos.x;
        y = pos.y;
        if (data == null) {
            behavior = null;
        } else {
            behavior = data.GetBehavior();
            data.Init(level, this);
            behavior.Init(level, this);
        }
        viewNeedUpdate = true;
        if (onInit != null)
            onInit(level, this);
    }
    public virtual void UpdateBehaviorTile(LevelTilemap level) {
        if (behavior != null)
            behavior.Update(level, this);
    }
    public virtual void UpdateBehaviorInViewTile(LevelTilemap level) {
        if (behavior != null)
            behavior.UpdateInView(level, this);
    }

    public virtual bool IsEmpty() {
        if (data == null)
            return true;
        return data.tile == null;
    }
    public void OverrideData(LevelTilemap level, LevelTileData data, int x,int y) {
        Init(level, data, x,y);
    }
    public void OverrideData(LevelTilemap level, LevelTileData data, Vector3Int pos) {
        Init(level,data,pos);
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
    public virtual bool OverrideColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return false;
        return behavior.OverrideColor(this, level);
    }

    public virtual void Destroy(LevelTilemap level) {
        if (HaveBehavior())
            behavior.OnDestry(this, level);
        if (go != null)
            GameObject.Destroy(go.gameObject);

    }
    public virtual void Remove(LevelTilemap level) {
        if (HaveBehavior())
            behavior.OnRemove(this, level);
        if (data != null)
            data.Remove(level, this);
        if (onRemove != null)
            onRemove(level,this);
    }
    public virtual Color GetColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return Color.white;
        return behavior.GetColor(this, level);
    }
    public virtual void AddGo(BaseTileMapGameobject go) {
        this.go = go;
    }
    public virtual string DebugInfo() {
        return "";
    }
    public string DebugDataInfo() {
        if (data == null)
            return "";
        return data.DebugInfo();
    }
   
}
