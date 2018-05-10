using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Tilemap_MiniMap : MonoBehaviour {
    public LevelTilemap tilemap;
    public ScrollRect scroll;
    RawImage[] images;
    public bool followerCameraPos;
	// Use this for initialization
	void Start () {
        images = new RawImage[tilemap.levelChunkX * tilemap.levelChunkY];
        for (int y = 0; y < tilemap.levelChunkY; y++) {
            for (int x = 0; x < tilemap.levelChunkX; x++) {
                GameObject go = new GameObject();
                go.name = "Image" + x + "/" + y;
                go.transform.SetParent(transform);
                images[x + y * tilemap.levelChunkX] = go.AddComponent<RawImage>();
                images[x + y * tilemap.levelChunkX].texture = tilemap.allChunksRT[GetRTIndex(x,y)];
                go.transform.localScale = Vector3.one;
            }
        }
	}
	int GetRTIndex(int x, int y) {
        //x = tilemap.levelChunkX -1 - x;
        y = tilemap.levelChunkY - 1 - y;
        return (x + (y * tilemap.levelChunkX));
    }
    public void FocusOnPosition(Vector2 pos) {
        scroll.normalizedPosition = pos;
    }
	// Update is called once per frame
	void Update () {
        if (followerCameraPos) {
            var indexPos = tilemap.WorldPosToIndex(Camera.main.transform.position);
            FocusOnPosition(new Vector2((float)indexPos.x / tilemap.sizeX, (float)indexPos.y / tilemap.sizeY));
        }
    }
}
