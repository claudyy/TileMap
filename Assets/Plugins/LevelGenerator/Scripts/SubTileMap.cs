using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
//[Serializable]
public class SubTileMap{

    //public Tilemap tilemap;
    public LevelTileData empty;
    public int posX;
    public int posY;
    //[SerializeField]
    //[HideInInspector]
    public BaseLevelTile[] tiles;
    public LevelTileData[] tilesData;
    public List<BaseLevelTile> updateTilesInView = new List<BaseLevelTile>();
    public List<BaseLevelTile> updateTiles = new List<BaseLevelTile>();

    //[HideInInspector]
    public int chunkSize;
    public bool isLoaded;
    public void Init(LevelTilemap level)
    {
        isLoaded = false;
        tiles = new BaseLevelTile[chunkSize * chunkSize * chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                tiles[x + y * chunkSize] = level.CreateTile(ToTilePosition(x, y), GetData(x, y));// new LevelTile(ToTilePosition(x, y), GetData(x,y),level);
            }
        }
        for (int x = 0; x < chunkSize; x++){
            for (int y = 0; y < chunkSize; y++){
                var tile = GetTile(x, y);
                tile.Init(level,GetData(x,y), ToTilePosition(x, y));
                //level.tilemap.RemoveTileFlags(ToTilePosition(x, y), TileFlags.LockColor);
                if (tile.HaveBehaviorUpdate())
                    updateTiles.Add(tile);
                if (tile.HaveBehaviorViewUpdate())
                    updateTilesInView.Add(tile);
                level.InitTile(tile);
            }
        }
    }
    
    public IEnumerator UpdateViewChunke(LevelTilemap level)
    {
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                level.UpdateViewTile(GetTile(x, y));
            }
            yield return null;
        }

    }

    
    public LevelTileData GetDataFromList(string name,LevelTileData[] list) {
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
        level.LoadChunk(UpdateViewChunke(level));
    }
    public void Update(LevelTilemap level) {


        for (int i = 0; i < updateTiles.Count; i++) {
            updateTiles[i].UpdateBehaviorTile(level);
        }
        return;
    }
    public void Hide()
    {
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    public void Erase(Vector3Int pos, LevelTilemap level)
    {
        RemoveTile(level,pos.x, pos.y);
        GetTile(pos.x, pos.y).OverrideData(level,null,pos);
        //UpdateTile(pos.x, pos.y, level);
    }
    
    void RemoveTile(LevelTilemap level, int x, int y) {
        var tile = GetTile(x, y);
        tile.Remove(level);
        if (tile.HaveBehaviorUpdate())
            updateTiles.Remove(tile);
        if (tile.HaveBehaviorViewUpdate())
            updateTilesInView.Remove(tile);
    }
    public BaseLevelTile GetTile(int x, int y)
    {
        return tiles[y * chunkSize + x];
    }
    #region Generation
    public void GenerateDataList(int posX, int posY, int chunkSize) {
        this.posX = posX;
        this.posY = posY;
        this.chunkSize = chunkSize;
        tilesData = new LevelTileData[chunkSize * chunkSize];
    }
    public void UpdateFromData(LevelTilemap level) {
        LevelTileData data = null;
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                data = tilesData[y * chunkSize + x];
                if (data == null)
                    continue;
                level.tilemap.SetTile(ToTilePosition(x, y), data.GetTile());
            }
        }
    }
    public void OverrideDataNameTile(int x, int y, LevelTileData data) {
        tilesData[y * chunkSize + x] = data;
    }
    public LevelTileData GetData(int x, int y) {
        return tilesData[x + y * chunkSize];
    }

    public virtual void ChangedTile(int x, int y) {
        var tile = GetTile(x, y);
        if (tile.HaveBehaviorUpdate())
            updateTiles.Add(tile);
        if (tile.HaveBehaviorViewUpdate())
            updateTilesInView.Add(tile);
    }
    #endregion
}
