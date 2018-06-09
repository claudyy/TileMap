using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelTilemap))]
public abstract class BaseTilemapLookUpTexture : MonoBehaviour {
    public int tileSize;
    public int chunkSize;
    public int levelChunkX;
    public int levelChunkY;
    public RenderTexture[] chunksRT;
    public RenderTexture[] allChunksRT;
    public RenderTexture chunkBufferRT;
    public abstract string RenderTextureName();
    protected bool loaded;

    protected LevelTilemap levelTilemap;
    
    protected virtual void Awake() {
        levelTilemap = GetComponent<LevelTilemap>();
        levelTilemap.onLoaded += Onloaded;
        levelTilemap.onLoadChunkNewMidChunk += LoadNewMidChunk;
        levelTilemap.onInitTile += InitTile;
        levelTilemap.onLoadTileFirstTime += LoadTileFirstTime;



    }
    protected virtual void Update() {

    }
    public virtual void InitTile(LevelTilemap levelTilemap,BaseLevelTile tile) {

    }
    public virtual void LoadTileFirstTime(LevelTilemap levelTilemap, BaseLevelTile tile) {

    }
    public virtual void Onloaded(LevelTilemap levelTilemap) {
        chunkBufferRT = CreateRenderTexture(levelTilemap,"buffer");
        chunksRT = new RenderTexture[9];
        for (int i = 0; i < chunksRT.Length; i++) {
            chunksRT[i] = null;
        }
        allChunksRT = new RenderTexture[levelTilemap.levelChunkX * levelTilemap.levelChunkY];
        for (int x = 0; x < levelTilemap.levelChunkX; x++) {
            for (int y = 0; y < levelTilemap.levelChunkY; y++) {
                allChunksRT[x + y * levelTilemap.levelChunkX] = CreateRenderTexture(levelTilemap,""+x+"_"+y);
            }
        }
        loaded = true;
        chunkSize = levelTilemap.chunkSize;
        levelChunkX = levelTilemap.levelChunkX;
        levelChunkY = levelTilemap.levelChunkY;
        Shader.SetGlobalFloat("_ChunkSize", chunkSize);

    }
    public virtual void LoadNewMidChunk(LevelTilemap levelTilemap,Vector2Int chunckPos,Vector3Int midChunk) {
        Vector3Int vec3IntTemp = Vector3Int.zero;
        vec3IntTemp.x = chunckPos.x - midChunk.x;//dir
        vec3IntTemp.y = chunckPos.y - midChunk.y;
        vec3IntTemp.z = 0;
        SetRenderTexture(vec3IntTemp.x, vec3IntTemp.y, GetRenderAllTexutre(chunckPos.x, chunckPos.y));
    }
    RenderTexture CreateRenderTexture(LevelTilemap levelTilemap,string name) {
        var r = new RenderTexture(levelTilemap.chunkSize* tileSize, levelTilemap.chunkSize* tileSize, 0);
        r.filterMode = FilterMode.Trilinear;
        r.wrapMode = TextureWrapMode.Clamp;
        r.name = name;
        return r;
    }
    protected bool RenderTextureIndexInOfRange(int x, int y) {
        if (x < -1 || x > 1)
            return false;
        if (y < -1 || y > 1)
            return false;
        return true;
    }
    protected bool RenderTextureIndexOutOfLevel(int x, int y) {
        if (x < 0 || x > levelChunkX)
            return true;
        if (y < 0 || y > levelChunkY)
            return true;
        return false;
    }
    protected RenderTexture GetRenderTexutre(int x, int y) {
        if (RenderTextureIndexInOfRange(x, y) == false)
            return null;

        return chunksRT[1 + x + (1 + y) * 3];
    }
    void SetRenderTexture(int x, int y, RenderTexture rt) {
        if (RenderTextureIndexInOfRange(x, y) == false)
            return;
        
        chunksRT[1 + x + (1 + y) * 3] = rt;
        Shader.SetGlobalTexture(RenderTextureName()+"CenterTex", GetRenderTexutre(0, 0));
        Shader.SetGlobalTexture(RenderTextureName()+"LeftTex", GetRenderTexutre(-1, 0));
        Shader.SetGlobalTexture(RenderTextureName()+"TopTex", GetRenderTexutre(0, 1));
        Shader.SetGlobalTexture(RenderTextureName()+"RightTex", GetRenderTexutre(1, 0));
        Shader.SetGlobalTexture(RenderTextureName()+"BottomTex", GetRenderTexutre(0, -1));
        Shader.SetGlobalTexture(RenderTextureName()+"TopLeftTex", GetRenderTexutre(-1, 1));
        Shader.SetGlobalTexture(RenderTextureName()+"TopRightTex", GetRenderTexutre(1, 1));
        Shader.SetGlobalTexture(RenderTextureName()+"BottomRightTex", GetRenderTexutre(1, -1));
        Shader.SetGlobalTexture(RenderTextureName()+"BottomLeftTex", GetRenderTexutre(-1, -1));

    }
    protected RenderTexture GetRenderAllTexutre(int x, int y) {
        return allChunksRT[x + y * levelChunkX];
    }

}
