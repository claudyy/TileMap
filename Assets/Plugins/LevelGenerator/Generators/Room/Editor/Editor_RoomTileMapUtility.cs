using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RoomTileMapUtility))]
public class Editor_RoomTileMapUtility : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var utility = target as RoomTileMapUtility;

        if (GUILayout.Button("Load Room"))
            utility.LoadRoom();
        if (GUILayout.Button("Apply To Room"))
            utility.ApplyToRoom();
        if (GUILayout.Button("Create Room"))
            utility.CreateRoom();
    }
}
