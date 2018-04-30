using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoseRoomGenerator : BaseLevelStructure {
    LoseRoomGenerator_Data loseRoomData;
    List<BaseRoom> rooms;
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
        loseRoomData = data as LoseRoomGenerator_Data;
    }
    public override void TryGenerate(BaseLevelGenerator generator) {
        base.TryGenerate(generator);
        //place rooms
        //recalcuatePos
        //Room
    }
    public override List<Bounds> GetBounds() {
        return base.GetBounds();
    }
    public override void Generate(GeneratorMapData map) {
        base.Generate(map);
        for (int i = 0; i < rooms.Count; i++) {
            rooms[i].Generate(map);
        }
    }
}
public class LoseRoomGenerator_Data : BaseLevelStructureData {
    public int roomCount;
    public List<Structure_BaseRoomData> roomsData;
    public override BaseLevelStructure GetStructure() {
        var s = new LoseRoomGenerator();
        s.Init(this);
        return s;
    }
    public Structure_BaseRoomData GetRoom() {
        return roomsData[Random.Range(0, roomsData.Count)];
    }
}
