using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTileFluidData : LevelTileData, IFluidData {
    public Texture2D fluidTileCollision;
    Texture2D IFluidData.Collision() {
        return fluidTileCollision;
    }
}
