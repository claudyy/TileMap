using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
public class TileMapEditorUtility {

  public  void CreateTextureFromSaveFile(TileMapSaveData st, SaveUtility saveUtility, LevelTilemap target) {
        Texture2D tex = new Texture2D(st.sizeX, st.sizeY);
        var datas = target.GetTileLevelDataFromSaveFile(st);
        for (int x = 0; x < st.sizeX; x++) {
            for (int y = 0; y < st.sizeY; y++) {
                tex.SetPixel(x, y, datas[x + y * st.sizeX].editorColor);
            }
        }
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        //UnityEditor.AssetDatabase.CreateAsset(tex, "Assets/"+saveUtility.fileName+".asset");

        Object.DestroyImmediate(tex);

        //var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(saveUtility.path + saveUtility.fileName + ".png");

        //asset.filterMode = FilterMode.Point;
        File.WriteAllBytes(saveUtility.GetPath("png"), bytes);
        UnityEditor.AssetDatabase.Refresh();

    }
    public TileMapSaveData CreateSaveDataFromTexutre(Texture2D loadTexture) {
        var allData = Resources.LoadAll("", typeof(TileLevelData)).Cast<TileLevelData>().ToArray();
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
    public TileSaveData GetDataFromColor(Color c, TileLevelData[] allData) {
        TileLevelData data = null;
        for (int i = 0; i < allData.Length; i++) {
            if (allData[i].editorColor == c) {
                data = allData[i];
                break;
            }
        }
        return new TileSaveData(data);
    }
}
