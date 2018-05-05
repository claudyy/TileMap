using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTest : MonoBehaviour {
    public LevelTilemap level;
    public LevelTileData data;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 originPos = transform.position + Vector3.up * .3f;
            Vector2Int targetPos = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            level.Erase(targetPos);
        }
        if (Input.GetMouseButtonDown(2)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 originPos = transform.position + Vector3.up * .3f;
            Vector2Int targetPos = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            level.OverrideTile(targetPos, data);
        }
    }
}
