using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TilemapTextureConverter))]
public class TilemapTextureConverter_Editor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var tilemap = target as TilemapTextureConverter;
        if (GUILayout.Button("Apply Texture"))
            tilemap.LoadTexture();
        if (GUILayout.Button("Create Save File Texture"))
            tilemap.CreateSaveFile();
        if (GUILayout.Button("Create Texture From TileMap"))
            tilemap.CreateTexture();
    }
}
