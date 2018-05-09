using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Tilemap_Dig : LevelTilemap {
    public TileBase lightTile;
    public TileBase waterTile;
    public Tilemap backgroundTileMap;
    public Tilemap sunLightMap;
    public Tilemap lightMap;
    public Tilemap objectTileMap;
    public Tilemap waterTileMap;
    public Tilemap damageTileMap;
    public Info_DamageTileFX defaultDamageFX;
    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new LevelTile_Dig();
    }
    protected override void Init() {
        base.Init();
        for (int x = 0; x < sizeX; x++) {
            LightOnPos(x, sizeY-1);
        }
    }
    public override void ClearTilemap() {
        base.ClearTilemap();
        backgroundTileMap.ClearAllTiles();
        sunLightMap.ClearAllTiles();
        lightMap.ClearAllTiles();
        objectTileMap.ClearAllTiles();
        waterTileMap.ClearAllTiles();
        damageTileMap.ClearAllTiles();
    }
    public override void OverrideTile(int x, int y, LevelTileData data) {
        base.OverrideTile(x, y, data);

        if (IsEmpty(x,y) == false)
            ShadowOnPos(x, y);

        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;
        SetBackground(x, y, digData.GetBackgroundTile(this, x, y));
    }
    public override void DestroyTile(BaseLevelTile tile) {
        base.DestroyTile(tile);
        LevelTile_Dig tileAbove = GetTile<LevelTile_Dig>(tile.pos.x, tile.pos.y + 1);
        if (tileAbove.inSunLight == true)
            LightOnPos(tile.pos.x, tile.pos.y, true);
        else
            ShadowOnPos(tile.pos.x, tile.pos.y);
    }

    public void SetBackground(int x, int y, TileBase tile) {

        backgroundTileMap.SetTile(IndexToTilemap(x,y), tile);
    }
    public override void LoadTileFirstTime(BaseLevelTile tile) {
        base.LoadTileFirstTime(tile);
        var data = tile.data;
        if (data == null)
            return;
        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;
        SetBackground(tile.x, tile.y, digData.GetBackgroundTile(this, tile.x, tile.y));
        lightMap.RemoveTileFlags(IndexToTilemap(tile.pos), TileFlags.LockColor);
        lightMap.SetTile(IndexToTilemap(tile.pos), lightTile);
        sunLightMap.RemoveTileFlags(IndexToTilemap(tile.pos), TileFlags.LockColor);
        sunLightMap.SetTile(IndexToTilemap(tile.pos), lightTile);
        var digTile = tile as LevelTile_Dig;
        if (digTile != null) {
            Color c = Color.white;
            c.a = digTile.inSunLight ? 1 : 0;
            sunLightMap.SetColor(IndexToTilemap(digTile.pos), c);
            c.a = digTile.GetLight(true);
            lightMap.SetColor(IndexToTilemap(digTile.pos), c);

        }

    }
    public override void EditorApplySaveFileToTile(int x, int y, LevelTileData data) {
        base.EditorApplySaveFileToTile(x, y, data);
        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;
        SetBackground(x, y, digData.GetBackgroundTile(this, x, y));


    }
    public void ApplyDamage(Vector3Int pos, Vector3 dir,TileDamageType type, int damage) {
        if (IsPosOutOfBound(pos.x, pos.y))
            return;
        var tile = GetTile<LevelTile_Dig>(pos.x,pos.y,false);
        tile.ApplyDamage(this,type, damage);

        if (tile.currentHealth > 0) {
            var digData = tile.data as LevelTileData_Dig;
            if (digData != null && digData.damageFX.IsEmpty() == false)
                damageTileMap.SetTile(IndexToTilemap(pos), digData.damageFX.GetDamageTile(tile.HealthPercent));
            else
                damageTileMap.SetTile(IndexToTilemap(pos), defaultDamageFX.GetDamageTile(tile.HealthPercent));

        } else {

            damageTileMap.SetTile(IndexToTilemap(pos), null);
        }
    }
    
    public override void UpdateViewTile(BaseLevelTile tile) {
        if (tile.viewNeedUpdate == false)
            return;
        var digTile = tile as LevelTile_Dig;
        Color c = Color.white;
        c.a = digTile.inSunLight ? 1 : 0;
        sunLightMap.SetColor(IndexToTilemap(digTile.pos), c);
        c.a = digTile.GetLight(true);
        lightMap.SetColor(IndexToTilemap(digTile.pos), c);


        base.UpdateViewTile(tile);
    }
    #region Light
    public void LightOnPos(int x, int posY, bool sunLight = false) {
        LevelTile_Dig tile = GetTile<LevelTile_Dig>(x, posY + 1);
        if (sunLight == true && tile.inSunLight == false)
            return;
        var pos = Vector3Int.zero;
        bool lastHit = false;
        float value = 1;
        int penetration = 5;
        for (int y = posY; y > 0; y--) {
            if (penetration <= 0)
                break;
            tile = GetTile<LevelTile_Dig>(x, y);
            if (tile == null)
                break;
            value -= GetAbsorption(tile);
            if (value == 1)
                tile.inSunLight = true;

            SetLight(tile, value);
            if (tile.IsEmpty() == false) {
                penetration--;
                lastHit = true;
            }


        }
    }

    public void ShadowOnPos(int x, int posY) {
        LevelTile_Dig tile = GetTile<LevelTile_Dig>(x, posY);
        var pos = Vector3Int.zero;
        var data = tile.data as LevelTileData_Dig;
        //if (data != null)
        //    return;
        bool lastHit = false;
        float value = 1;
        if (tile.IsEmpty())
            value = 0;
        int penetration = 5;
        for (int y = posY; y > 0; y--) {
            if (penetration <= 0)
                break;
            tile = GetTile<LevelTile_Dig>(x, posY);
            if (tile == null)
                break;
            value -= GetAbsorption(tile);
            tile.inSunLight = false;

            SetLight(tile, value);
            if (tile.IsEmpty() == false) {
                penetration--;
                lastHit = true;
            }
        }
    }
    float GetAbsorption(LevelTile_Dig tile) {
        if (tile.IsEmpty()) {
            return 0;
        }
        var data = tile.data as LevelTileData_Dig;

        if (data != null) {
            return data.isTranslucent? 0 : 1;
        } else {

            return 1;
        }
    }
    public void SetLight(LevelTile_Dig leveltile, float sunLight) {
        leveltile.sunLight = sunLight;
        leveltile.viewNeedUpdate = true;
        int x = leveltile.x;
        int y = leveltile.y;
        if (IsPosOutOfBound(x - 1, y) == false)GetITile(x - 1, y).viewNeedUpdate = true;
        if (IsPosOutOfBound(x + 1, y) == false)GetITile(x + 1, y).viewNeedUpdate = true;
        if (IsPosOutOfBound(x , y - 1) == false)GetITile(x, y - 1).viewNeedUpdate = true;
        if (IsPosOutOfBound(x , y + 1) == false)GetITile(x, y + 1).viewNeedUpdate = true;

    }
    #endregion
}
