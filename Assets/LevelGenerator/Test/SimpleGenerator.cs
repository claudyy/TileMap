using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGenerator : BaseLevelGenerator {

    public BaseLevelStructureData structure;
    protected override IEnumerator TryGenerate() {
        structureList = new List<BaseLevelStructure>(1) { structure.GetStructure(level) };
        yield return null;
    }
    protected override bool CheckGeneratedResult(List<BaseLevelStructure> structure) {
        return true;
    }

}
