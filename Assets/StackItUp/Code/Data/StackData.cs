using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StackIndex
{
	public int index;
	public Mesh mesh;
}

[CreateAssetMenu(fileName = "Stack", menuName = "Furious/ScriptableObjects/Stack", order = 1)]

public class StackData : ScriptableObject
{
	public List<StackIndex> meshes;
	public List<Material> materials;
	public Mesh stackBase;
	public Mesh pinMesh;
	public Material stackBaseMaterial;
	
}
