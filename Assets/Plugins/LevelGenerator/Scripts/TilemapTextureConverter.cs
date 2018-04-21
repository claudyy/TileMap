using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TilemapTextureConverter : MonoBehaviour {
    public Texture2D loadTexture;
    public LevelTilemap target;
	public void LoadTexture() {
        var allData = Resources.LoadAll("", typeof(TileLevelData)).Cast<TileLevelData>().ToArray();
        TileMapSaveData st = new TileMapSaveData();
        st.sizeX = loadTexture.width;
        st.sizeY = loadTexture.height;
        TileSaveData[] datas = new TileSaveData[st.sizeX * st.sizeY];
        for (int x = 0; x < loadTexture.width; x++) {
            for (int y = 0; y < loadTexture.height; y++) {
                datas[x + y * loadTexture.height] = GetDataFromColor(loadTexture.GetPixel(x,y),allData);

            }
        }
        st.tiles = datas;
        target.ApplySaveFileToMap(st);
    }
    TileSaveData GetDataFromColor(Color c,TileLevelData[] allData) {
        TileLevelData data = null;
        for (int i = 0; i < allData.Length; i++) {
            if(allData[i].editorColor == c) {
                data = allData[i];
                break;
            }
        }
        return new TileSaveData(data);
    }
    public void CreateTexture() {

    }
}
