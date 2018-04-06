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
    public List<LevelTile> updateTilesInView = new List<LevelTile>();
    public List<LevelTile> updateTiles = new List<LevelTile>();

    //[HideInInspector]
    public int chunkSize;
    public bool wasLoaded;
    public void Init( LevelTilemap level)
    {
        tiles = new LevelTile[chunkSize * chunkSize * chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                tiles[x + y * chunkSize] = new LevelTile(ToTilePosition(x, y), GetData(x,y),level);
            }
        }
        for (int x = 0; x < chunkSize; x++){
            for (int y = 0; y < chunkSize; y++){
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
        if (GetTile(x, y).needUpdate == false)
            return;
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
        GetTile(x, y).needUpdate = false;
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
        return new Vector3Int(chunkSize * posX + x, chunkSize * posY + y, 0);
    }
    public void UpdateInView(LevelTilemap level)
    {
        for (int i = 0; i < updateTilesInView.Count; i++) {
            updateTilesInView[i].UpdateBehaviorInViewTile(level);
        }
        UpdateViewChunke(level);
        return;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                if(GetTile(x, y).IsEmpty() == false)
                    GetTile(x, y).UpdateBehaviorInViewTile(level);
            }
        }
    }
    public void Update(LevelTilemap level) {


        for (int i = 0; i < updateTiles.Count; i++) {
            updateTiles[i].UpdateBehaviorTile(level);
        }
        return;
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                if (GetTile(x, y).IsEmpty() == false)
                    GetTile(x, y).UpdateBehaviorTile(level);
            }
        }
    }
    public void Hide()
    {
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    public void Erase(Vector3Int pos, LevelTilemap level)
    {
        RemoveTile(pos.x, pos.y);
        GetTile(pos.x, pos.y).OverrideData(level,null);
        UpdateTile(pos.x, pos.y, level);

    }
    public void OverrideTile(int x, int y, LevelTilemap level, TileLevelData data) {
        RemoveTile(x, y);
        var tile = GetTile(x, y);
        tile.OverrideData(level,data);
        if (tile.HaveBehaviorUpdate())
            updateTiles.Add(tile);
        if (tile.HaveBehaviorViewUpdate())
            updateTilesInView.Add(tile);

        UpdateTile(x, y, level);
    }
    void RemoveTile(int x, int y) {
        var tile = GetTile(x, y);
        tile.Remove();
        if (tile.HaveBehaviorUpdate())
            updateTiles.Remove(tile);
        if (tile.HaveBehaviorViewUpdate())
            updateTilesInView.Remove(tile);
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
        return tilesData[x + y * chunkSize];
    }
    #endregion
}
