using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoSorting : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder =1000-(int)(transform.position.y * 10);
	}
}
