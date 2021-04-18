using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVSetter : MonoBehaviour
{
	public List<MeshRenderer> rendererList;
	private Vector2 viewPoint;

	public void CheckFov()
	{
		Camera.main.orthographicSize = 12;
		if(rendererList[0].gameObject.activeInHierarchy)
		{
			//StartCoroutine(UpdateFOV());
			UpdateFOVImmidiate();
		}
		foreach (MeshRenderer renderer in rendererList)
		{
			renderer.enabled = false;
		}
	}

	void UpdateFOVImmidiate()
	{
		//Debug.LogError("Finding FOV");

		while (true)
		{
			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(rendererList[0].transform.position);
			viewPoint.x = viewportPoint.x;
			viewPoint.y = viewportPoint.y;
			if (Camera.main.rect.Contains(viewPoint))
			{
				break;
			}
			else
			{
				Camera.main.orthographicSize += 0.1f;
			}
		}

		//Debug.LogError("Found FOV");
	}
	IEnumerator UpdateFOV()
	{
		//Debug.LogError("Finding FOV");

		while (true)
		{
			Vector3 viewportPoint = Camera.main.WorldToViewportPoint(rendererList[0].transform.position);
			viewPoint.x = viewportPoint.x;
			viewPoint.y = viewportPoint.y;
			if(Camera.main.rect.Contains(viewPoint))
			{
				break;
			}
			else
			{
				Camera.main.orthographicSize += 0.1f;
			}
			yield return null;
		}

		//Debug.LogError("Found FOV");
	}
}
