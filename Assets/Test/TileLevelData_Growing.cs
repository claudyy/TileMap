using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileBehavior_Growing : BaseTileBehvior
{
    /*
    public TileLevelData_Growing growingData;
    float curGrowTime;
    public TileBehavior_Growing(TileLevelData_Growing growingData)
    {
        this.growingData = growingData;
    }
    public override void Update(LevelTilemap level, LevelTile tile)
    {
        base.Update(level,tile);
        curGrowTime += deltaTime;
        if (curGrowTime > growingData.growingSpeed)
        {
            curGrowTime = 0;
            Grow(level, tile);
        }
    }
    void Grow(LevelTilemap level, LevelTile tile)
    {
        for (int i = 0; i < growingData.maxSize; i++)
        {
            int x = tile.pos.x;
            int y = tile.pos.y + i;
            if (level.GetTile(x, y).IsEmpty())
            {
                level.GetTile(x, y).OverrideData(level,growingData.growingTile);
                break;
            }
        }
        //int x = tile.pos.x + (int)Random.Range(-1 ,2 );
        //int y = tile.pos.y + (int)Random.Range(-1 ,2 );
        //Debug.Log(x + " / " + y+ " / isEmpty" + level.GetTile(x, y).IsEmpty());
        //if (level.GetTile(x, y).IsEmpty())
            //level.GetTile(x, y).OverrideData(growingData.growingTile); 
    }
    */
}
[CreateAssetMenu(menuName = "Data/LevelGeneration/TileData_Growing")]
public class TileLevelData_Growing : TileLevelData
{
    /*
    public TileLevelData growingTile;
    public float growingSpeed;
    public int maxSize;
    public override BaseTileBehvior GetBehavior()
    {
        return new TileBehavior_Growing(this);
    }
    */
}
