using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TilemapTextureConverter))]
public class TilemapTextureConverter_Editor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var tilemap = target as TilemapTextureConverter;
        if (GUILayout.Button("Load Texture"))
            tilemap.LoadTexture();
    }
}
