using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : BaseLevelGenerator {

    public List<BiomData> bioms;
    protected override IEnumerator TryGenerate() {
        var sTemp = new List<BaseLevelStructure>();
        int lastY = 0;
        int totalY = 0;
        for (int i = 0; i < bioms.Count; i++) {
            totalY += bioms[i].sizeY;
            yield return null;
        }
        for (int i = 0; i < bioms.Count; i++) {
            var b = bioms[i].GetStructure();
            lastY += bioms[i].sizeY;
            b.posY = totalY - lastY;
            sTemp.Add(b);
            yield return null;
        }
        structureList = sTemp;
    }
}
