using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTileData_Isometric : LevelTileData {

	public BaseTileMapGameobject go;
    public override void Init(LevelTilemap map, BaseLevelTile tile) {
        base.Init(map, tile);
        if(go != null) {
            map.AddGameObjectToTile(tile, go);
        }
    }
}
