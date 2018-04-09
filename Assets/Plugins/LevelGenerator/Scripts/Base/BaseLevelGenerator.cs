using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using CreativeSpore.SuperTilemapEditor;

using System.Text;
using System.Diagnostics;
using System;

public class GeneratorMapData {
    public TileLevelData[] tiles;
    public int sizeX;
    public int sizeY;
    public GeneratorMapData(int sizeX, int sizeY,TileLevelData defaultTile) {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        tiles = new TileLevelData[sizeX*sizeY];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                tiles[x + y * sizeX] = defaultTile;
            }
        }
    }
    public void OverrideTile(int x,int y,TileLevelData data) {
        tiles[x + y *sizeX] = data;
    }
    public TileLevelData GetTile(int x, int y) {
        return tiles[x + y * sizeX];
    }
    public TileMapSaveData GetNewSaveFile() {
        var file = new TileMapSaveData();
        file.sizeX = sizeX;
        file.sizeY = sizeY;
        List<TileSaveData> saveTiles = new List<TileSaveData>();
        for (int y = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                saveTiles.Add(new TileSaveData(GetTile(x, y)));
            }
        }
        file.tiles = saveTiles.ToArray();
        return file;
    }

    public bool IsOnWall(int x, int y) {

        if (IsOnHorizontalWall(x, y) == true || IsOnVerticalWall(x, y) == true)
            return true;

        return false;
    }
    public bool IsOnHorizontalWall(int x, int y) {
        if (IsEmpty(x, y) == false)
            return false;
        if (IsEmpty(x - 1, y) == IsEmpty(x + 1, y))
            return false;
        return true;
    }
    public bool IsOnVerticalWall(int x, int y) {
        if (IsEmpty(x, y) == false)
            return false;
        if (IsEmpty(x, y - 1) == IsEmpty(x, y + 1))
            return false;
        return true;
    }

    public bool IsEmpty(int x, int y) {
        if (OutOfBound(x, y))
            return false;
        if (GetTile(x, y) == null)
            return true;
        return GetTile(x, y).tile == null;
    }
    public bool OutOfBound(int x, int y) {
        return x >= sizeX || y >= sizeY || x < 0 || y < 0;
    }
}
[RequireComponent(typeof(LevelTilemap))]
[ExecuteInEditMode()]
public class BaseLevelGenerator : MonoBehaviour {
    public TileLevelData defaultTileData;
    public LevelTilemap level;
    public StringBuilder result;
    public int border;
    protected double time;
    protected Stopwatch stopwatch;
    public bool fillWithDefault;
    //public List<BaseLevelStructure> structure;
    //public TileLevelData plant;
    //public BiomData biom;
    //public List<BiomData> biomList;
    protected int _progress;
    public virtual float GetProgress() {
        return (float)_progress / MaxProgressStep;
    }
    public virtual int MaxProgressStep {
        get {
            return 5;
        }
    }
    protected virtual IEnumerator TryGenerate() {
        structureList = new List<BaseLevelStructure>();
        yield return null;
    }
    protected virtual bool CheckGeneratedResult(List<BaseLevelStructure> structure) {
        return true;
    }
    protected virtual IEnumerator Generate(List<BaseLevelStructure> structure, GeneratorMapData map) {
        for (int i = 0; i < structure.Count; i++) {
            yield return StartCoroutine(structure[i].Generate(map));
        }
    }
    //
    protected List<BaseLevelStructure> structureList;
    public IEnumerator GenerateMap()
    {
        level.dontUpdate = true;
        yield return null;
        _progress = 0;
        var generationtime = UnityEditor.EditorApplication.timeSinceStartup;
        stopwatch = new Stopwatch();
        Stopwatch GenerationStopwatch = new Stopwatch();
        GenerationStopwatch.Start();
        result = new StringBuilder();
        int tries = 0;
        for (int i = 0; i < 12; i++) {
            tries++;

            yield return StartCoroutine(TryGenerate());
            if (CheckGeneratedResult(structureList))
                break;

        }
        _progress++;

        //rescale and move structures
        DebugCheckTimeStart("Move every thing");
        int minX = GetMinXFromStructure(structureList);
        int minY = GetMinYFromStructure(structureList);
        int moveX = 0 - minX + border;
        int moveY = 0 - minY + border;
        for (int i = 0; i < structureList.Count; i++) {
            structureList[i].Move(moveX, moveY);
        }
        _progress++;
        DebugCheckTimeEnd();

        DebugCheckTimeStart("Calculate Bounds");

        var bound = new Bounds(Vector3.one*.5f, Vector3.one);
        for (int i = 0; i < structureList.Count; i++) {
            structureList[i].EncapsulateBound(ref bound);
        }
        int mapSizeX = (int)bound.max.x + border;
        int mapSizeY = (int)bound.max.y + border;
        //level.Resize(, (int)bound.max.y + border);
        var map = new GeneratorMapData(mapSizeX, mapSizeY,defaultTileData);

        _progress++;
        DebugCheckTimeEnd();

        if (fillWithDefault) {
            DebugCheckTimeStart("Fill Default");
            //FillWithDataName(level,defaultTileData);
            DebugCheckTimeEnd();
        }

        DebugCheckTimeStart("Generating");
        yield return StartCoroutine(Generate(structureList,map));
        _progress++;
        DebugCheckTimeEnd();


        DebugCheckTimeStart("UpdateViews");
        level.Load(map.GetNewSaveFile());
        //level.Save();
        //level.Load();
        //yield return StartCoroutine(level.UpdatePrefabFromData());
        //level.GenerateGridFromPrefab();
        
        _progress++;
        DebugCheckTimeEnd();

        result.AppendLine("All Generation Duration: "+ GenerationStopwatch.Elapsed.Seconds + " : " + GenerationStopwatch.Elapsed.Milliseconds);
        result.AppendLine("Tries: "+ tries);
        UnityEngine.Debug.Log(result.ToString());
        yield return null;
        level.dontUpdate = false;


    }



    public int GetMinXFromStructure(List<BaseLevelStructure> s) {
        int x = int.MaxValue;
        for (int i = 0; i < s.Count; i++) {
            if (s[i].GetMinX() < x)
                x = s[i].GetMinX();
        }
        return x;
    }
    public int GetMinYFromStructure(List<BaseLevelStructure> s) {
        int y = int.MaxValue;
        for (int i = 0; i < s.Count; i++) {
            if (s[i].GetMinY() < y)
                y = s[i].GetMinY();
        }
        return y;
    }
    public void DebugCheckTimeStart(string name) {
        time = UnityEditor.EditorApplication.timeSinceStartup;
        stopwatch.Reset();
        stopwatch.Start();
        result.Append("Start Generating: " + name);
    }
    public void DebugCheckTimeEnd() {
        time -= UnityEditor.EditorApplication.timeSinceStartup;
        stopwatch.Stop();
        result.AppendLine("Duration: " + stopwatch.Elapsed.Seconds +": "+ stopwatch.Elapsed.Milliseconds);
    }
    public void DebugTryFail(string name) {
        result.AppendLine("Fail Because: " + name);
    }
    public void FillWithDefault(LevelTilemap level) {
        FillWithTile(level, defaultTileData);
    }
    public void FillWithTile(LevelTilemap level,TileLevelData data) {
        for (int x = 0; x < level.sizeX; x++)
        {
            for (int y = 0; y < level.sizeY; y++)
            {
                level.GetTile(x, y).OverrideData(level,data);
            }
        }
    }
    public void FillWithDataName(LevelTilemap level, TileLevelData data) {
        for (int x = 0; x < level.sizeX; x++) {
            for (int y = 0; y < level.sizeY; y++) {
                level.OverrideDataNameTile(x,y,data);
            }
        }
    }
    protected int CountOfStrcuture<T>(List<BaseLevelStructure> list) {
        int count = 0;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] is T)
                count++;
        }
        return count;
    }
    bool BoundListTouchesOtherBoundList(List<Bounds> bounds1,List<Bounds> bounds2){
		for (int b2 = 0; b2 < bounds1.Count; b2++) {
			if (BoundTouchesOtherBoundList(bounds1[b2],bounds2))
				return true;
		}
		return false;
	}
	bool BoundTouchesOtherBoundList(Bounds bound,List<Bounds> bounds){
		for (int b2 = 0; b2 < bounds.Count; b2++) {
			if (bound.Intersects (bounds [b2]))
				return true;
			}
		return false;
	}
    void GeneratePlant()
    {
        var target = GetEmptyTile();
        //if (target != null)
            //target.OverrideData(plant);
    }
    LevelTile GetEmptyTile()
    {
        int x = UnityEngine.Random.Range(0, level.sizeX);
        int y = UnityEngine.Random.Range(0, level.sizeY);
        for (int i = 0; i < 1000; i++)
        {
            if (level.GetTile(x, y).data.tile == null)
                return level.GetTile(x, y);
            x = UnityEngine.Random.Range(0, level.sizeX);
            y = UnityEngine.Random.Range(0, level.sizeY);
        }
        return null;
    }
    /*
    [System.Serializable]
    public struct Chunk
    {
        public int posX;
        public int posY;
        public LevelTile[,] tile;
        STETilemap target;
        public Chunk(int posX, int posY, STETilemap target)
        {
            this.posX = posX;
            this.posY = posY;
            this.target = target;
            tile = new LevelTile[LevelGenerator.chunkSize, LevelGenerator.chunkSize];
            for (int x = 0; x < LevelGenerator.chunkSize; x++)
            {
                for (int y = 0; y < LevelGenerator.chunkSize; y++)
                {
                    tile[x, y] = new LevelTile();
                }
            }
        }
        public void UpdateChunke()
        {
            for (int x = 0; x < LevelGenerator.chunkSize; x++)
            {
                for (int y = 0; y < LevelGenerator.chunkSize; y++)
                {
                    if (tile[x, y].tileID == -1)
                        target.Erase(x, y);
                    else
                        target.SetTile(x, y, tile[x, y].tileID);
                }
            }
            target.UpdateMeshImmediate();
            target.Refresh();
        }
    }
    public GameObject tilesetPrefab;
    public PickUp_Item itemPickUpPrefab;
    [HideInInspector]
    public Chunk[,] chunkGrid;
    public const int chunkSize = 60;
    public int levelChunkX = 2;
    public int levelChunkY = 2;
    float randomOffsetX;
    float randomOffsetY;
    public STETilemap[,] tileMapList;
    public List<BaseLevelStructure> structure;
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
        Generate();
    }
    void GenerateTileSets()
    {

        if(tileMapList!= null)
        {
            for (int x = 0; x < levelChunkX; x++)
            {
                for (int y = 0; y < levelChunkY; y++)
                {
                    if (tileMapList[x, y] == null)
                        continue;
                    DestroyImmediate(tileMapList[x, y].gameObject);
                }
            }
        }
        for (int i = transform.childCount - 1; i >=0 ; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        tileMapList = new STETilemap[levelChunkX, levelChunkY];
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                var tilemap = Instantiate(tilesetPrefab,transform.position+new Vector3(chunkSize * x,chunkSize*y),Quaternion.identity);
                tilemap.transform.SetParent(transform);
                tileMapList[x,y] = tilemap.GetComponent<STETilemap>();
            }
        }
        
    }
    //First Pass
    [System.Serializable]
    public struct WallGeneratorInfo
    {
        public float start;
        public TileLevelData data;
        public float density;
        public float end;
        public float smoothness;
        public float randomOffset;

    }
    public WallGeneratorInfo biom;
    public TileLevelData tileData1;
    public List<WallGeneratorInfo> wallList;
    public TileLevelData boraderWallData;
    public Texture2D border;
    public float borderFalloff = 0.5f;
    [System.Serializable]
    public struct OreInfo
    {
        public TileLevelData data;
        public Vector2Int depositSize;
        public Vector2Int depositCount;
        public int minDistanceBetweenDeposit;
    }
    public List<OreInfo> oreList;
    [Button]
	void Generate()
    {
        GenerateTileSets();

        randomOffsetX = Random.Range(0f,10);
        randomOffsetY = Random.Range(0f, 10);
        //Debug.Log(ReadTexture(border, .5f, .5f));
        chunkGrid = new Chunk[levelChunkX, levelChunkY];
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                chunkGrid[x, y] = new Chunk(x,y, tileMapList[x, y]);
            }
        }
        //First Pass
        for (int i = 0; i < wallList.Count; i++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    //(GetRandom(x, y)*(y/ sizeY)
                    if (GetRandom(x, y, wallList[i].randomOffset) * RandomHeight(y, wallList[i]) > .5f)
                    {
                        GetTile(x, y).OverrideData(wallList[i].data);
                    }
                }
            }
        }
        for (int i = 0; i < structure.Count; i++)
        {
            structure[i].Generate(this);
        }

        //Generate Ore
        for (int i = 0; i < oreList.Count; i++)
        {
            var ore = oreList[i];
            int count = Random.Range(ore.depositCount.x, ore.depositCount.y);
            for (int d = 0; d < count; d++)
            {
                int x = Random.Range(0, sizeX);
                int y = Random.Range(0, sizeY);
                int size = Random.Range(ore.depositSize.x, ore.depositSize.y);
                var positionList = GetOreDepositPositions(size, x, y);
                for (int s = 0; s < size; s++)
                {
                    if (positionList.Count == 0)
                        break;
                    int randomIndex = Random.Range(0, positionList.Count);
                    GetTile(positionList[randomIndex].x, positionList[randomIndex].y).OverrideData(ore.data);
                    positionList.RemoveAt(randomIndex);
                }
            }
        }


        //Generate Boarder
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                //(GetRandom(x, y)*(y/ sizeY)
                if (ReadTexture(border, (float)x / sizeX, (float)y / sizeY) * (GetRandom(x, y, Random.Range(0,100)+0.8f)) > borderFalloff)
                {
                    GetTile(x, y).OverrideData(boraderWallData);
                }
            }
        }

        //GenerateDisplay
        for (int x = 0; x < levelChunkX; x++)
        {
            for (int y = 0; y < levelChunkY; y++)
            {
                chunkGrid[x, y].UpdateChunke();

            }
        }
        
    }
    List<Vector2Int> GetOreDepositPositions(int size,int startX,int startY)
    {
        var list = new List<Vector2Int>();
        list.Add(new Vector2Int(startX, startY));
        for (int i = 1; i <= 1+size/7; i++)
        {
            list.Add(new Vector2Int(startX + i, startY ));
            list.Add(new Vector2Int(startX + i, startY + i));
            list.Add(new Vector2Int(startX, startY + i));
            list.Add(new Vector2Int(startX - i, startY + i));
            list.Add(new Vector2Int(startX - i, startY));
            list.Add(new Vector2Int(startX - i, startY - i));
            list.Add(new Vector2Int(startX, startY - i));
            list.Add(new Vector2Int(startX + i, startY - i));
        }
        RemoveOutOfBoundFromList(list);
        return list;
    }
    public void RemoveOutOfBoundFromList(List<Vector2Int> list)
    {
        for (int i = list.Count -1; i >=0 ; i--)
        {
            if (IsPosOutOfBound(list[i].x, list[i].y))
                list.RemoveAt(i);
        }
    }
    float ReadTexture(Texture2D tex,float x, float y)
    {
        int posX = (int)(x * tex.width);
        int posY = (int)(y * tex.height);
        //Debug.Log(posX);
        //Debug.Log(posY);
        //Debug.Log(tex.GetPixel(posX, posY));

        return tex.GetPixel(posX, posY).r;
    }
    float RandomHeight(float y, WallGeneratorInfo biom)
    {
        y += biom.start;
        return Mathf.SmoothStep(0, biom.density, (float)((sizeY - y) * biom.smoothness) /sizeY + biom.end);
    }
    public LevelTile GetTile(int x ,int y)
    {
        if (x >= sizeX || y >= sizeY || x <0|| y<0)
            return LevelTile.Empty;
        int posX = x % chunkSize;
        int posY = y % chunkSize;
        int chunkPosX = x / chunkSize;
        int chunkPosY = y / chunkSize;
        //Debug.Log(chunkPosX + "," + chunkPosY + "," + posX + "," + posY);
        return chunkGrid[chunkPosX, chunkPosY].tile[posX, posY];
    }
    public bool IsPosOutOfBound(int x, int y)
    {
        return x >= sizeX || y >= sizeY || x < 0 || y < 0;
    }
    public int PosToChunk(int x)
    {
        return  x / chunkSize;
    }
    public int PosToPosInChunk(int x)
    {
        return x % chunkSize;
    }
    public float GetRandom(float x, float y,float offset)
    {
        float r = Mathf.PerlinNoise(x/chunkSize * 20 + randomOffsetX + offset, y/chunkSize * 20 + randomOffsetY + offset);
        r = Mathf.Lerp(r,Mathf.PerlinNoise(x / chunkSize * 8 + randomOffsetX + offset, y / chunkSize * 8 + randomOffsetY + offset),.3f);
        r = Mathf.Lerp(r,Mathf.PerlinNoise(x / chunkSize * 2 + randomOffsetX + offset, y / chunkSize * 2 + randomOffsetY + offset),.1f);
        return (r-.2f)*2;
    }


    public void DestroyWall(Vector2 pos)
    {
        var tile = GetTile((int)pos.x, (int)pos.y);
        if (tile == null)
            return;
        if (tile.data != null)
        {
            var itemData = GetTile((int)pos.x, (int)pos.y).data.dropItem;
            if (itemData != null)
            {
                var pickUp = Instantiate(itemPickUpPrefab, pos + (Vector2)transform.position + Vector2.one * .5f, Quaternion.identity);
                pickUp.item = itemData;
            }
        }

        //tilemap.Erase((int)pos.x, (int)pos.y);
        var tilemap = tileMapList[PosToChunk((int)pos.x), PosToChunk((int)pos.y)];
        tilemap.Erase(PosToPosInChunk((int)pos.x), PosToPosInChunk((int)pos.y));
        tilemap.UpdateMeshImmediate();
        tilemap.Refresh();
    }
    */
}
