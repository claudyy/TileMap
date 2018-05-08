using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(menuName = "TileMap/Tileset/DigTile")]
public class LevelTileData_Dig : LevelTileData {
    public int maxHealth;
    public TileBase backgroundTile;
    public Info_DamageTileFX damageFX;
    [Header("Heat")]
    public int baseHeat;
    public int emitHeat;
    public int emitHeatRange;
    public float heatAbsorption;
    [Header("Light")]
    public int baseLight;
    public int emitLight;
    public int emitLightRange;
    public float lightAbsorption;

    internal TileBase GetBackgroundTile(Tilemap_Dig tilemap_Dig, int x, int y) {
        if (tilemap_Dig.IsEmpty(x, y + 1) == false)
            return backgroundTile;
        return null;
    }
}
