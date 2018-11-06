using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCameraBehavior : MonoBehaviour
{
    private Camera parentCamera;
    private new Camera camera;

	void Start ()
    {
        camera = GetComponent<Camera>();
        parentCamera = transform.parent.GetComponent<Camera>();
	}
	
	void LateUpdate ()
    {
        camera.fieldOfView = parentCamera.fieldOfView;
	}
}
