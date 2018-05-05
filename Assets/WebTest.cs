using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class WebTest : MonoBehaviour {
    public TileBase tile;
	// Use this for initialization
	void Start () {
        GetComponent<Tilemap>().SetTile(Vector3Int.zero, tile);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
