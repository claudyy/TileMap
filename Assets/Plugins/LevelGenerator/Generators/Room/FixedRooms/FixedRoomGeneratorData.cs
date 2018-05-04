using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FixedRoomGenerator : BaseLevelStructure {

    public BaseFixedRoom[] rooms;
    public List<BaseFixedRoom> freeRooms;
    FixedRoomGeneratorData gData;
    Vector2Int center {
        get {
            return new Vector2Int(gData.levelSizeX / 2, gData.levelSizeY / 2);
        }
    }
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
        gData = data as FixedRoomGeneratorData;
        gData.UpdateRoomData();

    }
    public override void Generate(GeneratorMapData map) {
        base.Generate(map);
        for (int i = 0; i < structureList.Count; i++) {
            structureList[i].Generate(map);
        }
    }
    public override IEnumerator RunTimeGenerate(GeneratorMapData map,MonoBehaviour mo) {
        for (int i = 0; i < structureList.Count; i++) {
            yield return mo.StartCoroutine(structureList[i].RunTimeGenerate(map, mo));
        }
        yield return null;
    }
    List<BaseLevelStructure> structureList = new List<BaseLevelStructure>();
    public override IEnumerator RunTimeTryGenerate(BaseLevelGenerator generator, MonoBehaviour mono) {
        structureList = new List<BaseLevelStructure>();
        int levelSizeX = gData.levelSizeX;
        int levelSizeY = gData.levelSizeY;
        rooms = new BaseFixedRoom[levelSizeX * levelSizeY];
        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                rooms[x + y * levelSizeX] = BaseFixedRoom.EmptyRoom(x, y);
        freeRooms = new List<BaseFixedRoom>();

        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                freeRooms.Add(BaseFixedRoom.EmptyRoom(x, y));
        yield return null;


        //placeExit
        //DebugCheckTimeStart("Place Exit");
        var exit = GetRandomRoomFromList(gData.roomExitDataList).GetStructure() as FixedRoom_Exit;
        var exitPos = GetRandomRoomPos();
        SetFixedRoom(exit, exitPos.x, exitPos.y);
        if (exitPos == Vector2Int.one * -1 || exit == null) {
            //DebugTryFail(exit == null ? "Couldnt find Exit Room" : "Could find Position for exit");
            //return false;
        }
        yield return null;
       // DebugCheckTimeEnd();

        //ChestRoom
        //DebugCheckTimeStart("Place Chest");

        var chest = GetRandomRoomFromList(gData.roomChestDataList).GetStructure() as FixedRoom_Chest;
        var chestPos = GetRandomFreeRoomPos((int)new Vector2(levelSizeX, levelSizeY).magnitude / 3, exit);
        if (chestPos == Vector2Int.one * -1 || chest == null) {
            //DebugTryFail(chest == null ? "Couldnt find Chest Room" : "Could find Position for chest");
            //return false;
        }
        SetFixedRoom(chest, chestPos.x, chestPos.y);
        yield return null;
        //DebugCheckTimeEnd();


        //PlaceShop
        //DebugCheckTimeStart("Place Shop");

        var shop = GetRandomRoomFromList(gData.roomShopDataList).GetStructure() as FixedRoom_Shop;
        var shopPos = GetRandomFreeRoomPos((int)new Vector2(levelSizeX, levelSizeY).magnitude / 3, chest, exit);
        if (shopPos == Vector2Int.one * -1 || shop == null) {
            //DebugTryFail(exit == null ? "Couldnt find Shop Room" : "Could find Position for shop");
            //return false;
        }
        SetFixedRoom(shop, shopPos.x, shopPos.y);
        yield return null;
        //DebugCheckTimeEnd();


        //Connect Rooms
        //DebugCheckTimeStart("Connection");

        if (GenerateConnection(chest, exit) == false) {
            //DebugTryFail("Couldnt connect chest and Exit");
            //return false;
        }
        if (GenerateConnection(shop, exit) == false) {
            //DebugTryFail("Couldnt connect shop and Exit");
            //return false;
        }
        yield return null;
        //DebugCheckTimeEnd();


        //FillRest
        //DebugCheckTimeStart("Fill");

        int roomCount = UnityEngine.Random.Range(gData.roomCountRange.x, gData.roomCountRange.y);
        for (int i = 0; i < 50; i++) {
            if (CountOfRooms() >= roomCount)
                break;
            int corridorLength = UnityEngine.Random.Range(gData.additionalCoridorLengthRange.x, gData.additionalCoridorLengthRange.y);
            GenerateCorridor(corridorLength, roomCount);
            yield return null;
        }
        yield return null;
        //DebugCheckTimeEnd();


        //Place Entry
        //DebugCheckTimeStart("Place Entry");

        var entry = GetRandomRoomFromList(gData.roomEntryDataList).GetStructure() as FixedRoom_Entry;
        var entryPos = FindEntryPos(exit, (int)new Vector2(levelSizeX, levelSizeY).magnitude / 3);
        if (entryPos == Vector2Int.one * -1 || entry == null) {
            //DebugTryFail(entry == null ? "Couldnt find Entry Room" : "Could find Position for Entry");
            //return false;
        }
        SetFixedRoom(entry, entryPos.x, entryPos.y);
        yield return null;
        //DebugCheckTimeEnd();

        //GenerateDoor
        for (int x = 0; x < levelSizeX; x++) {
            for (int y = 0; y < levelSizeY; y++) {
                if (GetRoom(x, y).IsEmpty())
                    continue;
                ConnectDoorsWithTop(GetRoom(x, y));
                ConnectDoorsWithBottom(GetRoom(x, y));
                ConnectDoorsWithLeft(GetRoom(x, y));
                ConnectDoorsWithRight(GetRoom(x, y));
                yield return null;
            }
        }

        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                if (GetRoom(x, y) != null && GetRoom(x, y).IsEmpty() == false)
                    structureList.Add(GetRoom(x, y));


        //return true;
    }
    public override void TryGenerate(BaseLevelGenerator generator) {
        structureList = new List<BaseLevelStructure>();
        var levelSizeX = gData.levelSizeX;
        var levelSizeY = gData.levelSizeY;
        rooms = new BaseFixedRoom[levelSizeX * levelSizeY];
        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                rooms[x + y * levelSizeX] = BaseFixedRoom.EmptyRoom(x, y);
        freeRooms = new List<BaseFixedRoom>();

        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                freeRooms.Add(BaseFixedRoom.EmptyRoom(x, y));


        //placeExit
       // DebugCheckTimeStart("Place Exit");
        var exit = GetRandomRoomFromList(gData.roomExitDataList).GetStructure() as FixedRoom_Exit;
        var exitPos = GetRandomRoomPos();
        SetFixedRoom(exit, exitPos.x, exitPos.y);
        if (exitPos == Vector2Int.one * -1 || exit == null) {
            //DebugTryFail(exit == null ? "Couldnt find Exit Room" : "Could find Position for exit");
            //return false;
        }
       // DebugCheckTimeEnd();

        //ChestRoom
       // DebugCheckTimeStart("Place Chest");

        var chest = GetRandomRoomFromList(gData.roomChestDataList).GetStructure() as FixedRoom_Chest;
        var chestPos = GetRandomFreeRoomPos((int)new Vector2(levelSizeX, levelSizeY).magnitude / 3, exit);
        if (chestPos == Vector2Int.one * -1 || chest == null) {
           // DebugTryFail(chest == null ? "Couldnt find Chest Room" : "Could find Position for chest");
            //return false;
        }
        SetFixedRoom(chest, chestPos.x, chestPos.y);
        //DebugCheckTimeEnd();


        //PlaceShop
        //DebugCheckTimeStart("Place Shop");

        var shop = GetRandomRoomFromList(gData.roomShopDataList).GetStructure() as FixedRoom_Shop;
        var shopPos = GetRandomFreeRoomPos((int)new Vector2(levelSizeX, levelSizeY).magnitude / 3, chest, exit);
        if (shopPos == Vector2Int.one * -1 || shop == null) {
            //DebugTryFail(exit == null ? "Couldnt find Shop Room" : "Could find Position for shop");
            //return false;
        }
        SetFixedRoom(shop, shopPos.x, shopPos.y);
        //DebugCheckTimeEnd();


        //Connect Rooms
        //DebugCheckTimeStart("Connection");

        if (GenerateConnection(chest, exit) == false) {
            //DebugTryFail("Couldnt connect chest and Exit");
            //return false;
        }
        if (GenerateConnection(shop, exit) == false) {
            //DebugTryFail("Couldnt connect shop and Exit");
            //return false;
        }
        //DebugCheckTimeEnd();


        //FillRest
        //DebugCheckTimeStart("Fill");

        int roomCount = UnityEngine.Random.Range(gData.roomCountRange.x, gData.roomCountRange.y);
        for (int i = 0; i < 50; i++) {
            if (CountOfRooms() >= roomCount)
                break;
            int corridorLength = UnityEngine.Random.Range(gData.additionalCoridorLengthRange.x, gData.additionalCoridorLengthRange.y);
            GenerateCorridor(corridorLength, roomCount);
        }
        //DebugCheckTimeEnd();


        //Place Entry
       // DebugCheckTimeStart("Place Entry");

        var entry = GetRandomRoomFromList(gData.roomEntryDataList).GetStructure() as FixedRoom_Entry;
        var entryPos = FindEntryPos(exit, (int)new Vector2(levelSizeX, levelSizeY).magnitude / 3);
        if (entryPos == Vector2Int.one * -1 || entry == null) {
           // DebugTryFail(entry == null ? "Couldnt find Entry Room" : "Could find Position for Entry");
            //return false;
        }
        SetFixedRoom(entry, entryPos.x, entryPos.y);
        //DebugCheckTimeEnd();

        //GenerateDoor
        for (int x = 0; x < levelSizeX; x++) {
            for (int y = 0; y < levelSizeY; y++) {
                if (GetRoom(x, y).IsEmpty())
                    continue;
                ConnectDoorsWithTop(GetRoom(x, y));
                ConnectDoorsWithBottom(GetRoom(x, y));
                ConnectDoorsWithLeft(GetRoom(x, y));
                ConnectDoorsWithRight(GetRoom(x, y));
            }
        }

        for (int x = 0; x < levelSizeX; x++)
            for (int y = 0; y < levelSizeY; y++)
                if (GetRoom(x, y) != null && GetRoom(x, y).IsEmpty() == false)
                    structureList.Add(GetRoom(x, y));

    }
    public void ConnectDoorsWithTop(BaseFixedRoom room) {
        var n = GetRoom(room.fixedX, room.fixedY + 1);
        if (n == null || n.IsEmpty())
            return;
        if (room.topDoorCount != 0)
            return;
        ConnectDoorVertical(n, room);
    }
    public void ConnectDoorsWithBottom(BaseFixedRoom room) {
        var n = GetRoom(room.fixedX, room.fixedY - 1);
        if (n == null || n.IsEmpty())
            return;
        if (room.bottomDoorCount != 0)
            return;
        ConnectDoorVertical(room, n);
    }
    public void ConnectDoorsWithLeft(BaseFixedRoom room) {
        var n = GetRoom(room.fixedX - 1, room.fixedY);
        if (n == null || n.IsEmpty())
            return;
        if (room.leftDoorCount != 0)
            return;
        ConnectDoorHorizontal(n, room);
    }
    public void ConnectDoorsWithRight(BaseFixedRoom room) {
        var n = GetRoom(room.fixedX + 1, room.fixedY);
        if (n == null || n.IsEmpty())
            return;
        if (room.rightDoorCount != 0)
            return;
        ConnectDoorHorizontal(room, n);
    }
    public void ConnectDoorVertical(BaseFixedRoom top, BaseFixedRoom bottom) {
        var bottoms = top.roomData.GetBottomPossibleDoors(gData.floor);
        var tops = bottom.roomData.GetTopPossibleDoors(gData.floor);
        var possibles = new List<int>();
        for (int x = 0; x < gData.fixedRoomSizeX; x++) {
            if (bottoms[x] == true && tops[x] == true)
                possibles.Add(x);
        }
        int posX = possibles[UnityEngine.Random.Range(0, possibles.Count)];
        top.SetDoorBottom(posX);
        bottom.SetDoorTop(posX);
    }
    public void ConnectDoorHorizontal(BaseFixedRoom left, BaseFixedRoom right) {
        var lefts = left.roomData.GetRightPossibleDoors(gData.floor);
        var rights = right.roomData.GetLeftPossibleDoors(gData.floor);
        var possibles = new List<int>();
        for (int y = 0; y < gData.fixedRoomSizeY; y++) {
            if (lefts[y] == true && rights[y] == true)
                possibles.Add(y);
        }
        int posY = possibles[UnityEngine.Random.Range(0, possibles.Count)];
        left.SetDoorRight(posY);
        right.SetDoorLeft(posY);
    }
    public void GenerateCorridor(int length, int maxRoomCount) {
        var curRoom = GetEmptyRoomWithStandartNeighbor();
        if (curRoom == null)
            return;
        var neighbours = new List<BaseFixedRoom>();
        BaseFixedRoom neighbour = null;
        for (int i = 0; i < length; i++) {
            if (CountOfRooms() >= maxRoomCount)
                break;
            SetFixedRoom(GetStandartRoom(curRoom.fixedX, curRoom.fixedY), curRoom.fixedX, curRoom.fixedY);
            neighbours = GetFreeNeighbours(curRoom);
            neighbours = RemoveRoomWithMoreThanOneNeighbour(neighbours);
            if (neighbours.Count == 0)
                return;
            neighbour = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
            curRoom = neighbour;
        }
    }
    public bool GenerateConnection(BaseFixedRoom start, BaseFixedRoom end) {
        var path = FindPath(start, end);
        if (path == null)
            return false;
        for (int i = 0; i < path.Length; i++) {
            var s = GetStandartRoom(path[i].x, path[i].y);
            SetFixedRoom(s, path[i].x, path[i].y);
        }
        return true;
    }
    BaseFixedRoom GetStandartRoom(int targetPosX, int targetPosY) {
        var roomData = RemoveDataThatHasNotTheRightDoors(gData.roomStandartDataList.Cast<BaseFixedRoomData>().ToList(), targetPosX, targetPosY);
        if (roomData.Count == 0)
            return gData.defaultStandartRoom.GetStructure() as BaseFixedRoom;
        return GetRandomRoomFromList(roomData).GetStructure() as BaseFixedRoom;
    }
    List<BaseFixedRoomData> RemoveDataThatHasNotTheRightDoors(List<BaseFixedRoomData> list, int targetX, int targetY) {
        var top = IsFreePos(targetX, targetY + 1) ? null : GetRoom(targetX, targetY + 1).roomData;
        var bottom = IsFreePos(targetX, targetY - 1) ? null : GetRoom(targetX, targetY - 1).roomData;
        var left = IsFreePos(targetX - 1, targetY) ? null : GetRoom(targetX - 1, targetY).roomData;
        var right = IsFreePos(targetX + 1, targetY) ? null : GetRoom(targetX + 1, targetY).roomData;

        for (int i = list.Count - 1; i >= 0; i--) {
            if (bottom != null && DoorCompareSide(bottom.GetTopPossibleDoors(gData.floor), list[i].GetBottomPossibleDoors(gData.floor)) == false)
                list.RemoveAt(i);
            else if (top != null && DoorCompareSide(top.GetBottomPossibleDoors(gData.floor), list[i].GetTopPossibleDoors(gData.floor)) == false)
                list.RemoveAt(i);
            else if (right != null && DoorCompareSide(right.GetLeftPossibleDoors(gData.floor), list[i].GetRightPossibleDoors(gData.floor)) == false)
                list.RemoveAt(i);
            else if (left != null && DoorCompareSide(left.GetRightPossibleDoors(gData.floor), list[i].GetLeftPossibleDoors(gData.floor)) == false)
                list.RemoveAt(i);
        }

        return list;
    }
    bool DoorCompareSide(List<bool> sideA, List<bool> sideB) {

        int doorSize = BaseFixedRoomData.doorSize;
        int count = 0;
        for (int i = 0; i < sideA.Count; i++) {
            if (sideA[i] == true && sideB[i] == true)
                count++;
            else
                count = 0;
            if (count >= doorSize)
                return true;
        }
        return false;
    }
    public BaseFixedRoom GetRoom(int x, int y) {
        if (IsOutOfBound(x, y))
            return null;
        return rooms[x + y * gData.levelSizeX];
    }
    public void SetFixedRoom(BaseFixedRoom room, int x, int y) {
        RemoveFreePos(x, y);
        room.SetFixedPos(x, y);
        rooms[x + y * gData.levelSizeX] = room;
    }
    public void RemoveFreePos(int x, int y) {
        for (int i = 0; i < freeRooms.Count; i++) {
            if (freeRooms[i].fixedX == x && freeRooms[i].fixedY == y) {
                freeRooms.RemoveAt(i);
                break;
            }

        }
    }
    public bool IsFreePos(int x, int y) {
        return GetRoom(x, y) == null || GetRoom(x, y).roomData == null;
    }
    BaseFixedRoom GetEmptyRoomWithStandartNeighbor() {
        var list = new List<BaseFixedRoom>();
        for (int i = 0; i < rooms.Length; i++) {
            if (HaveConnectionToStandartRoom(rooms[i]) && IsSpecialRoom(rooms[i]) == false && HaveSpecialRoomAsNeighbour(rooms[i]) == false)
                list.Add(rooms[i]);
        }
        if (list.Count == 0)
            return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    Vector2Int FindEntryPos(BaseFixedRoom exit, float minDistance) {
        var list = new List<BaseFixedRoom>();
        var distanceList = new List<float>();
        var posExit = new Vector2Int(exit.fixedX, exit.fixedX);
        for (int i = 0; i < rooms.Length; i++) {
            var pos = new Vector2Int(rooms[i].fixedX, rooms[i].fixedX);
            if (IsStandartRoom(rooms[i])) {
                list.Add(rooms[i]);
                distanceList.Add(Vector2Int.Distance(posExit, pos));
            }
                    
        }

        if (list.Count == 0)
            return Vector2Int.one * -1;
        var maxDistance = float.MinValue;
        var maxDistanceIndex = -1;
        for (int i = 0; i < list.Count; i++) {
            if ( distanceList[i] > maxDistance) {
                maxDistance = distanceList[i];
                maxDistanceIndex = i;
            }
        }
        if (maxDistance < minDistance) {
            return new Vector2Int(list[maxDistanceIndex].fixedX, list[maxDistanceIndex].fixedY);
        }
        for (int i = list.Count-1; i > 0; i--) {
            if (distanceList[i] < minDistance) {
                list.RemoveAt(i);
                distanceList.RemoveAt(i);
            }
        }
        var r = list[UnityEngine.Random.Range(0, list.Count)];
        return new Vector2Int(r.fixedX, r.fixedY);
    }
    public Vector2Int GetRandomRoomPos() {
        return new Vector2Int(UnityEngine.Random.Range(0, gData.levelSizeX), UnityEngine.Random.Range(0, gData.levelSizeY));
    }
    public Vector2Int GetRandomFreeRoomPos() {
        int i = UnityEngine.Random.Range(0, freeRooms.Count);
        if (freeRooms[i].IsEmpty() == false)
            return Vector2Int.one * -1;
        return new Vector2Int(freeRooms[i].fixedX, freeRooms[i].fixedY);
    }
    public Vector2Int GetRandomFreeRoomPos(int distance, BaseFixedRoom from) {
        Vector2 fromPos = new Vector2(from.fixedX, from.fixedY);
        Vector2 targetPos = Vector2.zero;
        targetPos = fromPos + GetRandomDir(from) * distance;

        Vector2Int pos = new Vector2Int((int)targetPos.x, (int)targetPos.y);

        int i = UnityEngine.Random.Range(0, freeRooms.Count);
        if (IsOutOfBound(pos.x, pos.y) || IsFreePos(pos.x, pos.y) == false)
            return Vector2Int.one * -1;
        return pos;
    }
    public Vector2Int GetRandomFreeRoomPos(int distance, BaseFixedRoom from, BaseFixedRoom from2) {
        Vector2 fromPos = new Vector2(from.fixedX, from.fixedY);
        Vector2 fromPos2 = new Vector2(from2.fixedX, from2.fixedY);
        Vector2 targetPos = Vector2.zero;
        targetPos = (fromPos + fromPos2) / 2 + (GetRandomDir(from) + GetRandomDir(from2)) * distance / 2;

        Vector2Int pos = new Vector2Int((int)targetPos.x, (int)targetPos.y);

        int i = UnityEngine.Random.Range(0, freeRooms.Count);
        if (IsOutOfBound(pos.x, pos.y) || IsFreePos(pos.x, pos.y) == false)
            return Vector2Int.one * -1;
        return pos;
    }
    Vector2 GetRandomDir(BaseFixedRoom from) {
        Vector2 dir = Vector2.up;
        Vector2 fromPos = new Vector2(from.fixedX, from.fixedY);
        Vector2 centerPos = new Vector2(center.x, center.y);
        Vector2 targetPos = Vector2.zero;
        dir = centerPos - fromPos;
        if (dir.magnitude < 2) {
            dir = Vector2.up;
            dir = dir.Rotate(UnityEngine.Random.Range(-180, 180));
        } else {
            dir.Normalize();
            dir = dir.Rotate(UnityEngine.Random.Range(-45, 45));
        }
        return dir;
    }
    bool IsOutOfBound(int x, int y) {
        if (x < 0 || x >= gData.levelSizeX)
            return true;
        if (y < 0 || y >= gData.levelSizeY)
            return true;
        return false;
    }
    public T GetRandomRoomFromList<T>(List<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }




    //Path

    Vector2Int[] FindPath(BaseFixedRoom startRoom, BaseFixedRoom targetRoom) {


        Vector2Int[] waypoints = new Vector2Int[0];
        bool pathSuccess = false;

        startRoom.parent = startRoom;

        List<BaseFixedRoom> openSet = new List<BaseFixedRoom>();
        HashSet<BaseFixedRoom> closedSet = new HashSet<BaseFixedRoom>();
        openSet.Add(startRoom);
        while (openSet.Count > 0) {
            BaseFixedRoom currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            if (HaveRoomAsNeighbour(currentNode, targetRoom) || HaveConnectionToStandartRoom(currentNode)) {
                targetRoom.parent = currentNode;
                pathSuccess = true;
                break;
            }

            foreach (BaseFixedRoom neighbour in GetFreeNeighbours(currentNode)) {
                if (!IsSelectable(neighbour) || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetRoom);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);

                }
            }
        }


        if (pathSuccess) {
            return waypoints = RetracePath(startRoom, targetRoom);
        }
        return null;
        //requestManager.FinishedProcessingPath(waypoints, pathSuccess);

    }

    Vector2Int[] RetracePath(BaseFixedRoom startNode, BaseFixedRoom endNode) {
        List<BaseFixedRoom> path = new List<BaseFixedRoom>();
        BaseFixedRoom currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Remove(startNode);
        path.Remove(endNode);
        Vector2Int[] waypoints = new Vector2Int[path.Count];
        for (int i = 0; i < path.Count; i++) {
            waypoints[i] = new Vector2Int(path[i].fixedX, path[i].fixedY);
        }
        Array.Reverse(waypoints);
        return waypoints;

    }
    List<BaseFixedRoom> GetFreeNeighbours(BaseFixedRoom from) {
        var list = new List<BaseFixedRoom>();
        int x = from.fixedX;
        int y = from.fixedY;
        if (IsOutOfBound(x - 1, y) == false && IsFreePos(x - 1, y))
            list.Add(GetRoom(x - 1, y));
        if (IsOutOfBound(x + 1, y) == false && IsFreePos(x + 1, y))
            list.Add(GetRoom(x + 1, y));
        if (IsOutOfBound(x, y - 1) == false && IsFreePos(x, y - 1))
            list.Add(GetRoom(x, y - 1));
        if (IsOutOfBound(x, y + 1) == false && IsFreePos(x, y + 1))
            list.Add(GetRoom(x, y + 1));
        return list;
    }
    List<BaseFixedRoom> GetNeighbours(BaseFixedRoom from) {
        var list = new List<BaseFixedRoom>();
        int x = from.fixedX;
        int y = from.fixedY;
        if (IsOutOfBound(x - 1, y) == false && IsFreePos(x - 1, y) == false)
            list.Add(GetRoom(x - 1, y));
        if (IsOutOfBound(x + 1, y) == false && IsFreePos(x + 1, y) == false)
            list.Add(GetRoom(x + 1, y));
        if (IsOutOfBound(x, y - 1) == false && IsFreePos(x, y - 1) == false)
            list.Add(GetRoom(x, y - 1));
        if (IsOutOfBound(x, y + 1) == false && IsFreePos(x, y + 1) == false)
            list.Add(GetRoom(x, y + 1));


        var randomizedList = new List<BaseFixedRoom>();
        var rnd = new System.Random();
        while (list.Count != 0) {
            var index = rnd.Next(0, list.Count);
            randomizedList.Add(list[index]);
            list.RemoveAt(index);
        }
        return randomizedList;
    }
    List<BaseFixedRoom> RemoveRoomWithMoreThanOneNeighbour(List<BaseFixedRoom> list) {
        for (int i = list.Count - 1; i >= 0; i--) {
            if (GetFreeNeighbours(list[i]).Count < 3)
                list.RemoveAt(i);
        }
        return list;
    }
    bool HaveConnectionToStandartRoom(BaseFixedRoom room) {
        var list = GetNeighbours(room);
        for (int i = 0; i < list.Count; i++) {
            if (IsStandartRoom(list[i]))
                return true;
        }
        return false;
    }
    bool HaveAnyRoomAsNeighbour(BaseFixedRoom room) {
        var list = GetNeighbours(room);
        for (int i = 0; i < list.Count; i++) {
            if (list[i].IsEmpty() == false)
                return true;
        }
        return false;
    }
    bool HaveRoomAsNeighbour(BaseFixedRoom room, BaseFixedRoom neighbour) {
        var list = GetNeighbours(room);
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == neighbour)
                return true;
        }
        return false;
    }
    bool IsSelectable(BaseFixedRoom target) {
        var s = GetSpecialRoomNeighbour(target);
        if (s == null)
            return true;
        if (HaveConnectionToStandartRoom(s))
            return false;
        return true;
    }
    BaseFixedRoom GetSpecialRoomNeighbour(BaseFixedRoom target) {
        var list = GetNeighbours(target);
        for (int i = 0; i < list.Count; i++)
            if (IsSpecialRoom(list[i]))
                return list[i];

        return null;
    }
    bool HaveSpecialRoomAsNeighbour(BaseFixedRoom target) {
        var list = GetNeighbours(target);
        for (int i = 0; i < list.Count; i++)
            if (IsSpecialRoom(list[i]))
                return true;

        return false;
    }
    bool IsSpecialRoom(BaseFixedRoom r) {
        if (r is FixedRoom_Shop)
            return true;
        if (r is FixedRoom_Exit)
            return true;
        if (r is FixedRoom_Chest)
            return true;
        return false;
    }
    bool IsStandartRoom(BaseFixedRoom r) {
        if (r is FixedRoom_Standart)
            return true;
        return false;
    }
    int GetDistance(BaseFixedRoom nodeA, BaseFixedRoom nodeB) {
        int dstX = Mathf.Abs(nodeA.fixedX - nodeB.fixedX);
        int dstY = Mathf.Abs(nodeA.fixedY - nodeB.fixedY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    int CountOfRooms() {
        int count = 0;
        for (int i = 0; i < rooms.Length; i++)
            if (rooms[i].IsEmpty() == false)
                count++;
        return count;
    }
    public override List<Bounds> GetBounds() {
        var list = base.GetBounds();
        for (int i = 0; i < structureList.Count; i++) {
            list.AddRange(structureList[i].GetBounds());
        }
        return list;
    }
    public override void Move(int x, int y) {
        base.Move(x, y);
        for (int i = 0; i < structureList.Count; i++) {
            structureList[i].Move(x, y);
        }
    }
}
[CreateAssetMenu(fileName = "FixedRoomGenerator",menuName ="TileMap/LevelGenerator/FixedRoom/Generator")]
public class FixedRoomGeneratorData : BaseLevelStructureData {
    public List<FixedRoomData_Exit> roomExitDataList;
    public List<FixedRoomData_Entry> roomEntryDataList;
    public List<FixedRoomData_Shop> roomShopDataList;
    public List<FixedRoomData_Chest> roomChestDataList;
    public List<FixedRoomData_Standart> roomStandartDataList;
    public FixedRoomData_Standart defaultStandartRoom;
    public int fixedRoomSizeX = 28;
    public int fixedRoomSizeY = 18;

    public int levelSizeX;
    public int levelSizeY;
    public Vector2Int roomCountRange;
    public Vector2Int additionalCoridorLengthRange;
    public TileLevelData floor;
    public TileLevelData wall;

    public override BaseLevelStructure GetStructure() {
        var s = new FixedRoomGenerator();
        s.Init(this);
        return s;
    }
    public void UpdateRoomData() {
        for (int i = 0; i < roomExitDataList.Count; i++) {
            roomExitDataList[i].UpdateDoors();
        }
        for (int i = 0; i < roomShopDataList.Count; i++) {
            roomShopDataList[i].UpdateDoors();
        }
        for (int i = 0; i < roomExitDataList.Count; i++) {
            roomExitDataList[i].UpdateDoors();
        }
        for (int i = 0; i < roomChestDataList.Count; i++) {
            roomChestDataList[i].UpdateDoors();
        }
        for (int i = 0; i < roomStandartDataList.Count; i++) {
            roomStandartDataList[i].UpdateDoors();
        }
    }
}
