using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NoiseTextureMixType{
    multiply,
    add,
    overlay
}

[System.Serializable]
public class NoiseTexture
{
    public Texture2D noiseTexture;
    public NoiseTextureMixType type;
    public bool randomX;
    public bool randomY;
    public float noiseXSize = 1;
    public float noiseYSize = 1;
    [Range(0,1)]
    public float mixFactor;
    public bool invert;
    public float GetValue(float percentX,float percentY, float randomValue)
    {
        GenerateUtility utility = new GenerateUtility();
        percentX *= noiseXSize;
        percentY *= noiseYSize;
        if (randomX)
            percentX += randomValue;
        if (randomY)
            percentY += randomValue;
        var v = utility.ReadTexture(noiseTexture, percentX, percentY);
        v = Mathf.Clamp(v, 0, 1);
        if (invert)
            v = 1 - v;
        return v;
    }
    public void MixWithValue(float percentX, float percentY, float randomValue, ref float value) {
        float v = 0;
        switch (type) {
            case NoiseTextureMixType.multiply:
                v = value * GetValue(percentX, percentY,randomValue);
                break;
            case NoiseTextureMixType.add:
                v = value + GetValue(percentX, percentY, randomValue);
                break;
            case NoiseTextureMixType.overlay:
                var ov = GetValue(percentX, percentX, randomValue);
                v = v < .5 ? (2 * v * ov) : (1 - 2 * (1 - v) * (1 - ov));
                break;
            default:
                v = value;
                break;
        }
        value = Mathf.Lerp(value, v, mixFactor);
    }
}
[System.Serializable]
public class NoiseMixer{
	public List<NoiseTexture> noiseTexture;
	public float GetValue(float percentX,float percentY,List<float> randomValue)
	{
		float value = 1;
        var pXTemp = percentX;
        var pYTemp = percentY;
        for (int i = 0; i < noiseTexture.Count; i++)
		{
			if(i == 0) {
                value = noiseTexture[i].GetValue(percentX , percentY, randomValue[i]);

            } else {
                noiseTexture[i].MixWithValue(percentX, percentY, randomValue[i], ref value);
            }

        }
		return value;
	}
}
[System.Serializable]
public class BiomWallData{
	
    public TileLevelData data;
    public float blendFactore = .5f;
	public bool randomOffset;
	public NoiseMixer noiseMixer;
	public void Generate(Rect rect,LevelTilemap level,bool[,] canOverwrite)
    {
       //GenerateUtility utility = new GenerateUtility();
       //float value = 1;
		//float rndValue;
		//rndValue = Random.value;
		//for (int x = 0; x < rect.width; x++)
       //{
		//	for (int y = 0; y < rect.height; y++)
       //    {
		//		if (canOverwrite [x, y] == false)
		//			continue;
       //
		//		if( noiseMixer.GetValue((float)(x) / rect.width, (float)(y) / rect.height,rndValue) > blendFactore)
		//			level.OverrideTile((int)rect.x + x, (int)rect.y+y, data);
       //    }
       //}
        
    }
    public TileLevelData GetTile(int x , int y,int sizeX,int sizeY, List<float> randomValue) {
        TileLevelData tile = null;
        Vector2 offset = Vector2.zero;

        if (randomOffset)
            offset = new Vector2(Random.Range(0, sizeX), Random.Range(0, sizeY));
        if (noiseMixer.GetValue((float)(x + offset.x) / sizeX, (float)(y + offset.y) / sizeY, randomValue) > blendFactore)
            tile = data;
        return tile;
    }
}
[System.Serializable]
public class BiomPlantInfo{
	public int minDistance;
	public TileLevelData tile;
	public bool placeOnWall;
}
public class BiomStructure : BaseLevelStructure {
    public BiomData biomData;
    public override void Init(BaseLevelStructureData data) {
        base.Init(data);
        biomData = data as BiomData;
    }
    public override IEnumerator Generate(GeneratorMapData map) {
        int startX = posX;
        int startY = posY;

        //Board rnd
        List<float> borderRndList = new List<float>();
        for (int i = 0; i < biomData.border.noiseTexture.Count; i++) {
            borderRndList.Add(Random.value);
        }

        float rndValue;
        rndValue = Random.value;

        List<List<float>> rndList = new List<List<float>>();
        for (int i = 0; i < biomData.wallData.Count; i++) {
            var list = new List<float>();
            rndList.Add(list);
            for (int n = 0; n < biomData.wallData[i].noiseMixer.noiseTexture.Count; n++) {
                list.Add(Random.value);
            }
        }
        //Wall
        for (int x = 0; x < biomData.sizeX; x++) {
            for (int y = 0; y < biomData.sizeY; y++) {
                if (InBorder(x, y, borderRndList))
                    continue;
                var tile = biomData.baseTile;
                for (int i = 0; i < biomData.wallData.Count; i++) {
                    
                    var tileTemp = biomData.wallData[i].GetTile(x, y, biomData.sizeX, biomData.sizeY, rndList[i]);
                    if (tileTemp != null)
                        tile = tileTemp;
                }
                
                //level.OverrideTile(startX + x, startY + y, tile);
                map.OverrideTile(startX + x, startY + y, tile);
            }
        }
        //ores
        for (int i = 0; i < biomData.oreList.Count; i++) {
            var ore = biomData.oreList[i];
            int count = Random.Range(ore.depositCount.x, ore.depositCount.y);
            for (int d = 0; d < count; d++) {
                int x = Random.Range(0, biomData.sizeX);
                int y = Random.Range(0, biomData.sizeY);
                if (InBorder(x, y, borderRndList))
                    continue;
                int size = Random.Range(ore.depositSize.x, ore.depositSize.y);
                var positionList = GetOreDepositPositions(size, startX + x, startY + y,map);
                for (int s = 0; s < size; s++) {
                    if (positionList.Count == 0)
                        break;
                    int randomIndex = Random.Range(0, positionList.Count);
                    map.OverrideTile(positionList[s].x, positionList[s].y, ore.data);
                    //positionList.RemoveAt(randomIndex);
                }

            }
        }

        //structures
        ////Plants
        //List<Vector2Int>[] lastPlantPos = new List<Vector2Int>[plants.Count];
        //for (int i = 0; i < lastPlantPos.Length; i++) {
        //	lastPlantPos[i] = new List<Vector2Int> ();
        //}
        //for (int x = 0; x < level.sizeX; x++) {
        //	for (int y = 0; y < level.sizeY; y++) {
        //		for (int i = 0; i < plants.Count; i++) {
        //			Vector2Int pos = new Vector2Int(x, y);
        //			if (OutOfMinDisOfAlreadyUsed(lastPlantPos[i],pos,plants[i].minDistance) == false)
        //				continue;
        //			if (plants[i].placeOnWall && level.IsOnWall(x, y) == false)
        //				continue;
        //			
        //			lastPlantPos[i].Add(pos);
        //			break;
        //		}
        //	}
        //}
        //for (int i = 0; i < plants.Count; i++) {
        //	for (int p = 0; p < lastPlantPos[i].Count; p++) {
        //		level.OverrideTile(lastPlantPos[i][p], plants[i].tile);
        //	}
        //}
        yield return null;

    }
    public bool InBorder(int x,int y,List<float> rndList) {
        if (biomData.border.noiseTexture.Count == 0)
            return false;
        if (biomData.border.GetValue((float)x / biomData.sizeX, (float)y / biomData.sizeY, rndList) > biomData.borderBlend)
            return true;
        return false;
    }
    List<Vector2Int> GetOreDepositPositions(int size, int startX, int startY, GeneratorMapData map) {
        var list = new List<Vector2Int>();
        list.Add(new Vector2Int(startX, startY));
        for (int i = 1; i <= 1 + size / 7; i++) {
            list.Add(new Vector2Int(startX + i, startY));
            list.Add(new Vector2Int(startX + i, startY + i));
            list.Add(new Vector2Int(startX, startY + i));
            list.Add(new Vector2Int(startX - i, startY + i));
            list.Add(new Vector2Int(startX - i, startY));
            list.Add(new Vector2Int(startX - i, startY - i));
            list.Add(new Vector2Int(startX, startY - i));
            list.Add(new Vector2Int(startX + i, startY - i));
        }
        RemoveOutOfBoundFromList(list);
        return list;
    }
    public void RemoveOutOfBoundFromList(List<Vector2Int> list) {
        for (int i = list.Count - 1; i >= 0; i--) {
            if (InBound(new Vector3(list[i].x, list[i].y)) == false)
                list.RemoveAt(i);
        }
    }
    public override List<Bounds> GetBounds() {
        return new List<Bounds>(1) { new Bounds(new Vector3(posX + biomData.sizeX/2, posY + biomData.sizeY/2),new Vector3(posX + biomData.sizeX, posY + biomData.sizeY))};
    }
}
[CreateAssetMenu(menuName = "TileMap/LevelGenerator/Cave/Biom")]
public class BiomData : BaseLevelStructureData {
	public Vector2 randomDistanceFromCenter;
	public Vector2Int size;
    public int sizeX;
    public int sizeY;
    public TileLevelData baseTile;
    public NoiseMixer border;
	public float borderBlend = 0.5f;
    public List<BiomWallData> wallData;
    public List<BaseLevelStructureData> structure;
	public List<BiomPlantInfo> plants;
    [System.Serializable]
    public struct OreInfo {
        public TileLevelData data;
        public Vector2Int depositSize;
        public Vector2Int depositCount;
        public int minDistanceBetweenDeposit;
    }
    public List<OreInfo> oreList;
    public Bounds GenerateBound(Vector2 center){
		Vector2 dir = Vector2.up;
		dir = dir.Rotate(Random.Range(0, 360f));
		dir *= Random.Range (randomDistanceFromCenter.x, randomDistanceFromCenter.y);
		Vector3 pos = new Vector3 (center.x+dir.x,center.y+dir.y );
		return new Bounds(pos,new Vector3(size.x,size.y));
	}
	public Rect GetRect(Vector2Int pos){
		return new Rect(pos.x -(float)size.x/2, pos.y-(float)size.y/2, size.x, size.y);
	}
    public override BaseLevelStructure GetStructure() {
        var s = new BiomStructure();
        s.Init(this);
        return s;
    }
    public void Generate(LevelTilemap level,Vector2Int startPos)
    {
		//Rect rect = GetRect(startPos);
		//bool[,] canOverwrite = new bool[(int)rect.width,(int)rect.height];
		//for (int x = 0; x < rect.width; x++) {
		//	for (int y = 0; y < rect.height; y++) {
		//		if (border.GetValue ((float)(x) / rect.width, (float)(y) / rect.height) > borderBlend)
		//			canOverwrite [x, y] = true;
		//		else
		//			canOverwrite [x, y] = false;
		//	}
		//}
		//for (int i = 0; i < wallData.Count; i++)
        //{
		//	wallData[i].Generate(rect,level,canOverwrite);
        //}
        //for (int i = 0; i < structure.Count; i++)
        //{
		//	structure[i].Generate(rect,level,canOverwrite);
        //}
		////Plants
		//List<Vector2Int>[] lastPlantPos = new List<Vector2Int>[plants.Count];
		//for (int i = 0; i < lastPlantPos.Length; i++) {
		//	lastPlantPos[i] = new List<Vector2Int> ();
		//}
		//for (int x = 0; x < level.sizeX; x++) {
		//	for (int y = 0; y < level.sizeY; y++) {
		//		for (int i = 0; i < plants.Count; i++) {
		//			Vector2Int pos = new Vector2Int(x, y);
		//			if (OutOfMinDisOfAlreadyUsed(lastPlantPos[i],pos,plants[i].minDistance) == false)
		//				continue;
		//			if (plants[i].placeOnWall && level.IsOnWall(x, y) == false)
		//				continue;
		//			
		//			lastPlantPos[i].Add(pos);
		//			break;
		//		}
		//	}
		//}
		//for (int i = 0; i < plants.Count; i++) {
		//	for (int p = 0; p < lastPlantPos[i].Count; p++) {
		//		level.OverrideTile(lastPlantPos[i][p], plants[i].tile);
		//	}
		//}

    }
	bool OutOfMinDisOfAlreadyUsed(List<Vector2Int> alreadyUsePos,Vector2Int pos,float minDistance){
		for (int i = 0; i < alreadyUsePos.Count; i++) {
			if (Vector2Int.Distance(pos, alreadyUsePos[i]) <minDistance)
				return false;
		}
		return true;
	}
}
