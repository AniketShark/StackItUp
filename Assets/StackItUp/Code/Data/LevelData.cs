using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public struct PinConfig : ICloneable
{
	//public Color color;
	public int pinIndex;
	public List<TileInfo> tiles;

    public PinConfig(int index)
    {
        pinIndex = index;
        tiles = new List<TileInfo>();
    }

    public PinConfig(int index, List<TileInfo> _tiles)
    {
        pinIndex = index;
        tiles = _tiles;
    }

    public void AddTile(TileInfo tile)
    {
        if (tiles == null)
            tiles = new List<TileInfo>();
        tiles.Add(tile);
    }

    public object Clone()
    {
        var clone = new PinConfig();
        clone.pinIndex = pinIndex;
        clone.tiles = new List<TileInfo>(tiles.ToArray());
        return clone;
    }

    public void RemoveTile(TileInfo _tile)
    {
        //for(int j = tiles.Count; j <= 0; j--)
        //{
        //    if (tiles[j].colorIndex == _tile.colorIndex && tiles[j].size == _tile.size)
        //        tiles.RemoveAt(j);
        //}

        tiles.Remove(_tile);
    }
}

[System.Serializable]
public struct TileInfo
{
	//public int stackIndex;
	public int colorIndex;
	public int size;

    public TileInfo(int colorIndex, int size)
    {
        this.colorIndex = colorIndex;
        this.size = size;
    }

    public override string ToString()
    {
        return "c" + colorIndex + "s"+size;
    }
}

[CreateAssetMenu(fileName = "Stack", menuName = "Furious/ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
	//public StackData stack;
	public int stackCount;
	public List<int> sequence;
	public List<Material> colors;
	public int pins;
	public List<PinConfig> pinConfig = new List<PinConfig>();

    private const string FORMATTER = "{0}:{1}#";
    public string GetHashKey()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(string.Format(FORMATTER, "st" ,stackCount));
        builder.Append(string.Format(FORMATTER, "pins", pins));
        colors.Sort((l,r)=> {
            if (l.color.r != r.color.r)
                return l.color.r.CompareTo(r.color.r);
            if (l.color.g != r.color.g)
                return l.color.g.CompareTo(r.color.g);
             if (l.color.b != r.color.b)
                return l.color.b.CompareTo(r.color.b);
            return 0;
        });
        for (int i = 0;i < colors.Count; i++)
        {
            builder.Append(string.Format(FORMATTER, "cl"+i, colors[i].color.ToString()));
        }
        pinConfig.Sort((l,r)=> {
            return l.pinIndex.CompareTo(r.pinIndex);
        });
        for (int i = 0; i < pinConfig.Count; i++)
        {
            string pinData = "";
            for(int j = 0; j < pinConfig[i].tiles.Count; j++)
            {
                pinData = pinData +"c"+ pinConfig[i].tiles[j].colorIndex + "s" + pinData + pinConfig[i].tiles[j].size+",";
            }
            builder.Append(string.Format(FORMATTER, "p" + i, pinData));
        }
        return builder.ToString();
    }

    public static LevelData TransformToLevelData(LevelsData.Data data)
    {
        LevelData levelData = new LevelData();
        levelData.pins = data.pins;
        levelData.stackCount = data.colors;
        levelData.pinConfig = new List<PinConfig>();
       
        for (int i = 0; i < data.PinConfigs.Count; i++)
        {
             levelData.pinConfig.Add((PinConfig)data.PinConfigs[i].Clone());
        }
        ColorMaterialMap colorMappingList = Resources.Load<ColorMaterialMap>("ColorMaterialMapData");
        levelData.colors = new List<Material>();

        for (int i = 0; i < data.colors; i++)
        {
            levelData.colors.Add(colorMappingList.ColorMaps[i].material);
        }
        levelData.sequence = new List<int>();
        for (int i = 0; i < data.tileSize; i++)
        {
            levelData.sequence.Add(i+1);
        }

        return levelData;
    }
}
