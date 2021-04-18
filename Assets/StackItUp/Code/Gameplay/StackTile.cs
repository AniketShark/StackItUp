using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class StackTile : MonoBehaviour
{
	public int colorCode;
	public int index;
	public Color color;
	public int pinIndex;
	public StackPin pin;

	public MeshFilter cachedMesh;
	public MeshRenderer cachedMeshRenderer;
	public StackData stackData;

	public void SetData(StackData data)
	{
		stackData = data;
	}

	public void SetMesh(Mesh mesh)
	{
		cachedMesh.sharedMesh = mesh;
	}

	public void SetMaterials(Material[] materials)
	{
		cachedMeshRenderer.sharedMaterials = materials;
	}

	public void SetMaterialColor(Material color)
	{
		cachedMeshRenderer.sharedMaterial = color;
		//this.color = color;
		//MaterialPropertyBlock block = new MaterialPropertyBlock();
		//block.SetColor("_Color", color);
		//cachedMeshRenderer.SetPropertyBlock(block,0);
	}

	public void PopTile(StackPin stackPin)
	{
		transform.DOLocalMove(stackPin.GetEntryPoint(),0.1f);
	}

	public void PushTile(StackPin stackPin)
	{
		pin = stackPin;
		var position = stackPin.CalculateNextPosition(gameObject);
		var sequence = DOTween.Sequence();
		if (!stackPin.pinIndex.Equals(pinIndex))
		{
			sequence.Append(transform.DOLocalMove(stackPin.GetEntryPoint(), 0.2f));
		}
		sequence.Append(transform.DOLocalMove(position, 0.1f));
		sequence.Play();
	}

	public void PushInvalid()
	{
		pin.PushTile(gameObject);
	}
}
