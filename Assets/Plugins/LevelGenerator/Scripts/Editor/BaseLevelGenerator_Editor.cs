using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(BaseLevelGenerator),editorForChildClasses:true)]
public class BaseLevelGenerator_Editor : Editor {
    
    public override void OnInspectorGUI() {
        var generator = (BaseLevelGenerator)target;
         
        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            generator.GenerateMap();
        }
    }
    
}
