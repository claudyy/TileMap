using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClaudeFehlen.Tileset;
using System.Linq;
public class GenerateUtility {
    LevelTileData[] allData;
    public GenerateUtility() {
        allData = Resources.LoadAll("", typeof(LevelTileData)).Cast<LevelTileData>().ToArray();

    }
    public float ReadTexture(Texture2D tex, float x, float y)
    {
        int posX = (int)(x * tex.width);
        int posY = (int)(y * tex.height);
        posX %= tex.width;
        posY %= tex.height;
        return tex.GetPixel(posX, posY).r;
    }
    public LevelTileData GetDataFromName(string name) {
        if (name == "")
            return null;
        for (int i = 0; i < allData.Length; i++) {
            if (allData[i].name == name)
                return allData[i];
        }
        return null;
    }

}
