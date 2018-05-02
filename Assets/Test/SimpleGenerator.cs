using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGenerator : BaseLevelGenerator {
    
    public BaseLevelStructureData structure;
    protected override IEnumerator RunTimeTryGenerate() {
        structureList = new List<BaseLevelStructure>(1) {structure.GetStructure()};
        yield return null;
    }
    protected override void TryGenerate() {
        var s = structure.GetStructure();
        s.TryGenerate(this);
        structureList = new List<BaseLevelStructure>(1) {s};

    }
    protected override bool CheckGeneratedResult(List<BaseLevelStructure> structure) {
        return true;
    }
    
}
