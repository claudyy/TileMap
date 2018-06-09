using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapLookUpTexture_Fluid : BaseTilemapLookUpTexture {
    public Material fluidSim;
    public Material collisionDrawer;
    public float updateTime;
    float curUpdateTime;
    public List<Transform> emitterList;
    protected override void Awake() {
        base.Awake();
        collisionDrawer = new Material(Shader.Find("Hidden/TileMapFluidCollisionDrawer"));
        
    }
    public override void Onloaded(LevelTilemap levelTilemap) {
        base.Onloaded(levelTilemap);
        Shader.SetGlobalFloat("_FluidPixelPerChunk", tileSize * chunkSize);
        Shader.SetGlobalFloat("_FluidPixelPerTile", tileSize);
    }
    protected override void Update() {
        base.Update();
        if (loaded == false)
            return;
        curUpdateTime += Time.deltaTime;
        if (curUpdateTime >= updateTime) {
           

            //Emitters
            if (emitterList.Count > 0) {
                Shader.SetGlobalInt("_EmitterCount", emitterList.Count);
                var emittersPos = new Vector4[emitterList.Count];
                var emittersSize = new float[emitterList.Count];
                var emittersStrength = new float[emitterList.Count];
                for (int i = 0; i < emitterList.Count; i++) {
                    if(emitterList[i] == null) {

                        continue;
                    }
                    emittersPos[i] =new Vector4(emitterList[i].position.x, emitterList[i].position.y, emitterList[i].up.x, emitterList[i].up.y);
                    emittersSize[i] = (emitterList[i].localScale.x);
                    emittersStrength[i] = (emitterList[i].localScale.y);
                }
                Shader.SetGlobalVectorArray("_EmittersPos", emittersPos);
                Shader.SetGlobalFloatArray("_EmittersSize", emittersSize);
                Shader.SetGlobalFloatArray("_EmittersStrength", emittersStrength);
            }

            Sim();
            curUpdateTime = 0;
        }
    }

    public override void LoadTileFirstTime(LevelTilemap levelTilemap, BaseLevelTile tile) {
        base.LoadTileFirstTime(levelTilemap, tile);
        var chunkPos = levelTilemap.PosToChunk(tile.x, tile.y);
        var inChunkPos = levelTilemap.PosToPosInChunk(tile.x, tile.y);
        var data = tile.data;
        var fluidData = data as IFluidData;
        collisionDrawer.SetInt("_CollisionTilePosX", inChunkPos.x);
        collisionDrawer.SetInt("_CollisionTilePosY", inChunkPos.y);
        collisionDrawer.SetTexture("_CollisionTex", fluidData.Collision());
        var targetRT = GetRenderAllTexutre(chunkPos.x, chunkPos.y);
        Graphics.Blit(targetRT, chunkBufferRT, collisionDrawer);
        Graphics.Blit(chunkBufferRT, targetRT);
    }
    void Sim() {
        Vector3Int midChunk = levelTilemap.PosToChunk((int)Camera.main.transform.position.x, (int)Camera.main.transform.position.y);
        Shader.SetGlobalVector("_ChunckCenterPos", new Vector4(midChunk.x, midChunk.y,0,0));
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (RenderTextureIndexOutOfLevel(midChunk.x + x, midChunk.y + y))
                    continue;
                fluidSim.SetInt("_ChunkX",midChunk.x + x);
                fluidSim.SetInt("_ChunkY", midChunk.y +  y);
                fluidSim.SetInt("_ChunkDirX", x);
                fluidSim.SetInt("_ChunkDirY",  y);
                Graphics.Blit(GetRenderTexutre(x,y), chunkBufferRT, fluidSim);
                Graphics.Blit(chunkBufferRT, GetRenderTexutre(x, y));
            }
        }

    }
    public override string RenderTextureName() {
        return "_Fluid";
    }
}
