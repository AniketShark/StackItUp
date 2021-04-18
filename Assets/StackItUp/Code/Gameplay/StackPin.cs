using DG.Tweening;
using FuriousPlay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StackPin : MonoBehaviour,IPointerUpHandler,IPointerDownHandler,IPointerClickHandler
{
	public static GameObject SelectedTile;
	public  GameObject pin;
	//public  ParticleSystem explosion;
	public  int pinIndex;
	public  bool celebrationDone;
	public  LevelData levelData;
	public  StackData stackData;
	private Bounds  baseBounds;
	private Bounds  pinBounds;
	private Vector3 entryPoint;
	private Vector3 startPoint;
	private Vector3 nextPosition;
	private bool stackLoadComplete;

	private Stack<GameObject> _stack;
	public Stack<GameObject> stack { get { if (_stack == null) _stack = new Stack<GameObject>(); return _stack; } }

	public void Init()
	{
        GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<MeshFilter>().sharedMesh = stackData.stackBase;
		gameObject.GetComponent<MeshRenderer>().sharedMaterial= stackData.stackBaseMaterial;
		pin.GetComponent<MeshFilter>().sharedMesh = stackData.pinMesh;
		pin.GetComponent<MeshRenderer>().sharedMaterial = stackData.stackBaseMaterial;
		baseBounds = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
		pinBounds  = pin.GetComponent<MeshFilter>().sharedMesh.bounds;
		pin.transform.position = transform.position;
		startPoint = transform.InverseTransformPoint(transform.position     + (Vector3.up * baseBounds.extents.y));
		entryPoint = transform.InverseTransformPoint(pin.transform.position + (Vector3.up * pinBounds.size.y));
		celebrationDone = true;
	}

	private void Awake()
	{
		ActionManager.SubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE,StackLoadComplete);
	}

	private void OnDestroy()
	{
		ActionManager.UnsubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE,StackLoadComplete);
	}

	private void StackLoadComplete(Hashtable parameters)
	{
		stackLoadComplete = true;
	}
	
	public void StackChanged()
	{
		//var stackTile = tile.GetComponent<StackTile>();
		//stackTile.SetData(stack);
		//stackTile.colorCode = tileInfo.colorIndex;
		//stackTile.index = tileInfo.size;
		//stackTile.SetMesh(stack.meshes[tileInfo.size - 1].mesh);
		//stackTile.SetMaterials(stack.materials.ToArray());
		//stackTile.SetMaterialColor(colors[tileInfo.colorIndex]);

		foreach (GameObject tile in stack)
		{
			var stackTile = tile.GetComponent<StackTile>();
			stackTile.SetData(stackData);
			stackTile.SetMesh(stackData.meshes[stackTile.index - 1].mesh);
			stackTile.SetMaterials(stackData.materials.ToArray());
			stackTile.SetMaterialColor(levelData.colors[stackTile.colorCode]);
		}
	}

	public void UpdateSelectedTile()
	{
		if(SelectedTile != null)
		{
			var stackTile = SelectedTile.GetComponent<StackTile>();
			stackTile.SetData(stackData);
			stackTile.SetMesh(stackData.meshes[stackTile.index - 1].mesh);
			stackTile.SetMaterials(stackData.materials.ToArray());
			stackTile.SetMaterialColor(levelData.colors[stackTile.colorCode]);
		}
	}

	

	public void PopTile()
	{
		try
		{
			var tile = stack.Pop();
			tile.GetComponent<StackTile>().PopTile(this);
			SelectedTile = tile;
		}
		catch(Exception exception)
		{
			//Debug.LogError("No More tiles in " + gameObject.name);
		}
	}

	public void PushTile(GameObject tile)
	{
		if (!PushValid(CalculateNextPosition(tile),tile))
		{
			tile.GetComponent<StackTile>().PushInvalid();
			return;
		}

		stack.Push(tile);
		tile.transform.parent = transform;
		tile.transform.localEulerAngles = Vector3.zero;
		tile.transform.localScale = Vector3.one;
		tile.GetComponent<StackTile>().PushTile(this);
		tile.GetComponent<StackTile>().pinIndex = pinIndex;
		SelectedTile = null;

		if (stackLoadComplete)
		{
			ActionManager.TriggerEvent(GameEvents.CHECK_COMPLETE);
		}
	}

	public bool CheckComplete()
	{
		if (stack.Count <= 0)
		{
			return false;
		}

		bool sameColor = true;
		bool currectSequence = true;
		var stackTile = stack.Peek().GetComponent<StackTile>();
		int colorCode = stackTile.colorCode;
		int index = stackTile.index;
		List<int> sequence = levelData.sequence;

		if (index != 1 || stack.Count != levelData.sequence.Count)
		{
			return false;
		}

		int count = 0;
		foreach(GameObject tile in stack)
		{
			sameColor &= colorCode.Equals(tile.GetComponent<StackTile>().colorCode); //check for same color tiles
			currectSequence &= sequence[count].Equals(tile.GetComponent<StackTile>().index);// check for currect sequence

			if (!sameColor)
			{
				//Debug.LogError("2> stack not in same color "+ gameObject.name);
				return false;
			}
			else if (!currectSequence)
			{
				//Debug.LogError("3> stack not in sequence " + gameObject.name);
				return false;
			}
			count++;
		}
		//Debug.LogError("4> stack complete " + gameObject.name);
		return true;
	}

	public void Reposition()
	{
		int count = stack.Count - 1;
		
		foreach (GameObject tile in stack)
		{
			float tileHeight = tile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
			float height = (2 * count * tileHeight) + tileHeight;
			tile.transform.localPosition = startPoint + (height * Vector3.up);
			count--;
		}
	}

	public void Celebrate()
	{
		int count = 0;
		foreach(GameObject tile in stack)
		{
			tile.transform.DOLocalRotate(Vector3.up * 10, 1, RotateMode.LocalAxisAdd);
			if(count == stack.Count - 1)
				tile.transform.DOScale(Vector3.one * 0.1f, 1);
			else
				tile.transform.DOScale(Vector3.one * 0.01f, 1).OnComplete(CelebrationComplete);
		}
	}

	public void RestoreToPool(List<GameObject> pool,Transform poolParent)
	{
		if (SelectedTile != null)
		{
			SelectedTile .transform.parent = poolParent;
			SelectedTile .GetComponent<MeshFilter>().sharedMesh = null;
			SelectedTile.GetComponent<MeshRenderer>().sharedMaterial = null;
			pool.Add(SelectedTile);
			SelectedTile = null;
		}

		foreach (GameObject tile in stack)
		{
			tile.transform.parent = poolParent;
			tile.GetComponent<MeshFilter>().sharedMesh = null;
			tile.GetComponent<MeshRenderer>().sharedMaterial = null;
			pool.Add(tile);
		}
	}

	public void CelebrationComplete()
	{
		//explosion.Play(true);
	}

	public void Reset()
	{
		stack.Clear();
		stackLoadComplete = false;
		SelectedTile = null;
		celebrationDone = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		//Debug.Log(gameObject.name + ": I was Clicked!");
		//if (SelectedTile != null)
		//{
		//	PushTile(SelectedTile);
		//}
		//else
		//{
		//	PopTile();
		//}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//Debug.Log(gameObject.name + ": I was Up!");
		if (SelectedTile != null)
		{
			PushTile(SelectedTile);
		}
		else
		{
			PopTile();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log(gameObject.name + ": I was Down!");

	}

	#region Math

	public Vector3 CalculateNextPosition(GameObject tile)
	{
		float tileHeight = tile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
		float height = (2 * (stack.Count - 1)* tileHeight) + tileHeight;
		nextPosition = startPoint + (height * Vector3.up);
		return nextPosition;
	}

	public Vector3 GetEntryPoint()
	{
		return entryPoint;
	}

	public bool PushValid(Vector3 nextPostion,GameObject tile)
	{
		float tileHeight = tile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
		float diff = (entryPoint.y - (tileHeight * 3));
		return nextPosition.y < diff;
	}
	
	#endregion

}
