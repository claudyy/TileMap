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
    public TileSaveData(LevelTileData data) {
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
    public LevelTileData defaultTileData;
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
    public bool useLUT;
    public int sizeX;
    public int sizeY;
    public SaveUtility saveUtility;
    public Action<LevelTilemap> onLoaded;
    public Action<LevelTilemap> onLoadChunk;
    public Action<LevelTilemap,Vector2Int,Vector3Int> onLoadChunkNewMidChunk;
    public Action<LevelTilemap,BaseLevelTile> onLoadTileFirstTime;
    public Action<LevelTilemap, BaseLevelTile> onInitTile;
    public Action<LevelTilemap, BaseLevelTile> onRemoveTile;

    private void Awake()
    {
        
    }
    protected Coroutine updateVisibleCoroutine;
    protected Coroutine updateAllCoroutine;
    private void Start() {

        if (loadOnAwake) {
            Load();
        }
    }
    protected virtual void InitChunkFromDataName() {
        for (int x = 0; x < levelChunkX; x++) {
            for (int y = 0; y < levelChunkY; y++) {
                GetTileMap(x, y).Init(this);
            }
        }
    }
    #region Unity Tilemap Utility
    public virtual void SetTile(Vector3Int pos, TileBase tile) {
        if(tilemap.GetTile(IndexToTilemap(pos)) != tile)
            tilemap.SetTile(IndexToTilemap(pos), tile);
    }
    public virtual void SetTiles(Vector3Int[] pos, TileBase[] tiles) {
        Vector3Int[] worldpos = new Vector3Int[pos.Length];
        for (int i = 0; i < pos.Length; i++) {
            worldpos[i] = IndexToTilemap(pos[i]);
        }
        tilemap.SetTiles(worldpos, tiles);
    }
    Vector3Int VectorIntTemp;
    public Vector3Int GetTileMapSize() {
        Vector3Int size = Vector3Int.zero;
        for (int x = 0; x < tilemap.size.x; x++) {
            VectorIntTemp.x = x;
            VectorIntTemp.y = 0;
            VectorIntTemp.z = 0;
            if (tilemap.GetTile(VectorIntTemp) == null)
                break;
            size.x += 1;
        }
        for (int y = 0; y < tilemap.size.y; y++) {
            VectorIntTemp.x = 0;
            VectorIntTemp.y = y;
            VectorIntTemp.z = 0;
            if (tilemap.GetTile(VectorIntTemp) == null)
                break;
            size.y += 1;
        }
        return size;
    }
    public virtual LevelTileData LoadDataFromTileMap(Vector3Int pos, LevelTileData[] allData) {
       var tile = tilemap.GetTile(pos);
        for (int i = 0; i < allData.Length; i++) {
            if (tile == allData[i].tile)
                return allData[i];
        }
        return null;
    }
    public TileMapSaveData GetSaveDataFromTileMap() {
        var size = GetTileMapSize();
        TileMapSaveData st = new TileMapSaveData();
        st.sizeX = size.x;
        st.sizeY = size.y;
        Vector3Int pos = new Vector3Int(0, 0, 0);
        TileSaveData[] datas = new TileSaveData[size.x * size.y];
        var allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                pos.x = x;
                pos.y = y;
                datas[x + y * size.x] = new TileSaveData(LoadDataFromTileMap(pos, allData));
            }
        }
        st.tiles = datas;
        return st;
    }
    public virtual void ClearTilemap() {
        tilemap.ClearAllTiles();
    }
    public void EditorApplySaveFileToTilmap(TileMapSaveData st) {
        ClearTilemap();
        var allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();
        LevelTileData tile = null;
        Vector3Int pos = new Vector3Int(0, 0, 0);
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tile = GetDataFromList(st.tiles[x + y * st.sizeX].tileDataName, allData);
                EditorApplySaveFileToTile(x, y, tile);
            }
        }
    }
    public virtual void EditorApplySaveFileToTile(int x, int y, LevelTileData data) {
        SetTile(IndexToTilemap(x, y), data.GetTile());
    }
    #endregion

    protected virtual void Init() {
        sizeX = levelChunkX* chunkSize;
        sizeY = levelChunkY* chunkSize;
        if (useLUT)
            InitRenderTextures();
    }
    void SetupChunks() {
        tileMapList = new Dictionary<int, SubTileMap>();
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
                    LoadChunkNewMidChunk(x, y,midChunk);
                UpdateVisibleChunk(x, y);
            }
        }
        yield return null;
        updateVisibleCoroutine = StartCoroutine(UpdateVisible());
        lastMidChunk = midChunk;
        
    }
    public virtual void LoadChunkNewMidChunk(int cx, int cy, Vector3Int midChunk) {
        var chunk = GetTileMap(cx, cy);
        if (chunk.isLoaded == false)
            LoadChunkFirstTime(cx,cy);
        if (onLoadChunkNewMidChunk != null)
            onLoadChunkNewMidChunk(this,new Vector2Int(cx,cy), midChunk);
    }
    public virtual void LoadChunkFirstTime(int cx, int cy) {
        var chunk = GetTileMap(cx, cy);


        if (chunk.isLoaded == true)
            return;
        var tiles = new TileBase[chunkSize* chunkSize];
        var pos = new Vector3Int[chunkSize* chunkSize];
        BaseLevelTile tileTemp = null;
        TileBase tileViewTemp = null;
        LevelTileData dataTemp = null;
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                vec3IntTemp.x = x + cx * chunkSize;
                vec3IntTemp.y = y + cy * chunkSize;
                tileTemp = GetITile(vec3IntTemp.x, vec3IntTemp.y);

                if (tileTemp != null) {
                    dataTemp = tileTemp.data;
                }
                if (dataTemp != null) {
                    tileViewTemp = tileTemp.data.tile;
                    LoadTileFirstTime(tileTemp);
                }
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
        Vector3Int midChunk = PosToChunk((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y);
        vec3IntTemp.x = x - midChunk.x;//dir
        vec3IntTemp.y = y - midChunk.y;
        vec3IntTemp.z = 0;
        if (useLUT) {
            SetRenderTexture(vec3IntTemp.x, vec3IntTemp.y, GetRenderAllTexutre(x, y));
            Shader.SetGlobalVector("_ChunckCenterPos", new Vector4(midChunk.x, midChunk.y, 0, 0));
            Shader.SetGlobalFloat("_ChunkSize", chunkSize);
        }
    }
    
    public virtual void LoadTileFirstTime(BaseLevelTile tile) {
        if (useLUT)
            UpdateRenderTexutre(tile);
        if (onLoadTileFirstTime != null)
            onLoadTileFirstTime(this, tile);
    }


    #region Load Save
    public void Load() {
        XmlSerializer serializer = new XmlSerializer(typeof(TileMapSaveData));
        if(Application.isEditor == false) {
            TextAsset textAsset = (TextAsset)Resources.Load(saveUtility.fileName);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(textAsset.text);
            XmlReader reader = new XmlNodeReader(xmldoc);
            TileMapSaveData result = (TileMapSaveData)serializer.Deserialize(reader);

            Load(result);
            return;
        }
        FileStream stream = new FileStream(saveUtility.GetPath(), FileMode.Open);



        var st = serializer.Deserialize(stream) as TileMapSaveData;
        stream.Close();
        Load(st);
    }
    public void Load(TileMapSaveData st) {
        if(updateVisibleCoroutine != null)
            StopCoroutine(updateVisibleCoroutine);
        ClearTilemap();
        Resize(st.sizeX, st.sizeY);//inits chunk
        LevelTileData tile = null;
        //UnityEngine.Debug.Log(sizeX + sizeY * sizeX);
        //UnityEngine.Debug.Log(st.tiles.Length);
        StringBuilder str = new StringBuilder();
        Stopwatch w = new Stopwatch();

        

        str.Append("Load Data : ");
        w.Reset();
        w.Start();
        var allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();
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
        if (onLoaded != null)
            onLoaded(this);

        //UpdateViews();
        if (loadAllChunks) {
            for (int x = 0; x < levelChunkX; x++) {
                for (int y = 0; y < levelChunkY; y++) {
                    LoadChunkFirstTime(x, y);
                }
            }
        }
        



        str.AppendLine(" Duration: " + w.Elapsed);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(str.ToString());
#endif
        //if(Application.isEditor == false) {
            updateVisibleCoroutine = StartCoroutine(UpdateVisible());
            updateAllCoroutine = StartCoroutine(UpdateAllTiles());
        //}

    }
    public LevelTileData[] GetTileLevelDataFromSaveFile(TileMapSaveData st) {
        var allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();
        var tiles = new LevelTileData[st.sizeX * st.sizeY];
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tiles[x+y*st.sizeX] = GetDataFromList(st.tiles[x + y * st.sizeX].tileDataName, allData);
            }
        }
        return tiles;
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
        LevelTileData tile = null;
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                cx = PosToChunk(x, y).x;
                cy = PosToChunk(x, y).y;
                tile = GetTileMap(cx, cy).GetTile(PosToPosInChunk(x,y).x, PosToPosInChunk(x, y).y).data;
                st.tiles[x+y*sizeX] = new TileSaveData(tile);
            }
        }

        XmlSerializer serializer = new XmlSerializer(typeof(TileMapSaveData));
        FileStream stream = new FileStream(saveUtility.GetPath(), FileMode.Create);
        serializer.Serialize(stream,st);
        stream.Close();
    }
    public void CreateSaveFile(TileMapSaveData st,SaveUtility saveUtility) {
        XmlSerializer serializer = new XmlSerializer(typeof(TileMapSaveData));
        FileStream stream = new FileStream(saveUtility.GetPath(), FileMode.Create);
        serializer.Serialize(stream, st);
        stream.Close();
# if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    #endregion

    public void AddGameObjectToTile(BaseLevelTile tile,BaseTileMapGameobject prefab) {
        var go = Instantiate(prefab, transform);
        vec3IntTemp.x = tile.pos.x;
        vec3IntTemp.y = tile.pos.y;
        vec3IntTemp.z = 0;
        go.transform.position = IndexToWorldPos(vec3IntTemp);
        tile.AddGo(go);
    }
    public virtual Vector3Int IndexToTilemap(Vector3Int index) {
        return index;
    }
    public virtual Vector3Int IndexToTilemap(int x, int y) {
        vec3IntTemp.x = x;
        vec3IntTemp.y = y;
        vec3IntTemp.z = 0;
        return IndexToTilemap(vec3IntTemp);
    }
    public virtual Vector3Int WorldPosToIndex(Vector3 worldPos) {
        return new Vector3Int((int)worldPos.x,(int)worldPos.y,0);
    }
    public virtual Vector3 IndexToWorldPos(Vector3Int index) {
        return index;
    }

    public LevelTileData GetDataFromList(string name, LevelTileData[] list) {
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
        GenerateTileSets();
    }

    protected SubTileMap GetTileMap(int x, int y)
    {
        return tileMapList[x + y * levelChunkX];
    }
    public void UpdateAllViews()
    {
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                StartCoroutine(GetTileMap(x, y).UpdateViewChunke(this));
            }
        }
    }
    public void UpdateViews(int x, int y) {
        StartCoroutine(GetTileMap(PosToChunk(x,y).x, PosToChunk(x, y).y).UpdateViewChunke(this));
    }
    public T GetTile<T>(Vector2Int pos, bool ignoreLinkedTile = true) where T: BaseLevelTile {
        return GetTile<T>(pos.x,pos.y, ignoreLinkedTile);
    }
    public BaseLevelTile GetITile(Vector2Int pos, bool ignoreLinkedTile = true) {
        return GetTile<BaseLevelTile>(pos.x, pos.y, ignoreLinkedTile);
    }
    public BaseLevelTile GetITile(int x, int y,bool ignoreLinkedTile = true) {

        return GetTile<BaseLevelTile>(x, y, ignoreLinkedTile);
    }
    public T GetTile<T>(int x, int y, bool ignoreLinkedTile = true) where T: BaseLevelTile {
        if (x >= sizeX || y >= sizeY || x < 0 || y < 0)
            return null;

        int posX = x % chunkSize;
        int posY = y % chunkSize;
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;
        if (ignoreLinkedTile == false && GetTileMap(chunkPosX, chunkPosY).GetTile(posX, posY).linkedTile != null)
            return (T)GetTileMap(chunkPosX, chunkPosY).GetTile(posX, posY).linkedTile;
        return (T)GetTileMap(chunkPosX, chunkPosY).GetTile(posX, posY);
    }
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
    public Vector2Int PosToChunkVec2(int x, int y) {
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;
        return new Vector2Int(chunkPosX, chunkPosY);
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
        tilemap.Erase(PosToPosInChunk(x,y),this);
        //SetTile(IndexToTilemap(x, y), null);
    }
    public void Erase(Vector2Int pos) {
        Erase(pos.x, pos.y);
    }
    public virtual void OverrideTile(int x, int y, LevelTileData data)
    {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);
        if (data == null) {
            Erase(new Vector2Int(x, y));
            return;
        }
        RemoveTile( x, y);
        var tile = GetITile(x, y);
        tile.OverrideData(this, data, x, y);
        if (tile.HaveBehaviorUpdate())
            tilemap.updateTiles.Add(tile);
        if (tile.HaveBehaviorViewUpdate())
            tilemap.updateTilesInView.Add(tile);
        //OverrideTile(tile);
        //UpdateTile(x, y, this);
        tile.viewNeedUpdate = true;
        UpdateViewTile(tile);
    }

    public void OverrideTile(Vector2Int pos,LevelTileData data)
    {
        OverrideTile(pos.x, pos.y, data);
    }
    public void OverrideTile(Vector3Int pos, LevelTileData data) {
        OverrideTile(pos.x, pos.y, data);
    }
    public void OverrideDataNameTile(int x, int y, LevelTileData data) {
        var tilemap = GetTileMap(PosToChunk(x,y).x, PosToChunk(x,y).y);
        tilemap.OverrideDataNameTile(PosToPosInChunk(x, y).x, PosToPosInChunk(x, y).y, data);
    }
    public virtual void RemoveTile(int x, int y) {
        var tilemap = GetTileMap(PosToChunk(x, y).x, PosToChunk(x, y).y);

        var tile = GetITile(x, y);
        tile.Remove(this);
        if (tile.HaveBehaviorUpdate())
            tilemap.updateTiles.Remove(tile);
        if (tile.HaveBehaviorViewUpdate())
            tilemap.updateTilesInView.Remove(tile);
    }
    public bool IsEmpty(int x, int y){
        if (IsPosOutOfBound(x, y))
            return true;
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
    protected Vector3Int vec3IntTemp = new Vector3Int();
    public abstract BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data);
    public virtual void UpdateViewTile(BaseLevelTile tile) {
        if (tile.viewNeedUpdate == false)
            return;
        vec3IntTemp = IndexToTilemap(tile.pos);
        if (tile.IsEmpty() == false) {
            var tileDisplay = tile.data.GetTile();
            if (tilemap.GetTile(vec3IntTemp)!= tileDisplay)
            tilemap.SetTile(vec3IntTemp, tileDisplay);

        } else {
            tilemap.SetTile(vec3IntTemp, null);
        }
        if (useLUT)
            UpdateRenderTexutre(tile);
        tile.viewNeedUpdate = false;
    }
    public virtual void ChangedTile(BaseLevelTile tile) {
        var x = tile.x;
        var y = tile.y;
        var tilemap = GetTileMap(PosToChunk(x, y).x, PosToChunk(x, y).y);
        tilemap.ChangedTile(tile);
    }
    public virtual void DestroyTile(BaseLevelTile tile) {
        Erase(tile.x,tile.y);
        tile.Destroy(this);
    }

    public virtual void InitTile(BaseLevelTile tile) {
        if (onInitTile != null)
            onInitTile(this, tile);
    }
    public bool CanSetLinkedTile(Vector2Int size, BaseLevelTile tile) {
        int startX = tile.x;
        int startY = tile.y;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                if (IsEmpty(startX + x, startY + y) == false || GetITile(startX + x, startY + y).linkedTile != null)
                    return false;
            }
        }
        return true;
    }
    public void SetLinks(Vector2Int size, BaseLevelTile tile) {
        int startX = tile.x;
        int startY = tile.y;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                if (x == 0 && y == 0)
                    continue;
                SetLink(tile, startX + x, startY + y);
            }
        }
    }
    public void SetLink(BaseLevelTile tile, int otherX, int otherY) {
        var otherTile = GetITile(otherX, otherY);
        otherTile.linkedTile = tile;
    }
    internal void RemoveLinks(Vector2Int size, BaseLevelTile tile) {
        int startX = tile.x;
        int startY = tile.y;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                if (x == 0 && y == 0)
                    continue;
                RemoveLink(tile, startX + x, startY + y);
            }
        }
    }
    public void RemoveLink(BaseLevelTile tile, int otherX, int otherY) {
        var otherTile = GetITile(otherX, otherY);
        if (otherTile.linkedTile == tile)
            otherTile.linkedTile = null;
    }
    #region LookUpTexture
    RenderTexture[] chunksRT;
    public RenderTexture[] allChunksRT;
    RenderTexture chunkBufferColor;
    public Material colorDrawer;
    void InitRenderTextures() {
        colorDrawer = new Material(Shader.Find("Hidden/PixelDrawer"));
        chunkBufferColor = CreateRenderTexture();
        chunksRT = new RenderTexture[9];
        for (int i = 0; i < chunksRT.Length; i++) {
            chunksRT[i] = CreateRenderTexture();
        }
        allChunksRT = new RenderTexture[levelChunkX * levelChunkY];
        for (int x = 0; x < levelChunkX; x++) {
            for (int y = 0; y < levelChunkY; y++) {
                allChunksRT[x + y * levelChunkX] = CreateRenderTexture();
            }
        }
    }
    bool RenderTextureIndexInOfRange(int x, int y) {
        if (x < -1 || x > 1)
            return false;
        if (y < -1 || y > 1)
            return false;
        return true;
    }
    RenderTexture GetRenderTexutre(int x, int y) {
        if (RenderTextureIndexInOfRange(x, y) == false)
            return null;

        return chunksRT[1 + x + (1 + y) * 3];
    }
    
    void SetRenderTexture(int x, int y, RenderTexture rt) {
        if (RenderTextureIndexInOfRange(x, y) == false)
            return;
        chunksRT[1 + x + (1 + y) * 3] = rt;
        Shader.SetGlobalTexture("_ChunckCenterTex", GetRenderTexutre(0, 0));
        Shader.SetGlobalTexture("_ChunckLeftTex", GetRenderTexutre(-1, 0));
        Shader.SetGlobalTexture("_ChunckTopTex", GetRenderTexutre(0, 1));
        Shader.SetGlobalTexture("_ChunckRightTex", GetRenderTexutre(1, 0));
        Shader.SetGlobalTexture("_ChunckBottomTex", GetRenderTexutre(0, -1));
        Shader.SetGlobalTexture("_ChunckTopLeftTex", GetRenderTexutre(-1, 1));
        Shader.SetGlobalTexture("_ChunckTopRightTex", GetRenderTexutre(1, 1));
        Shader.SetGlobalTexture("_ChunckBottomRightTex", GetRenderTexutre(1, -1));
        Shader.SetGlobalTexture("_ChunckBottomLeftTex", GetRenderTexutre(-1, -1));
        
    }
    RenderTexture GetRenderAllTexutre(int x, int y) {

        return allChunksRT[x + y * levelChunkX];
    }
    RenderTexture CreateRenderTexture() {
        var r = new RenderTexture(chunkSize, chunkSize, 16);
        r.filterMode = FilterMode.Point;
        r.wrapMode = TextureWrapMode.Clamp;
        return r;
    }
    void UpdateRenderTexutre(BaseLevelTile tile, RenderTexture rt) {

        colorDrawer.SetFloat("_ChunckSize", chunkSize);
        var pos = PosToPosInChunk(tile.pos.x, tile.pos.y);
        colorDrawer.SetFloat("_PosX", pos.x);
        colorDrawer.SetFloat("_PosY", pos.y);
        Color c = GetLookUpTextureColor(tile);
        colorDrawer.SetColor("_Color", c);

        Graphics.Blit(rt, chunkBufferColor);
        Graphics.Blit(chunkBufferColor, rt, colorDrawer);

    }
    void UpdateRenderTexutre(BaseLevelTile tile) {
        vec3IntTemp = PosToChunk(tile.x, tile.y);
        UpdateRenderTexutre(tile, allChunksRT[vec3IntTemp.x + vec3IntTemp.y*levelChunkX]);
    }
    public virtual Color GetLookUpTextureColor(BaseLevelTile tile) {
        return Color.white;
    }
    #endregion
}
