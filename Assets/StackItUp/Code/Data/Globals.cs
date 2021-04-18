using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Globals", menuName = "Furious/ScriptableObjects/Globals", order = 0)]

[System.Serializable]
public class Pattern
{
	public string name;
	public Texture2D patternTexture;
}

public class Globals : ScriptableObject
{
	public int totalLevels;
	public int maxStackSize;
	public List<Material> allMaterials;
	public List<Pattern> allPatterns;

}
