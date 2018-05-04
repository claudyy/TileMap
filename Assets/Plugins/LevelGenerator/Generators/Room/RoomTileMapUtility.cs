using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
public class RoomTileMapUtility : MonoBehaviour {
    public SaveUtility saveUtility;
    public LevelTilemap level;
    public Structure_BaseRoomData room;
    public TileLevelData defaultData;
    public TileLevelData roomWall;
    public TileLevelData roomFloor;
    public List<Texture2D> textureList;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadRoom() {
        GeneratorMapData mapdata = new GeneratorMapData(room.sizeX,room.sizeY, defaultData);
        var structure = room.GetStructure();
        structure.posX = 0;
        structure.posY = 0;
        structure.Generate(mapdata);
        level.ApplySaveFileToMap(mapdata.GetNewSaveFile());
    }

    public void ApplyToRoom() {
        var saveFile = level.GetSaveDataFromTileMap();
        ApplyToRoom(room, saveFile);
    }

    public void CreateRoom() {
        var saveFile = level.GetSaveDataFromTileMap();
        CreateRoom(saveFile);
    }
    public void CreateRoom(TileMapSaveData saveFile) {
#if UNITY_EDITOR
        var room = new BaseFixedRoomData();
        room.name = saveUtility.fileName;
        ApplyToRoom(room, saveFile);
        UnityEditor.AssetDatabase.CreateAsset(room, saveUtility.GetAssetPath("asset"));
        //UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    public void CreateRoomFromTextureList() {
        TileMapEditorUtility utility = new TileMapEditorUtility();
        for (int i = 0; i < textureList.Count; i++) {
            CreateRoom(utility.CreateSaveDataFromTexutre(textureList[i]));
        }
    }

    void ApplyToRoom(Structure_BaseRoomData room, TileMapSaveData saveFile) {
        room.CreateRoomFromSaveFill(saveFile);
        room.floor = roomFloor;
        room.wall = roomWall;
        room.UpdateDoors();

    }
}
