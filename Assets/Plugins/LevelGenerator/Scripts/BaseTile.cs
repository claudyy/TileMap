using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile {

    public int health;

    public TileLevelData data;
    public BaseTileBehvior behavior;
    public Vector2Int pos;

    public bool needUpdate;

    public virtual void Init(LevelTilemap level) {
        if (data == null) {
            behavior = null;
        } else {
            behavior = data.GetBehavior();
            behavior.Init(level, this);
        }
        needUpdate = true;
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

    public void OverrideData(LevelTilemap level, TileLevelData data) {
        this.data = data;

        Init(level);
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
        Remove(level);
    }
    public virtual void Remove(LevelTilemap level) {
        if (HaveBehavior())
            behavior.OnRemove(this, level);
    }
    public virtual Color GetColor(LevelTilemap level) {
        if (HaveBehavior() == false)
            return Color.white;
        return behavior.GetColor(this, level);
    }
}
