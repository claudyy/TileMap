using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class RoomTileMapUtility : MonoBehaviour {
    public SaveUtility saveUtility;
    public Tilemap tilemap;
    public LevelTilemap level;
    public Structure_BaseRoomData room;
    public TileLevelData defaultData;
    public TileLevelData roomWall;
    public TileLevelData roomFloor;
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
        room.CreateRoomFromSaveFill(saveFile);
        room.floor = roomFloor;
        room.wall = roomWall;
        room.UpdateDoors();
    }

    public void CreateRoom() {
        var saveFile = level.GetSaveDataFromTileMap();
        CreateRoom(saveFile);
    }
    public void CreateRoom(TileMapSaveData saveFile) {
#if UNITY_EDITOR
        var room = new BaseFixedRoomData();
        room.name = saveUtility.fileName;
        room.CreateRoomFromSaveFill(saveFile);
        UnityEditor.AssetDatabase.CreateAsset(room, saveUtility.path);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    public void CreateRoomFromTextureList() {
    }
}
