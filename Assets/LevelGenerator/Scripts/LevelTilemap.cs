﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System;
using System.Linq;
[ExecuteInEditMode]
public class LevelTilemap : MonoBehaviour {
    public TileLevelData defaultTileData;
    public Tilemap tilemap;
    public GameObject grid;
    public GameObject tilemapPrefab;
    //[HideInInspector]
    public SubTileMap[] tileMapList;
    public int levelChunkX = 2;
    public int levelChunkY = 2;
    public int chunkSize = 60;
    //public PickUp_Item itemPickUpPrefab;
    public float updateRate = .02f;
    float curUpdate;
    int updateX = 0;
    int updateY = 0;
    public bool onlyUpdateVisible;
    //
    public int sizeX
    {
        get
        {
            return levelChunkX * chunkSize;
        }
    }
    public int sizeY
    {
        get
        {
            return levelChunkY * chunkSize;
        }
    }

    private void Awake()
    {
        return;
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                tileMapList[y * levelChunkX + x].Init(this);
            }
        }

    }
    private void Update()
    {
        //curUpdate += Time.deltaTime;
        //if (curUpdate < updateRate)
        //    return;
        //curUpdate = 0;
        //if (onlyUpdateVisible) {
        //    UpdateVisible();
        //    return;
        //}
        //updateX++;
        //if(updateX >= levelChunkX)
        //{
        //    updateX = 0;
        //    updateY++;
        //    if (updateY >= levelChunkY)
        //    {
        //        updateY = 0;
        //    }
        //}
        //    
        //GetTileMap(updateX, updateY).UpdateTileMap(this);

    }
    void UpdateVisible() {
        Vector3Int midChunk = PosToChunk((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y);
        int fromX = Mathf.Clamp(midChunk.x - 1,0, levelChunkX -1);
        int toX = Mathf.Clamp(midChunk.x + 1,0, levelChunkX - 1);
        int fromY = Mathf.Clamp(midChunk.y - 1,0, levelChunkY - 1);
        int toY = Mathf.Clamp(midChunk.y + 1,0, levelChunkY - 1);
        for (int x = fromX; x <= toX; x++) {
            for (int y = fromY; y <= toY; y++) {
                GetTileMap(x, y).UpdateTileMap(this);
            }
        }
    }
    public IEnumerator UpdatePrefabFromData() {
        for (int cx = 0; cx < levelChunkX; cx++) {
            for (int cy = 0; cy < levelChunkY; cy++) {
                for (int x = 0; x < chunkSize; x++) {
                    for (int y = 0; y < chunkSize; y++) {
                        var tile = GetTileMap(cx, cy).GetData(x, y);
                        if (tile == null)
                            tile = defaultTileData;
                        tilemapPrefab.GetComponentInChildren<Tilemap>().SetTile(new Vector3Int(x + cx * chunkSize, y + cy * chunkSize,0), tile.GetTile());
                        //tilemap.SetTile(new Vector3Int(x + cx * chunkSize, y + cy * chunkSize,0), tile.GetTile());
                    }
                }
                
            }
        }
        yield return null;
    }
    public void GenerateGridFromPrefab() {
        if (grid != null)
            DestroyImmediate(grid);
        GameObject go = Instantiate(tilemapPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        grid = go;
        tilemap = go.GetComponentInChildren<Tilemap>();
        //tilemap.SetTile(new Vector3Int(1,0, 0), defaultTileData.GetTile());
        //tilemap.ClearAllTiles();
    }
    public TileLevelData GetDataFromList(string name, TileLevelData[] list) {
        if (name == "")
            return null;
        for (int i = 0; i < list.Length; i++) {
            if (list[i].name == name)
                return list[i];
        }
        return null;
    }

    void GenerateTileSets()
    {
        //tilemap.ClearAllTiles();
        if (tileMapList != null)
        {
            for (int x = 0; x < levelChunkX; x++)
            {
                for (int y = 0; y < levelChunkY; y++)
                {
                   // if (tileMapList[x, y] == null)
                   //     continue;
                   // DestroyImmediate(tileMapList[x, y].gameObject);
                }
            }
        }
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        tileMapList = new SubTileMap[levelChunkX * levelChunkY];
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                //var tilemap = Instantiate(subTilemapPrefab, transform.position + new Vector3(chunkSize * x, chunkSize * y), Quaternion.identity);
                //tilemap.transform.SetParent(transform);
                //var go = new GameObject();
                //go.transform.SetParent(transform);
                //var tilemap = go.AddComponent<SubTileMap>();
                var tilemap = new SubTileMap();
                tilemap.GenerateDataList(x, y, chunkSize);
                tileMapList[ y * levelChunkX + x] = tilemap;
            }
        }
    }

    internal void Resize(int x, int y) {
        int chunkX = 1 + x / chunkSize;
        int chunkY = 1 + y / chunkSize;
        levelChunkX = chunkX;
        levelChunkY = chunkY;
        GenerateTileSets();
    }

    SubTileMap GetTileMap(int x, int y)
    {
        return tileMapList[y * levelChunkX + x];
    }
    public void UpdateViews()
    {
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                GetTileMap(x, y).UpdateViewChunke(this);
            }
        }
    }
    public void UpdateViews(int x, int y) {
        GetTileMap(PosToChunk(x,y).x, PosToChunk(x, y).y).UpdateViewChunke(this);
    }
    public LevelTile GetTile(Vector2Int pos)
    {
        return GetTile(pos.x,pos.y);
    }
    public LevelTile GetTile(int x, int y)
    {
        if (x >= sizeX || y >= sizeY || x < 0 || y < 0)
            return LevelTile.Empty;
        int posX = x % chunkSize;
        int posY = y % chunkSize;
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;

        return GetTileMap(chunkPosX, chunkPosY).GetTile(posX, posY);
    }
    public Vector3Int PosToChunk(int x, int y)
    {
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;
        return new Vector3Int(chunkPosX, chunkPosY, 0);
    }
    public Vector3Int PosToPosInChunk(int x,int y)
    {
        int posX = x % chunkSize;
        int posY = y % chunkSize;
        return new Vector3Int(posX, posY, 0);
    }
    public Vector3Int PosToChunk(Vector2Int pos)
    {
        return PosToChunk(pos.x,pos.y);
    }
    public Vector3Int PosToPosInChunk(Vector2Int pos)
    {
        return PosToPosInChunk(pos.x,pos.y);
    }
    public Vector2Int GetCeneter() {
        return new Vector2Int((int)((float)sizeX / 2), (int)((float)sizeY / 2));
    }
    public bool IsPosOutOfBound(int x, int y)
    {
        return x >= sizeX || y >= sizeY || x < 0 || y < 0;
    }
    //[Button]
    public void Hide()
    {
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                GetTileMap(x, y).Hide();
            }
        }
    }
    public void ApplyDamage(Vector2Int pos, TileDamageType type, int damage)
    {
        if (IsPosOutOfBound(pos.x,pos.y))
            return;
        var tile = GetTile(pos);
        int health = tile.health;
        if (health > damage)
        {
            tile.health -= damage;
            return;
        }
        DestroyWall(new Vector2Int((int)pos.x, (int)pos.y));
    }
    public void DestroyWall(Vector2Int pos)
    {
        var tile = GetTile((int)pos.x, (int)pos.y);
        if (tile == null)
            return;
        var tilemap = GetTileMap(PosToChunk(pos).x, PosToChunk(pos).y);
        tilemap.Erase(PosToPosInChunk(pos),this);
    }
    public void Erase(int x, int y) {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);
        tilemap.Erase(PosToPosInChunk(x,y),this);
    }
    public void Erase(Vector2Int pos) {
        var tilemap = GetTileMap(PosToChunk(pos).x, PosToChunk(pos).y);
        tilemap.Erase(PosToPosInChunk(pos),this);
    }
    public void OverrideTile(int x, int y, TileLevelData data)
    {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);

        if (data != null)
            tilemap.OverrideTile(PosToPosInChunk(x,y).x, PosToPosInChunk(x, y).y,this, data);
        else
            Erase(new Vector2Int(x, y));
    }
    public void OverrideTile(Vector2Int pos,TileLevelData data)
    {
        var tilemap = GetTileMap(PosToChunk(pos).x, PosToChunk(pos).y);

        if (data != null)
            tilemap.OverrideTile(PosToPosInChunk(pos).x, PosToPosInChunk(pos).y,this, data);
        else
            Erase(new Vector2Int(pos.x, pos.y));
    }
    public void OverrideDataNameTile(int x, int y, TileLevelData data) {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);
        tilemap.OverrideDataNameTile(PosToPosInChunk(x, y).x, PosToPosInChunk(x, y).y, data);
    }

    public bool IsEmpty(int x, int y){
        if (GetTile(x, y).data == null)
            return true;
		return GetTile(x, y).data.tile == null;
	}

	public bool IsOnWall(int x, int y){

		if (IsOnHorizontalWall (x, y) == true || IsOnVerticalWall (x, y) == true)
			return true;

		return false;
	}
	public bool IsOnHorizontalWall(int x, int y){
        if (IsEmpty(x, y) == false)
            return false;
        if (IsEmpty(x - 1, y) == IsEmpty(x + 1, y))
			return false;
		return true;
	}
	public bool IsOnVerticalWall(int x, int y){
        if (IsEmpty(x, y) == false)
            return false;
        if (IsEmpty(x, y - 1) == IsEmpty(x, y + 1))
			return false;
		return true;
	}
    public void Explosion(int cx, int cy,float raduis) {
        for (int x = -1; x < 1; x++) {
            for (int y = -1; y < 1; y++) {
                Erase(cx + x , cy + y);
            }
        }
    }
}