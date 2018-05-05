using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTileMapUtility : MonoBehaviour {
#if UNITY_EDITOR
    public SaveUtility saveUtility;
    public LevelTilemap level;
    public Structure_BaseRoomData room;
    public LevelTileData defaultData;
    public LevelTileData roomWall;
    public LevelTileData roomFloor;
    public List<Texture2D> textureList;

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

        var room = new BaseFixedRoomData();
        room.name = saveUtility.fileName;
        ApplyToRoom(room, saveFile);
        UnityEditor.AssetDatabase.CreateAsset(room, saveUtility.GetAssetPath("asset"));
        //UnityEditor.AssetDatabase.SaveAssets();

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
#endif
}
