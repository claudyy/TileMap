using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTileData_Isometric : TileLevelData {

	public BaseTileMapGameobject go;
    public override void Init(LevelTilemap map, BaseTile tile) {
        base.Init(map, tile);
        if(go != null) {
            map.AddGameObjectToTile(tile, go);
        }
    }
}
