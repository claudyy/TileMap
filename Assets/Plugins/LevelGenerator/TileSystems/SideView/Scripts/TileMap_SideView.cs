using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileMap_SideView : LevelTilemap {
    public Tilemap background;
    public override BaseTile CreateTile(Vector3Int pos, TileLevelData data) {
        return new Tile_SideView();
    }
    public override void UpdateTile(BaseTile tile) {
        base.UpdateTile(tile);
    }
    public override void LoadTileFirstTime(BaseTile tile) {
        base.LoadTileFirstTime(tile);
        vec3IntTemp.x = tile.pos.x;
        vec3IntTemp.y = tile.pos.y;
        vec3IntTemp.z = 0;
        var sideviewTile = tile as Tile_SideView;
        var backgroundTile = (sideviewTile.data as Tile_SideViewData).backgroundTile;
        background.SetTile(vec3IntTemp, backgroundTile);
    }

    public override void SetTile(Vector3Int pos, TileBase tile) {
        base.SetTile(pos, tile);
    }
}
