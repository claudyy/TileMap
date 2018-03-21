using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(BaseLevelGenerator),editorForChildClasses:true)]
public class BaseLevelGenerator_Editor : Editor {
    public EditorCoroutine ec;
    public Coroutine c;
    public BaseLevelGenerator generator;
    public override void OnInspectorGUI() {
        if(generator == null)
            generator = (BaseLevelGenerator)target;
         
        DrawDefaultInspector();

        if (GUILayout.Button("Generate") && ec == null) {
            //ec = EditorCoroutine.StartCoroutine(generator.GenerateMap());
            //EditorCoroutines.EditorCoroutines.StartCoroutine(generator.GenerateMap(), generator);
            c = generator.StartCoroutine(generator.GenerateMap());
        }

        if (ec!= null && ec.IsDone == true) {
            //generator.level.UpdatePrefabFromData();
            //generator.level.GenerateGridFromPrefab();
            ec = null;
        }
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), generator.GetProgress(),"");

        if (c != null) {
            //if (EditorUtility.DisplayCancelableProgressBar("Generate", "", .5f)) {
            //    generator.StopCoroutine(c);
            //}
        } else {
            //EditorUtility.ClearProgressBar();
        }
    
        EditorCoroutine.ShowCoroutineProgressBar("test", ec,generator);
    }
}
