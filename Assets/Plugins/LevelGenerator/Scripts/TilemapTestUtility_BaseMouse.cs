using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_Base : MonoBehaviour {
    public bool displayText = true;
    public virtual string TileText(BaseLevelTile tile) {
        return "";
    }
}
