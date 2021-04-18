using System;
using System.Collections.Generic;
using Types;
using UnityEngine;
[CreateAssetMenu(fileName = "ColorMaterialMap",menuName = "Furious/StackUp/Global Color Map")]
public class ColorMaterialMap : ScriptableObject
{
  public List<ColorMat> ColorMaps;
}
[Serializable]
public class ColorMat{
    public DiscColors name;
    public Material material;
}

