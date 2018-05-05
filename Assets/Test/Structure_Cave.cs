using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StructureTunnel
{
    
    public Vector2Int start;
    public Vector2Int end;
    public int width;
    public int length;
    public Vector2 dir;
    public StructureTunnel(Vector2Int start, Vector2Int end, int width)
    {
        this.start = start;
        this.end = end;
        this.width = width;
        dir = (end - start);
        length = (int)(end - start).magnitude;
        dir.Normalize();
    }
	public void GenerateTunnel(Rect rect,LevelTilemap level, BaseLevelStructureData data, Texture2D noise, Texture2D tunnelTex, LevelTileData tileData,bool[,] canOverwrite)
    {
        for (float x = 0; x < length; x += .5f)
        {
            for (int y = 0; y < width; y++)
            {
                //int posX = tunnel.start.x + (int)tunnel.dir.x * x + (int)tunnel.dir.Rotate(90).x * x - tunnel.width / 2;
                //int posY = tunnel.start.y + (int)tunnel.dir.y * y + (int)tunnel.dir.Rotate(90).y * y - tunnel.width / 2;
                Vector2 pos = new Vector2(start.x, start.y);
                pos += dir * x;
                pos += dir.Rotate(90) * y;
                int posX = (int)pos.x;
                int posY = (int)pos.y;
                float noiseValue = data.ReadTexture(noise, (float)posX / level.sizeX * 20, (float)posY / level.sizeY * 20);
                float tunnelValue = data.ReadTexture(tunnelTex, (float)x / length, (float)y / width);
                float value = tunnelValue;
				if (level.IsPosOutOfBound(posX, posY) || rect.Contains(new Vector2(posX,posY)) == false)
					return;
                if (value > .5f)
                {
					canOverwrite[posX-(int)rect.x, posY-(int)rect.y] = false;
                    //level.GetITile(posX, posY).OverrideData(level,tileData);
                }
            }
        }
    }
	public bool TryIfCanGenerateTunnel(Rect rect , LevelTilemap level, BaseLevelStructureData data, Texture2D noise, Texture2D tunnelTex, LevelTileData tileData,bool[,] canOverwrite)
	{
		for (float x = 0; x < length; x += .5f)
		{
			for (int y = 0; y < width; y++)
			{
				//int posX = tunnel.start.x + (int)tunnel.dir.x * x + (int)tunnel.dir.Rotate(90).x * x - tunnel.width / 2;
				//int posY = tunnel.start.y + (int)tunnel.dir.y * y + (int)tunnel.dir.Rotate(90).y * y - tunnel.width / 2;
				Vector2 pos = new Vector2(start.x, start.y);
				pos += dir * x;
				pos += dir.Rotate(90) * y;
				int posX = (int)pos.x;
				int posY = (int)pos.y;
				float noiseValue = data.ReadTexture(noise, (float)posX / level.sizeX * 20, (float)posY / level.sizeY * 20);
				float tunnelValue = data.ReadTexture(tunnelTex, (float)x / length, (float)y / width);
				float value = tunnelValue;
				if (level.IsPosOutOfBound(posX, posY) || rect.Contains(new Vector2(posX,posY)) == false)
					return false;
				if (value > .5f && canOverwrite[posX-(int)rect.x, posY-(int)rect.y] == false)
					return false;
				
			}
		}
		return true;
	}
    public Vector2Int GetRandomPos()
    {
        Vector2 pos = start + dir * Random.Range(0, length);
        return new Vector2Int((int)pos.x,(int)pos.y);
    }
    
}

public class StructureCave{

    int sizeX;
    int sizeY;
    Vector2Int pos;
    public StructureCave(int sizeX, int sizeY,Vector2Int pos)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.pos = pos;
    }
	public void Generate(Rect rect,LevelTilemap level, BaseLevelStructureData data, Texture2D noise, Texture2D caveTex, LevelTileData tileData,bool[,] canOverwrite)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int posX = x - sizeX / 2 + pos.x;
                int posY = y - sizeY / 2 + pos.y;
                float caveValue = data.ReadTexture(caveTex, (float)x / sizeX, (float)y / sizeY);
                float value = caveValue;
				if (level.IsPosOutOfBound(posX, posY) || rect.Contains(new Vector2(posX,posY)) == false)
					return;
                if (value > .5f)
                {
					canOverwrite [posX-(int)rect.x, posY-(int)rect.y] = false;
                    //level.GetTile(posX, posY).OverrideData(level,tileData);
                }
            }
        }
    }
	public bool CheckIfCanGenerate(Rect rect, LevelTilemap level, BaseLevelStructureData data, Texture2D noise, Texture2D caveTex, LevelTileData tileData,bool[,] canOverwrite)
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				int posX = x - sizeX / 2 + pos.x;
				int posY = y - sizeY / 2 + pos.y;
				float caveValue = data.ReadTexture(caveTex, (float)x / sizeX, (float)y / sizeY);
				float value = caveValue;
				if (level.IsPosOutOfBound(posX, posY)|| rect.Contains(new Vector2(posX,posY)) == false)
					return false;
				if (value > .5f && canOverwrite [posX-(int)rect.x, posY-(int)rect.y] == false)
					return false;
			}
		}
		return true;
	}
}
[CreateAssetMenu(menuName = "Data/LevelGeneration/Structure/Cave")]
public class Structure_Cave : BaseLevelStructureData
{
    /*
    public Texture2D noise;
    public Texture2D tunnelTex;
    public Texture2D caveTex;
    public TileLevelData tileData;

    public Vector2Int tunnelSegmentWidth;
    public Vector2Int tunnelSegmentLenght;

    public Vector2Int tunnelNumber;
    public Vector2 tunnelRandomAngle;
    public int borderPadding;

    public Vector2Int caveNumber;
    public Vector2Int caveRandomSize;
	public override bool TryToGenerate (Rect rect,LevelTilemap level,bool[,] canOverwrite)
	{
		int number = Random.Range(tunnelNumber.x, tunnelNumber.y);
		List<StructureTunnel> tunnelList = new List<StructureTunnel>();

		Vector2Int start = new Vector2Int((int)Random.Range(rect.x + borderPadding, rect.xMax-borderPadding), (int)Random.Range(rect.y + borderPadding, rect.yMax-borderPadding));
		Vector2 dir = new Vector2(Random.Range(tunnelSegmentLenght.x, tunnelSegmentLenght.y), 0);
		float angle = Random.Range(0, 360);
		dir = dir.Rotate(angle);
		Vector2Int end = start + new Vector2Int((int)dir.x, (int)dir.y);


		for (int i = 0; i < number; i++)
		{
			tunnelList.Add(new StructureTunnel(start, end, tunnelSegmentWidth.y));
			start = end;
			dir = new Vector2(Random.Range(tunnelSegmentLenght.x, tunnelSegmentLenght.y), 0);
			angle += Random.Range(tunnelRandomAngle.x, tunnelRandomAngle.y) * Mathf.Sign(Random.Range(-1,1));
			dir = dir.Rotate(angle);
			start -= new Vector2Int((int)(dir.normalized*tunnelSegmentWidth.x).x, (int)(dir.normalized * tunnelSegmentWidth.x).y);
			end = start + new Vector2Int((int)dir.x, (int)dir.y);
		}
		var tunnelCaveList =new List<StructureTunnel>(tunnelList);
		var CaveList = new List<StructureCave>();
		int caveNum = Random.Range(caveNumber.x, caveNumber.y);

		for (int i = 0; i < caveNum; i++)
		{
			int randomTunnel = Random.Range(0, tunnelCaveList.Count);
			Vector2Int pos = tunnelCaveList[randomTunnel].GetRandomPos();
			tunnelCaveList.RemoveAt(randomTunnel);
			CaveList.Add(new StructureCave(Random.Range(caveRandomSize.x, caveRandomSize.y), Random.Range(caveRandomSize.x, caveRandomSize.y), pos));
		}

		for (int i = 0; i < tunnelList.Count; i++)
		{
			if (tunnelList [i].TryIfCanGenerateTunnel(rect,level, this, noise, tunnelTex, tileData,canOverwrite) == false)
				return false;
		}
		for (int i = 0; i < CaveList.Count; i++)
		{
			if (CaveList [i].CheckIfCanGenerate(rect,level, this, noise, caveTex, tileData,canOverwrite) == false)
				return false;
		}


		//GenerateForReal
		for (int i = 0; i < tunnelList.Count; i++)
		{
			tunnelList[i].GenerateTunnel(rect,level,this,noise,tunnelTex,tileData,canOverwrite );
		}
		for (int i = 0; i < CaveList.Count; i++)
		{
			CaveList[i].Generate(rect,level, this, noise, caveTex, tileData,canOverwrite);
		}



		return true;
	}
    */
}

