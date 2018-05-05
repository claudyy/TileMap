using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap_TopDown : LevelTilemap {
    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new TopDownTile();
    }

}
