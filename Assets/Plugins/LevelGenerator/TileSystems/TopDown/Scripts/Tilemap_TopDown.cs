using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap_TopDown : LevelTilemap {
    public override BaseTile CreateTile(Vector3Int pos, TileLevelData data) {
        return new TopDownTile();
    }

}
