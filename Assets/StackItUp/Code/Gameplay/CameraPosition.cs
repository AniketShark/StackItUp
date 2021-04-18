using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPosition : MonoBehaviour
{
	[SerializeField]
	ObjectPlacement3D objectPlacer;
	public bool execute;
	public Vector3 padding;
	public GameObject min;
	public GameObject max;


	public void ShiftCamera()
	{
		transform.position = new Vector3(objectPlacer.GetMeanX(), transform.position.y, transform.position.z);
		bool visible = false;
		
		Camera.main.orthographicSize = 1.0f;
		min.transform.position = objectPlacer.ViewMin;
		max.transform.position = objectPlacer.ViewMax;
		while (!visible)
		{
			Vector3 screenPoint = Camera.main.WorldToViewportPoint(objectPlacer.ViewMax + padding);
			if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
			{
				visible = true;
			}
			else
			{
				Camera.main.orthographicSize += 0.1f;
			}
		}
	}

#if UNITY_EDITOR
	public void Update()
	{
		if(!Application.isPlaying && execute)
		{
			ShiftCamera();
			execute = false;
		}
	}
#endif
}
