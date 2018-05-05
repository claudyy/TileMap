using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap_Dig : LevelTilemap {

    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new LevelTile_Dig();
    }
}
