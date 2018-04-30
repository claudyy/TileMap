using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//[CustomPropertyDrawer(typeof(SaveUtility))]
public class SaveUtility_PropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        base.OnGUI(position, property, label);
        var targetObject = property.serializedObject.targetObject;
        var targetObjectClassType = targetObject.GetType();
        var pathfield = targetObjectClassType.GetField("path");
        var fileNamefield = targetObjectClassType.GetField("fileName");
        string path = pathfield.GetValue(targetObject) as string;
        string fileName = fileNamefield.GetValue(targetObject) as string;
        if (GUILayout.Button("Select"))
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path + fileName + ".xml");
    }
}
