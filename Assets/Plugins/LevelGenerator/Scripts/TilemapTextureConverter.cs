using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class TilemapTextureConverter : MonoBehaviour {
    public SaveUtility saveUtility;
    public Texture2D loadTexture;
    public LevelTilemap target;
	public void LoadTexture() {
        target.ApplySaveFileToMap(CreateSaveData());
    }
    public void CreateSaveFile() {

        target.CreateSaveFile(CreateSaveData(), saveUtility);
    }
    TileMapSaveData CreateSaveData() {
        var allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();
        TileMapSaveData st = new TileMapSaveData();
        st.sizeX = loadTexture.width;
        st.sizeY = loadTexture.height;
        TileSaveData[] datas = new TileSaveData[st.sizeX * st.sizeY];
        for (int x = 0; x < loadTexture.width; x++) {
            for (int y = 0; y < loadTexture.height; y++) {
                datas[x + y * loadTexture.width] = GetDataFromColor(loadTexture.GetPixel(x, y), allData);

            }
        }
        st.tiles = datas;
        return st;
    }
    TileSaveData GetDataFromColor(Color c,LevelTileData[] allData) {
        LevelTileData data = null;
        for (int i = 0; i < allData.Length; i++) {
            if(allData[i].editorColor == c) {
                data = allData[i];
                break;
            }
        }
        return new TileSaveData(data);
    }
    public void CreateTexture() {
       var st = target.GetSaveDataFromTileMap();
        CreateTextureFromSaveFile(st);
    }
    void CreateTextureFromSaveFile(TileMapSaveData st) {
        Texture2D tex = new Texture2D(st.sizeX, st.sizeY);
        var datas = target.GetTileLevelDataFromSaveFile(st);
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tex.SetPixel(x, y, datas[x + y * st.sizeX].editorColor);
            }
        }
        #if UNITY_EDITOR
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        //UnityEditor.AssetDatabase.CreateAsset(tex, "Assets/"+saveUtility.fileName+".asset");

        Object.DestroyImmediate(tex);

        //var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(saveUtility.path + saveUtility.fileName + ".png");

        //asset.filterMode = FilterMode.Point;
        File.WriteAllBytes(saveUtility.GetPath("png"), bytes);
        UnityEditor.AssetDatabase.Refresh();
        #endif

    }
}
