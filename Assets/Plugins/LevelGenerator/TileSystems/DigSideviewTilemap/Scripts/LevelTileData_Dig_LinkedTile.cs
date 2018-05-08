using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelTileData_Dig_LinkedTile : LevelTileData_Dig {
    public Vector2Int size = Vector2Int.one;
    public override void Init(LevelTilemap map, BaseLevelTile tile) {
        base.Init(map, tile);
        map.SetLinks(size,tile);
    }
    public override void Remove(LevelTilemap map, BaseLevelTile tile) {
        base.Remove(map, tile);
        map.RemoveLinks(size,tile);
    }
}
