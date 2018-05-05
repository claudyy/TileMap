using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileMap_SideView : LevelTilemap {
    public Tilemap background;
    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new Tile_SideView();
    }

    public override void LoadTileFirstTime(BaseLevelTile tile) {
        base.LoadTileFirstTime(tile);
        vec3IntTemp.x = tile.pos.x;
        vec3IntTemp.y = tile.pos.y;
        vec3IntTemp.z = 0;
        var backgroundTile = (tile.data as Tile_SideViewData).backgroundTile;
        background.SetTile(vec3IntTemp, backgroundTile);
    }
    public override void UpdateViewTile(BaseLevelTile tile) {
        base.UpdateViewTile(tile);
        var backgroundTile = (tile.data as Tile_SideViewData).backgroundTile;
        background.SetTile(vec3IntTemp, backgroundTile);
    }
    public override void ClearTilemap() {
        base.ClearTilemap();
        background.ClearAllTiles();
    }
}
