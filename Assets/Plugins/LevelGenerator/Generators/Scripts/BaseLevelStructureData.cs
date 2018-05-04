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
    public virtual IEnumerator RunTimeGenerate(GeneratorMapData map, MonoBehaviour mono) {
        yield return null;
    }
    public virtual void Generate(GeneratorMapData map) {

    }
    public virtual void TryGenerate(BaseLevelGenerator generator) {

    }
    public virtual IEnumerator RunTimeTryGenerate(BaseLevelGenerator generator, MonoBehaviour mono) {
        yield return null;
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
    public bool IntersectBound(Ray ray) {
        var myBounds = GetBounds();
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].IntersectRay(ray) == false)
                return false;
        }
        return true;
    }
    public bool InBound(Vector3 pos) {
        var myBounds = GetBounds();
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].Contains(pos) == false)
                return false;
        }
        return true;
    }
    public bool InBound(List<Bounds> otherBound) {
        for (int i = 0; i < otherBound.Count; i++) {
            if (InBound(otherBound[i]) == true)
                return true;
        }
        return false;
    }
    public bool InBound(Bounds otherBound) {
        var myBounds = GetBounds();
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].Intersects(otherBound) == true)
                return true;
        }
        return false;
    }
    public int GetMinX() {
        var myBounds = GetBounds();
        if (myBounds.Count == 0)
            return posX;
        int x = (int)myBounds[0].min.x;
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].min.x < x)
                x = (int)myBounds[i].min.x;
        }
        return x;
    }
    public int GetMinY() {
        var myBounds = GetBounds();
        if (myBounds.Count == 0)
            return posY;
        int y = (int)myBounds[0].min.y;
        for (int i = 0; i < myBounds.Count; i++) {
            if (myBounds[i].min.y < y)
                y = (int)myBounds[i].min.y;
        }
        return y;
    }
    public virtual void Move(int x,int y) {
        posX += x;
        posY += y;
    }
}
public class BaseLevelStructureData : ScriptableObject {


    public virtual BaseLevelStructure GetStructure() {
        return new BaseLevelStructure();
    }

    public float ReadTexture(Texture2D tex, float x, float y)
    {
        int posX = (int)(x * tex.width);
        int posY = (int)(y * tex.height);

        return tex.GetPixel(posX, posY).r;
    }

}
