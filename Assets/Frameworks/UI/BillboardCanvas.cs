using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
	private Transform cameraTransform;
	private Quaternion originalRotation;
    void Start()
    {
		originalRotation = transform.rotation;
		cameraTransform = Camera.main.transform;
    }

    void Update()
    {
		transform.rotation = cameraTransform.rotation * originalRotation;
    }
}
