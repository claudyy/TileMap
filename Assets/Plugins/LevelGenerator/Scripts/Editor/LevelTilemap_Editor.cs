﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
[CustomEditor(typeof(LevelTilemap))]
public class LevelTilemap_Editor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var tilemap = target as LevelTilemap;
        if (GUILayout.Button("Load"))
            tilemap.Load();
        if (GUILayout.Button("Save"))
            tilemap.Save();
        if (GUILayout.Button("Clear Prefab"))
            tilemap.grid.GetComponentInChildren<Tilemap>().ClearAllTiles();
    }
}