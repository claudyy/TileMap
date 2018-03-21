using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateUtility {

    public float ReadTexture(Texture2D tex, float x, float y)
    {
        int posX = (int)(x * tex.width);
        int posY = (int)(y * tex.height);
        posX %= tex.width;
        posY %= tex.height;
        return tex.GetPixel(posX, posY).r;
    }
}
