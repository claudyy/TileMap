using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
//[Serializable]
public class SubTileMap{

    //public Tilemap tilemap;
    public TileLevelData empty;
    public int posX;
    public int posY;
    //[SerializeField]
    //[HideInInspector]
    public LevelTile[] tiles;
    public TileLevelData[] tilesData;
    //[HideInInspector]
    public int chunkSize;
    public void Init(LevelTilemap level)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                GetTile(x, y).Init(level);
                level.tilemap.RemoveTileFlags(ToTilePosition(x, y), TileFlags.LockColor);

            }
        }
    }
    
    public void UpdateViewChunke(LevelTilemap level)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                UpdateTile(x, y, level);
            }
        }

    }
    public void UpdateTile(int x,int y,LevelTilemap level) {
        if (GetTile(x, y).IsEmpty() == false) {
            if (GetTile(x, y).HaveBehavior()) {
                if (GetTile(x, y).data.ruleTile != null) {
                    level.tilemap.SetTile(ToTilePosition(x, y), GetTile(x, y).data.ruleTile);
                } else {
                    level.tilemap.SetTile(ToTilePosition(x, y), GetTile(x, y).behavior.GetTile(GetTile(x, y), level));
                }
            } else {
                level.tilemap.SetTile(ToTilePosition(x, y), GetTile(x, y).data.GetTile());
            }
            if (GetTile(x, y).OverrideColor(level)) {
                Color c = GetTile(x, y).GetColor(level);
                level.tilemap.RemoveTileFlags(ToTilePosition(x, y), TileFlags.LockColor);
                level.tilemap.SetColor(ToTilePosition(x, y), c);

            }

        } else {
            level.tilemap.SetTile(ToTilePosition(x, y), null);
        }
    }
    
    public TileLevelData GetDataFromList(string name,TileLevelData[] list) {
        if (name == "")
            return null;
        for (int i = 0; i < list.Length; i++) {
            if (list[i].name == name)
                return list[i];
        }
        return null;
    }
    public Vector3Int ToTilePosition(int x,int y) {
        return new Vector3Int(chunkSize * posX+ x, chunkSize * posY + y, 0);
    }
    public void UpdateTileMap(LevelTilemap level)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                if(GetTile(x, y).IsEmpty() == false)
                    GetTile(x, y).UpdateTile(level);
            }
        }
        UpdateViewChunke(level);
    }
    public void Hide()
    {
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    public void Erase(Vector3Int pos, LevelTilemap level)
    {
        GetTile(pos.x, pos.y).OverrideData(level,null);
        UpdateTile(pos.x, pos.y, level);

    }
    public void OverrideTile(int x, int y, LevelTilemap level, TileLevelData data) {
        GetTile(x, y).OverrideData(level,data);
        UpdateTile(x, y, level);
    }
    
    public LevelTile GetTile(int x, int y)
    {
        return tiles[y * chunkSize + x];
    }
    #region Generation
    public void GenerateDataList(int posX, int posY, int chunkSize) {
        this.posX = posX;
        this.posY = posY;
        this.chunkSize = chunkSize;
        tilesData = new TileLevelData[chunkSize * chunkSize];
    }
    public void UpdateFromData(LevelTilemap level) {
        TileLevelData data = null;
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                data = tilesData[y * chunkSize + x];
                if (data == null)
                    continue;
                level.tilemap.SetTile(ToTilePosition(x, y), data.GetTile());
            }
        }
    }
    public void OverrideDataNameTile(int x, int y, TileLevelData data) {
        tilesData[y * chunkSize + x] = data;
    }
    public TileLevelData GetData(int x, int y) {
        return tilesData[y * chunkSize + x];
    }
    #endregion
}
