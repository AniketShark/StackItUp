using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectPlacement3D : MonoBehaviour
{
	public int rows; //only xz
	public int columns;
	public int height;
	public LevelData Level;
	public Game game;

	[SerializeField]
	public Vector3 padding;
	[SerializeField]
	List<GameObject> objects;
	public bool execute;

	public Vector3 ViewMin;
	public Vector3 ViewMax;

	int rowsCount = 0;
	int columnsCount = 0;
	int heightCount = 0;
	int oddDiviser = 0;

	public void Execute(List<StackPin> stackPins)
	{
		int rowsCount = 0;
		int columnsCount = 0;
		int heightCount = 0;
		int oddDiviser = 0;
		ViewMax = Vector3.zero;
		ViewMin = Vector3.zero;
		bool oddNumberOfElements = false;

		if (stackPins.Count % 2 != 0)
		{
			oddNumberOfElements = true;
			oddDiviser = columns;
			while (oddDiviser % 2 == 0)
			{
				oddDiviser--;
			}
		}

		foreach (StackPin game in stackPins)
		{
			if (!game.gameObject.activeInHierarchy)
				continue;

			var objectBounds = game.GetComponent<MeshFilter>().sharedMesh.bounds;
			float objectHalfWidth = objectBounds.size.x * 0.5f;
			float objectHalfHeight = objectBounds.size.z * 0.5f;
			float nextX = (2 * columnsCount * objectHalfWidth) + (columnsCount * padding.x) + (oddDiviser > 0 ? objectHalfWidth : 0);
			float nextZ = (2 * rowsCount * objectHalfHeight) + (rowsCount * padding.z);
			float nextY = (heightCount * padding.y);
			game.transform.position = transform.position + (nextX * Vector3.right) + (nextZ * Vector3.forward) + (nextY * Vector3.up);

			columnsCount++;
			oddDiviser--;
			if (columnsCount % columns == 0 || (oddNumberOfElements && oddDiviser == 0))
			{
				columnsCount = 0;
				if (oddDiviser <= 0)
				{
					oddDiviser = 0;
					oddNumberOfElements = false;
				}

				heightCount++;
				rowsCount++;

				if (rowsCount > rows)
				{
					heightCount = 0;
					rowsCount = 0;
					break;
				}
			}

			if (game.transform.position.x > ViewMax.x)
			{
				ViewMax = game.transform.position + (Vector3.right * objectHalfWidth);
			}

			if (game.transform.position.x < ViewMin.x)
			{
				ViewMin = game.transform.position - (Vector3.right * objectHalfWidth);
			}
		}
	}

	public void Reset()
	{
		rowsCount = 0;
		columnsCount = 0;
		heightCount = 0;
		oddDiviser = 0;
	}

	public Vector3 GetNextPosition(GameObject gameobject)
	{
		Vector3 nextPosition = Vector3.zero;
		bool oddNumberOfElements = false;

		if (objects.Count % 2 != 0)
		{
			oddNumberOfElements = true;
			oddDiviser = columns;
			while (oddDiviser % 2 == 0)
			{
				oddDiviser--;
			}
		}


		var objectBounds = gameobject.GetComponent<MeshFilter>().sharedMesh.bounds;


		float objectHalfWidth = objectBounds.size.x * 0.5f;
		float objectHalfHeight = objectBounds.size.z * 0.5f;
		float nextX = (2 * columnsCount * objectHalfWidth) + (columnsCount * padding.x) + (oddDiviser > 0 ? objectHalfWidth : 0);
		float nextZ = (2 * rowsCount * objectHalfHeight) + (rowsCount * padding.z);
		float nextY = (heightCount * padding.y);
		nextPosition = transform.position + (nextX * Vector3.right) + (nextZ * Vector3.forward) + (nextY * Vector3.up);

		columnsCount++;
		oddDiviser--;
		if (columnsCount % columns == 0 || (oddNumberOfElements && oddDiviser == 0))
		{
			columnsCount = 0;
			rowsCount++;
			heightCount++;

			if (oddDiviser == 0)
				oddNumberOfElements = false;

			if (rowsCount > rows)
			{
				heightCount = 0;
				rowsCount = 0;
			}
		}
		if (nextPosition.x > ViewMax.x)
		{
			ViewMax = nextPosition + (Vector3.right * objectHalfWidth);
		}

		if (nextPosition.x < ViewMin.x)
		{
			ViewMin = nextPosition - (Vector3.right * objectHalfWidth);
		}

		return nextPosition;
	}

	public float GetMeanX()
	{
		return (ViewMax.x + ViewMin.x) * 0.5f;
	}

#if UNITY_EDITOR
	public void Update()
	{
		if (!Application.isPlaying && execute)
		{
			Execute(game.stackPins);
			execute = false;
		}
	}
#endif
}
