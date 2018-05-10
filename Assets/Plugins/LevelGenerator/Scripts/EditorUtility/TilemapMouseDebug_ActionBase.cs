using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapMouseDebug_ActionBase : MonoBehaviour {
    public LevelTilemap target;

	// Update is called once per frame
	void Update () {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        var pos = mousePos;
        pos.x = Mathf.Floor(pos.x);
        pos.y = Mathf.Floor(pos.y);
        var tile = target.GetITile((int)pos.x, (int)pos.y,false);

        if (tile != null && Input.GetMouseButtonDown(0)) {
            ActionOnTile(tile);
        }
    }
    public virtual void ActionOnTile(BaseLevelTile tile) {

    }
}
