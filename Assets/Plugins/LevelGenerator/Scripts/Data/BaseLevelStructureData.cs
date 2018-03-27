using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseLevelStructure{
    protected List<Bounds> boundList;
    public int posX = 0;
    public int posY = 0;
    public virtual void Init(BaseLevelStructureData data) {
    }
    public bool IntersectWithBound(Bounds bound) {
        for (int i = 0; i < boundList.Count; i++) {
            if (boundList[i].Intersects(bound))
                return true;
        }
        return false;
    }
    public virtual IEnumerator Generate(GeneratorMapData map) {
        yield return null;
    }
    public virtual void TryGenerate(BaseLevelGenerator generator) {

    }
    public virtual List<Bounds> GetBounds() {
        return new List<Bounds>();
    }
    public void EncapsulateBound(ref Bounds otherBound) {
        var myBounds = GetBounds();
        for (int i = 0; i < myBounds.Count; i++) {
            otherBound.Encapsulate(myBounds[i].max);
        }
    }
    public bool InBound(Vector3 pos) {
        var myBounds = GetBounds();
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].Contains(pos) == false)
                return false;
        }
        return true;
    }
    public int GetMinX() {
        var myBounds = GetBounds();
        int x = posX;
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].min.x < x)
                x = (int)myBounds[i].min.x;
        }
        return x;
    }
    public int GetMinY() {
        var myBounds = GetBounds();
        int y = posY;
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].min.y< y)
                y = (int)myBounds[i].min.y;
        }
        return y;
    }
    public void Move(int x,int y) {
        posX += x;
        posY += y;
    }
}
public class BaseLevelStructureData : ScriptableObject {


    public virtual BaseLevelStructure GetStructure() {
        return new BaseLevelStructure();
    }
    public virtual void Generate(Rect rect, GeneratorMapData map) {

    }
    public virtual void Generate(Rect rect,LevelTilemap level,bool[,] canOverwrite)
    {
		for (int i = 0; i < 2; i++) {
			if (TryToGenerate (rect,level,canOverwrite))
				return;
		}
    }
	public virtual bool TryToGenerate(Rect rect,LevelTilemap level,bool[,] canOverwrite){
		return true;
	}
    public float ReadTexture(Texture2D tex, float x, float y)
    {
        int posX = (int)(x * tex.width);
        int posY = (int)(y * tex.height);

        return tex.GetPixel(posX, posY).r;
    }

}
