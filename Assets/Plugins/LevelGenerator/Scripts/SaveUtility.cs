using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveUtility {
    public string path;
    public string fileName;
    public string GetPath() {
        return Application.dataPath + "/" + path + fileName + ".xml";
    }
    public string GetPath(string fileType,bool withApplicationPath = true) {
        var fileName = this.fileName;
        if (withApplicationPath == false)
            return path + fileName + "." + fileType;
        return Application.dataPath + "/" + path + fileName + "."+ fileType;
    }
    public string GetAssetPath(string fileType) {
#if UNITY_EDITOR
        var path = this.path + fileName + "." + fileType;
        return UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
#endif
        return "";
    }
}
