using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Tilemap_Dig : LevelTilemap {
    public Tilemap backgroundTileMap;
    public Tilemap lightMap;
    public Tilemap objectTileMap;
    public Tilemap waterTileMap;
    public Tilemap damageTileMap;
    public override BaseLevelTile CreateTile(Vector3Int pos, LevelTileData data) {
        return new LevelTile_Dig();
    }
    public override void ClearTilemap() {
        base.ClearTilemap();
        backgroundTileMap.ClearAllTiles();
    }
    public override void OverrideTile(int x, int y, LevelTileData data) {
        base.OverrideTile(x, y, data);

        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;

    }
    public void SetBckground(int x, int y, TileBase tile) {

        backgroundTileMap.SetTile(IndexToTilemap(x,y), tile);
    }
    public override void LoadTileFirstTime(BaseLevelTile tile) {
        base.LoadTileFirstTime(tile);
        var data = tile.data;
        if (data == null)
            return;
        var digData = data as LevelTileData_Dig;
        if (digData == null)
            return;

    }
    public override void EditorApplySaveFileToTile(int x, int y, LevelTileData data) {
        base.EditorApplySaveFileToTile(x, y, data);

    }
}
