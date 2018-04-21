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
    public string GetPath(string fileType) {
        return Application.dataPath + "/" + path + fileName + "."+ fileType;
    }
}
