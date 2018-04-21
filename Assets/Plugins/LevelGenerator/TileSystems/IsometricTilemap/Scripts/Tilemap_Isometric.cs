using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Tilemap_Isometric : LevelTilemap {
    public Grid uneavenGrid;
    public override BaseTile CreateTile(Vector3Int pos, TileLevelData data) {
        return new LevelTile_Isometric();
    }
    public override void SetTile(Vector3Int pos, TileBase tile) {
        Vector3Int position = pos;
        position.x = pos.x  - pos.y;
        position.y = pos.x + pos.y;

        tilemap.SetTile(position, tile);
    }
    public override void ClearTilemap() {
        base.ClearTilemap();

    }
}
