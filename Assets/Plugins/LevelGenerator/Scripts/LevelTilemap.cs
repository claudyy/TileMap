using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Xml;
using System.IO;
using System.Xml.Serialization;
[System.Serializable]
public class TileSaveData {
    public string tileDataName="";
    public int health;
    public TileSaveData() {

    }
    public TileSaveData(TileLevelData data) {
        if (data == null)
            return;
        tileDataName = data.name;
    }
}
[System.Serializable]
public class TileMapSaveData {
    public TileMapSaveData() {

    }
    public int sizeX;
    public int sizeY;
    public TileSaveData[] tiles;
}

public abstract class LevelTilemap : MonoBehaviour {
    public TileLevelData defaultTileData;
    public Tilemap tilemap;
    public GameObject grid;
    public Dictionary<int,SubTileMap> tileMapList;
    public int levelChunkX = 2;
    public int levelChunkY = 2;
    public int chunkSize = 60;
    public float updateRate = .02f;
    public bool loadOnAwake;
    public bool loadAllChunks;
    public bool onlyUpdateVisible;
    public bool dontUpdate;

    public int sizeX;
    public int sizeY;

    private void Awake()
    {
        if (loadOnAwake) {
            Load();
        }
    }
    protected Coroutine updateVisibleCoroutine;
    protected Coroutine updateAllCoroutine;
    private void Start() {


    }
    protected virtual void InitChunkFromDataName() {
        for (int x = 0; x < levelChunkX; x++) {
            for (int y = 0; y < levelChunkY; y++) {
                GetTileMap(x, y).Init(this);
            }
        }
    }
    protected virtual void Init() {
        sizeX = levelChunkX* chunkSize;
        sizeY = levelChunkY* chunkSize;
    }
    void SetupChunks() {
        tileMapList = new Dictionary<int, SubTileMap>();
    }
    private void Update()
    {
    }
    IEnumerator UpdateAllTiles() {
        for (int x = 0; x < levelChunkX; x++) {
            for (int y = 0; y < levelChunkY; y++) {
                GetTileMap(x, y).Update(this);
                yield return new WaitForSeconds(updateRate);
            }
        }
        updateAllCoroutine = StartCoroutine(UpdateAllTiles());

    }
    public virtual void LoadChunk(IEnumerator enumerator) {
        StartCoroutine(enumerator);
    }
    protected Vector3Int lastMidChunk = Vector3Int.one*-1;
    public IEnumerator UpdateVisible() {
        Vector3Int midChunk = PosToChunk((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y);
        int fromX = Mathf.Clamp(midChunk.x - 1,0, levelChunkX -1);
        int toX = Mathf.Clamp(midChunk.x + 1,0, levelChunkX - 1);
        int fromY = Mathf.Clamp(midChunk.y - 1,0, levelChunkY - 1);
        int toY = Mathf.Clamp(midChunk.y + 1,0, levelChunkY - 1);
        for (int x = fromX; x <= toX; x++) {
            for (int y = fromY; y <= toY; y++) {
                if(midChunk != lastMidChunk)
                    LoadVisibleChunk(x, y);
                UpdateVisibleChunk(x, y);
            }
        }
        yield return null;
        updateVisibleCoroutine = StartCoroutine(UpdateVisible());
        lastMidChunk = midChunk;
    }
    public virtual void LoadVisibleChunk(int cx, int cy) {
        var chunk = GetTileMap(cx, cy);


        if (chunk.isLoaded == true)
            return;
        var tiles = new TileBase[chunkSize* chunkSize];
        var pos = new Vector3Int[chunkSize* chunkSize];
        BaseTile tileTemp = null;
        TileBase tileViewTemp = null;
        TileLevelData dataTemp = null;
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                vec3IntTemp.x = x + cx * chunkSize;
                vec3IntTemp.y = y + cy * chunkSize;
                tileTemp = GetITile(vec3IntTemp.x, vec3IntTemp.y);
                if (tileTemp != null)
                    dataTemp = tileTemp.data;
                if (dataTemp != null)
                    tileViewTemp = tileTemp.data.tile;
                if (tileViewTemp != null) {
                    tiles[x+y*chunkSize] = tileViewTemp;
                    pos[x + y * chunkSize] = vec3IntTemp;
                } else {
                    tiles[x + y * chunkSize] = null;
                    pos[x + y * chunkSize] = vec3IntTemp;
                }
                dataTemp = null;
                tileViewTemp = null;
            }
        }
        chunk.isLoaded = true;
        SetTiles(pos, tiles);
    }
    public virtual void UpdateVisibleChunk(int x,int y) {
        GetTileMap(x, y).UpdateInView(this);
    }
    

    #region Load Save
    public void Load() {
        XmlSerializer serializer = new XmlSerializer(typeof(TileMapSaveData));
        FileStream stream = new FileStream(Application.dataPath + "/Test.xml", FileMode.Open);
        var st = serializer.Deserialize(stream) as TileMapSaveData;
        stream.Close();
        Load(st);
    }
    public void Load(TileMapSaveData st) {
        if(updateVisibleCoroutine != null)
            StopCoroutine(updateVisibleCoroutine);

        Resize(st.sizeX, st.sizeY);//inits chunk
        TileLevelData tile = null;
        //UnityEngine.Debug.Log(sizeX + sizeY * sizeX);
        //UnityEngine.Debug.Log(st.tiles.Length);
        StringBuilder str = new StringBuilder();
        Stopwatch w = new Stopwatch();

        

        str.Append("Load Data : ");
        w.Reset();
        w.Start();
        var allData = Resources.LoadAll("", typeof(TileLevelData)).Cast<TileLevelData>().ToArray();
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tile = GetDataFromList(st.tiles[x + y * st.sizeX].tileDataName, allData);
                OverrideDataNameTile(x, y, tile);
            }
        }
        str.AppendLine(" Duration: " + w.Elapsed);
        //UpdatePrefabFromData();
        //GenerateGridFromPrefab();
        str.Append("Init: ");
        w.Reset();
        w.Start();
        InitChunkFromDataName();
        Init();
        str.AppendLine(" Duration: " + w.Elapsed);

        str.Append("TileMap Cleare : ");
        w.Reset();
        w.Start();
        //tilemap.ClearAllTiles();
        str.AppendLine(" Duration: " + w.Elapsed);

        str.Append("UpdateView : ");
        w.Reset();
        w.Start();
        //UpdateViews();
        if (loadAllChunks) {
            for (int x = 0; x < levelChunkX; x++) {
                for (int y = 0; y < levelChunkY; y++) {
                    LoadVisibleChunk(x, y);
                }
            }
        }
        



        str.AppendLine(" Duration: " + w.Elapsed);

        UnityEngine.Debug.Log(str.ToString());
        if(Application.isEditor == false) {
            updateVisibleCoroutine = StartCoroutine(UpdateVisible());
            updateAllCoroutine = StartCoroutine(UpdateAllTiles());
        }

    }
    public void ApplySaveFileToMap(TileMapSaveData st) {
        var allData = Resources.LoadAll("", typeof(TileLevelData)).Cast<TileLevelData>().ToArray();
        TileLevelData tile = null;
        Vector3Int pos = new Vector3Int(0,0,0);
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tile = GetDataFromList(st.tiles[x + y * st.sizeX].tileDataName, allData);
                pos.x = x;
                pos.y = y;
                if (tile != null)
                    tilemap.SetTile(pos, tile.GetTile());
            }
        }
    }
    public void Save() {
       var st =  new TileMapSaveData();
        st.sizeX = sizeX;
        st.sizeY = sizeY;
        st.tiles = new TileSaveData[sizeX + sizeY * sizeX];
        UnityEngine.Debug.Log(sizeX + sizeY * sizeX);
        UnityEngine.Debug.Log(st.tiles.Length);

        int cx = 0;
        int cy = 0;
        TileLevelData tile = null;
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                cx = PosToChunk(x, y).x;
                cy = PosToChunk(x, y).y;
                tile = GetTileMap(cx, cy).GetTile(PosToPosInChunk(x,y).x, PosToPosInChunk(x, y).y).data;
                st.tiles[x+y*sizeX] = new TileSaveData(tile);
            }
        }

        XmlSerializer serializer = new XmlSerializer(typeof(TileMapSaveData));
        FileStream stream = new FileStream(Application.dataPath + "/Test.xml", FileMode.Create);
        serializer.Serialize(stream,st);
        stream.Close();
    }
    #endregion
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
        tileMapList = new Dictionary<int, SubTileMap>();
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                var tilemap = new SubTileMap();
                tilemap.GenerateDataList(x, y, chunkSize);
                tileMapList[ y * levelChunkX + x] = tilemap;
            }
        }
    }

    internal void Resize(int x, int y) {
        int chunkX = Mathf.CeilToInt((float)x / chunkSize);
        int chunkY = Mathf.CeilToInt((float)y / chunkSize);
        levelChunkX = chunkX;
        levelChunkY = chunkY;
        UnityEngine.Debug.Log(x + " : " + chunkX + " / " + chunkSize);
        GenerateTileSets();
    }

    protected SubTileMap GetTileMap(int x, int y)
    {
        return tileMapList[x + y * levelChunkX];
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
    public T GetTile<T>(Vector2Int pos) where T: BaseTile {
        return GetTile<T>(pos.x,pos.y);
    }
    public BaseTile GetITile(Vector2Int pos) {
        return GetTile<BaseTile>(pos.x, pos.y);
    }
    protected BaseTile GetITile(int x, int y) {
        return GetTile<BaseTile>(x, y);
    }
    public T GetTile<T>(int x, int y) where T: BaseTile {
        //if (x >= sizeX || y >= sizeY || x < 0 || y < 0)
        //    return LevelTile.Empty;
        int posX = x % chunkSize;
        int posY = y % chunkSize;
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;

        return (T)GetTileMap(chunkPosX, chunkPosY).GetTile(posX, posY);
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
    #region Funktionality
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

    public void DestroyWall(Vector2Int pos)
    {
        var tile = GetITile((int)pos.x, (int)pos.y);
        if (tile == null)
            return;
        var tilemap = GetTileMap(PosToChunk(pos).x, PosToChunk(pos).y);
        tilemap.Erase(PosToPosInChunk(pos),this );
    }
    public void Erase(int x, int y) {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);
        tilemap.Erase(PosToPosInChunk(x,y),this  );
    }
    public void Erase(Vector2Int pos) {
        var tilemap = GetTileMap(PosToChunk(pos).x, PosToChunk(pos).y);
        tilemap.Erase(PosToPosInChunk(pos),this );
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
        if (GetITile(x, y).data == null)
            return true;
		return GetITile(x, y).data.tile == null;
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
    #endregion
    protected Vector3Int vec3IntTemp;
    public abstract BaseTile CreateTile(Vector3Int pos, TileLevelData data);
    public virtual void UpdateTile(BaseTile tile) {
        if (tile.needUpdate == false)
            return;
        vec3IntTemp.x = tile.pos.x;
        vec3IntTemp.y = tile.pos.y;
        vec3IntTemp.z = 0;
        if (tile.IsEmpty() == false) {
            var tileDisplay = tile.data.GetTile();
            if (tilemap.GetTile(vec3IntTemp)!= tileDisplay)
            tilemap.SetTile(vec3IntTemp, tileDisplay);

        } else {
            tilemap.SetTile(vec3IntTemp, null);
        }
        tile.needUpdate = false;
    }
    public virtual void ChangedTile(ITile tile) {
        var x = tile.pos.x;
        var y = tile.pos.y;
        var tilemap = GetTileMap(PosToChunk(x, y).x, PosToChunk(x, y).y);
        tilemap.ChangedTile(PosToPosInChunk(x, y).x, PosToPosInChunk(x, y).y);
    }
    public virtual void DestroyTile(BaseTile tile) {

    }
    public virtual void OverrideTile(BaseTile tile) {

    }
    public virtual void InitTile(BaseTile tile) {

    }
    #region
    public virtual void SetTile(Vector3Int pos,TileBase tile) {
        tilemap.SetTile(pos, tile);
    }
    public virtual void SetTiles(Vector3Int[] pos, TileBase[] tiles) {
        tilemap.SetTiles(pos, tiles);
    }
    #endregion
}
