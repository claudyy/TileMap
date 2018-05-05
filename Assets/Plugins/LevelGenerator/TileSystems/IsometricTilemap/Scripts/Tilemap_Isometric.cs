using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Tilemap_Isometric : LevelTilemap {
    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new LevelTile_Isometric();
    }

    public override Vector3 IndexToWorldPos(Vector3Int index) {
        Vector3 position = index;
        position.x = index.x - index.y + .5f;
        position.y = (index.x + index.y)/2 ;
        if((index.y % 2 == 1) || (index.x % 2 == 1 ))
            position.y += .5f;
        if(((index.y % 2 == 1) && (index.x % 2 == 1)) ==false)
            position.y += .5f;

        return position;
    }
    public override Vector3Int IndexToTilemap(Vector3Int index) {
        Vector3Int position = index;
        position.x = index.x - index.y;
        position.y = index.x + index.y;
        return position;
    }
}
