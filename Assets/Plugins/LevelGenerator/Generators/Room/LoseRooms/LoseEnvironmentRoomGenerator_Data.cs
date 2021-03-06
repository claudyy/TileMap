﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoseEnvironmentRoomGenerator : BaseLevelStructure {
    LoseEnvironmentRoomGenerator_Data loseRoomData;
    List<BaseRoom> rooms;
    List<BaseRoom> notConnectedRooms;
    List<Vector2> startDoor = new List<Vector2>();
    List<Vector2> endDoor = new List<Vector2>();
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
        loseRoomData = data as LoseEnvironmentRoomGenerator_Data;
    }
    public override IEnumerator RunTimeTryGenerate(BaseLevelGenerator generator, MonoBehaviour mono) {
        notConnectedRooms = new List<BaseRoom>();
        //place rooms
        for (int i = 0; i < loseRoomData.roomCount; i++) {
            int x = Random.Range(0, loseRoomData.roomCount * loseRoomData.averageRoomSize.x);
            int y = Random.Range(0, loseRoomData.roomCount * loseRoomData.averageRoomSize.y);
            var room = loseRoomData.GetRoom().GetStructure() as BaseRoom;
            room.posX = x;
            room.posY = y;
            notConnectedRooms.Add(room);
        }
        //recalcuate Positions
        for (int i = 0; i < 80; i++) {
            MoveStructureAway(notConnectedRooms);
        }
        //remove some rooms
        for (int i = notConnectedRooms.Count - 1; i >= 0; i--) {
            var otherStructure = GetRoomInBounds(notConnectedRooms, notConnectedRooms[i]);
            if (otherStructure.Count != 0 && Random.value > .5f) {
                //notConnectedRooms.Remove(notConnectedRooms[i]);
            }
        }
        //recalculate position again
        for (int i = 0; i < 40; i++) {
            MoveStructureAway(notConnectedRooms);
        }
        for (int i = 0; i < 44; i++) {
            MoveTogehter(notConnectedRooms);
        }
        //remove some rooms
        for (int i = notConnectedRooms.Count - 1; i >= 0; i--) {
            var otherStructure = GetRoomInBounds(notConnectedRooms, notConnectedRooms[i]);
            if (otherStructure.Count != 0) {
                //notConnectedRooms.Remove(notConnectedRooms[i]);
            }
        }
        //connect rooms to start room
        for (int i = 0; i < notConnectedRooms.Count; i++) {
            //ConnectRoomWithOther(notConnectedRooms, notConnectedRooms[i]);
        }
        //remove not connected Rooms
        rooms = notConnectedRooms;
        for (int i = 0; i < rooms.Count; i++) {
            rooms[i].TryGenerate(generator);
        }
        yield return null;
    }

    public override void TryGenerate(BaseLevelGenerator generator) {
        base.TryGenerate(generator);
        notConnectedRooms = new List<BaseRoom>();
        //place rooms
        for (int i = 0; i < loseRoomData.roomCount; i++) {
            int x = 0;
            int y = 0;
            var room = loseRoomData.GetRoom().GetStructure() as BaseRoom;
            if (notConnectedRooms.Count != 0) {
                int index = Random.Range(0, notConnectedRooms.Count);
                int xDir = (int)Mathf.Sign(Random.Range(-1, 1));
                int yDir = (int)Mathf.Sign(Random.Range(-1, 1));
                if (Random.value > .5)
                    xDir = 0;
                else
                    yDir = 0;
                x = notConnectedRooms[index].centerX + (notConnectedRooms[index].sizeX / 2 + room.sizeX / 2 - room.centerX) * xDir;
                y = notConnectedRooms[index].centerY + (notConnectedRooms[index].sizeY / 2 + room.sizeY / 2 - room.centerY) * yDir;
            }

            room.posX = x;
            room.posY = y;
            notConnectedRooms.Add(room);
        }
        //recalcuate Positions
       //for (int i = 0; i < 80; i++) {
       //    MoveStructureAway(notConnectedRooms);
       //}
       //
       ////recalculate position again
       //for (int i = 0; i < 40; i++) {
       //    MoveStructureAway(notConnectedRooms);
       //}
       for (int i = 0; i < 4; i++) {
           MoveTogehter(notConnectedRooms);
       }

        rooms = notConnectedRooms;
        for (int i = 0; i < rooms.Count; i++) {
            rooms[i].TryGenerate(generator);
        }
    }
    public void MoveStructureAway(List <BaseRoom> structures) {
        var otherStructure = new List<BaseRoom>();
        Vector2 dir = Vector2.zero;
        Vector2 pos = Vector2.zero;
        Vector2 otherPos = Vector2.zero;
        for (int i = 0; i < structures.Count; i++) {
            var room = structures[i];
            otherStructure = GetRoomInBounds(structures, room);
            dir = Vector2.zero;
            
            if(otherStructure.Count !=0) {
                for (int odir = 0; odir < otherStructure.Count; odir++) {
                    pos.x = structures[i].centerX;
                    pos.y = structures[i].centerY;
                    otherPos.x = otherStructure[odir].centerX;
                    otherPos.y = otherStructure[odir].centerY;
                    dir += (pos - otherPos).normalized;

                }
                dir /= otherStructure.Count;
            }
            else {
                for (int o = 0; o < structures.Count; o++) {
                    pos.x = structures[i].centerX;
                    pos.y = structures[i].centerY;
                    otherPos.x = structures[o].centerX;
                    otherPos.y = structures[o].centerY;
                    dir += (otherPos - pos).normalized;
                }
                dir /= structures.Count;

            }

            structures[i].Move(Mathf.RoundToInt(dir.x),Mathf.RoundToInt(dir.y));
            
        }
    }
    void MoveTogehter(List<BaseRoom> structures) {
        Vector2 dir = Vector2.zero;
        Vector2 pos = Vector2.zero;
        Vector2 otherPos = Vector2.zero;
        for (int i = 0; i < structures.Count; i++) {
            var room = structures[i];
            dir = Vector2.zero;
         
            for (int o = 0; o < structures.Count; o++) {
                pos.x = structures[i].centerX;
                pos.y = structures[i].centerY;
                otherPos.x = structures[o].centerX;
                otherPos.y = structures[o].centerY;
                dir += (otherPos - pos).normalized;
            }
            dir /= structures.Count;

            structures[i].Move(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));

        }
    }
    List<BaseRoom> GetRoomInBounds(List<BaseRoom> rooms,BaseRoom room) {
        var inBoundsRoom = new List<BaseRoom>();
        var bound =new Bounds();
        bound.size = room.GetBounds()[0].size;
        bound.center = room.GetBounds()[0].center;
        //bound.Expand(-1f);
        for (int o = 0; o < rooms.Count; o++) {
            if (bound.Intersects(rooms[o].GetBounds()[0]))
                inBoundsRoom.Add(rooms[o]);
        }
        inBoundsRoom.Remove(room);
        return inBoundsRoom;
    }
    void ConnectRoomWithOther(List<BaseRoom> rooms, BaseRoom room) {
        var topDoors = room.roomData.GetTopPossibleDoorsPos();
        var roomPos = new Vector2(room.posX, room.posY);
        Vector2 door = Vector2.zero;
        List<BaseRoom> intersectRooms = new List<BaseRoom>();
        rooms.Remove(room);
        for (int i = 0; i < topDoors.Count; i++) {
            for (int r = 0; r < rooms.Count; r++) {
                Ray ray = new Ray(topDoors[i] + roomPos, Vector2.up);
                if (rooms[r].IntersectBound(ray)) {
                    door = topDoors[i];
                    intersectRooms.Add(rooms[r]);
                }
            }
            if (door != Vector2.zero)
                break;
        }
        if (intersectRooms.Count == 0)
            return;
        float distance = float.MaxValue;
        float distanceTemp;
        int resultIndex =-1;
        for (int i = 0; i < intersectRooms.Count; i++) {
            distanceTemp = Vector2Int.Distance(room.center, intersectRooms[i].center);
            if (distanceTemp < distance) {
                distance = distanceTemp;
                resultIndex = i;
            }
        }
        if (resultIndex == -1)
            return;
        var otherRoom = intersectRooms[resultIndex];
        var otherRoomPos = new Vector2(otherRoom.posX, otherRoom.posY);
        var otherdoorPos = new Vector2(roomPos.x + door.x - otherRoomPos.x, 0 );

        startDoor.Add(roomPos + door+Vector2.up*room.sizeY);
        endDoor.Add(otherRoomPos + otherdoorPos);

    }
    public override List<Bounds> GetBounds() {
        var bounds = new List<Bounds>();
        for (int i = 0; i < rooms.Count; i++) {
            bounds.AddRange(rooms[i].GetBounds());
        }
        return bounds;
    }
    public override void Generate(GeneratorMapData map) {
        base.Generate(map);
        for (int i = 0; i < rooms.Count; i++) {
            var room = rooms[i];
            int startX = room.posX;
            int startY = room.posY;
            for (int x = 0; x < room.roomData.sizeX; x++) {
                for (int y = 0; y < room.roomData.sizeY; y++) {
                    if (IgnoreTile(room.roomData.roomTileList[x + y * room.roomData.sizeX]) == false)
                        room.OverrideTile(map, x, y, startX, startY);
                }
            }
        }

    }
    public override IEnumerator RunTimeGenerate(GeneratorMapData map, MonoBehaviour mono) {
        for (int i = 0; i < rooms.Count; i++) {
            var room = rooms[i];
            int startX = room.posX;
            int startY = room.posY;
            for (int x = 0; x < room.roomData.sizeX; x++) {
                for (int y = 0; y < room.roomData.sizeY; y++) {
                    if (IgnoreTile(room.roomData.roomTileList[x + y * room.roomData.sizeX]) == false)
                        room.OverrideTile(map, x, y, startX, startY);
                }
            }
            yield return null;
        }
    }
    bool IgnoreTile(LevelTileData data) {
        for (int i = 0; i < loseRoomData.ignoreTileList.Count; i++) {
            if (data == loseRoomData.ignoreTileList[i])
                return true;
        }
        return false;
    }
    public override void Move(int x, int y) {
        base.Move(x, y);
        for (int i = 0; i < rooms.Count; i++) {
            rooms[i].Move(x, y);
        }
    }
}
public class LoseEnvironmentRoomGenerator_Data : BaseLevelStructureData {
    public int roomCount;
    public int needRoomCount;
    public Vector2Int averageRoomSize;
    public List<Structure_BaseRoomData> roomsData;
    public LevelTileData floor;
    public List<LevelTileData> ignoreTileList;
    public override BaseLevelStructure GetStructure() {
        var s = new LoseEnvironmentRoomGenerator();
        s.Init(this);
        return s;
    }
    public Structure_BaseRoomData GetRoom() {
        return roomsData[Random.Range(0, roomsData.Count)];
    }
}
