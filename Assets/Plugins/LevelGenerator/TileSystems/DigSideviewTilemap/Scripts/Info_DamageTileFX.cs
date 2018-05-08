using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
[System.Serializable]
public class Info_DamageTileFX  {
    public bool IsEmpty() {
        return damageTiles.Count == 0;
    }
    public List<TileBase> damageTiles;
    public TileBase GetDamageTile(float percent) {
        return damageTiles[(int)Mathf.Clamp(damageTiles.Count * percent,0, damageTiles.Count - 1)];
    }

}
