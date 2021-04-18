using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid3D : MonoBehaviour
{
	[SerializeField]
	private Vector2 fixedRowsAndColumns;
	[SerializeField]
	private Vector3 gridSize = Vector3.one;
	[SerializeField]
	private Vector3 cellSize = Vector3.one;
	[SerializeField]
	public Vector3 Size { get { return cellSize; } }

	public void Execute()
	{

	}

	public Vector3 GetNearestPointOnGrid(Vector3 position)
	{
		position -= transform.position;

		int xCount = Mathf.RoundToInt(position.x / cellSize.x);
		int yCount = Mathf.RoundToInt(position.y / cellSize.y);
		int zCount = Mathf.RoundToInt(position.z / cellSize.z);

		Vector3 result = new Vector3(
				(float)xCount * cellSize.x,
				(float)yCount * cellSize.y,
				(float)zCount * cellSize.z
			);

		result += transform.position;

		return result;
	}

	public void PlaceGameObjects(List<StackPin> gameObjects)
	{
		Vector3 initalPoint = Vector3.zero;
		int rowCount = 1;
		int columnCount = 1;
		for(int i = 0;i < gameObjects.Count;i++)
		{
			var point = GetNearestPointOnGrid(initalPoint);
			gameObjects[i].transform.position = point;

			if((rowCount % (int)fixedRowsAndColumns.x) == 0)
			{
				initalPoint.x += cellSize.x;
				initalPoint.z = 0;
			}
			else
			{
				initalPoint.z += cellSize.z;
				//initalPoint.y += cellSize.y;
			}
			rowCount++;
		}
	}


	private void OnDrawGizmos()
	{
		cellSize = cellSize.magnitude <= 0 ? Vector3.one : cellSize;

		Gizmos.color = Color.yellow;
		
		for (float x = 0; x < gridSize.x; x += cellSize.x)
		{
			for (float z = 0; z < gridSize.z; z += cellSize.z)
			{
				var point = GetNearestPointOnGrid(new Vector3(x, 0, z));
			}
		}
	}

}
