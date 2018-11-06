using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    private new CameraMovement camera;
    private GameObject player;
    private float distance;

	void Start ()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
        distance = transform.position.z - player.transform.position.z;
	}
	
	void Update ()
    {
        if (camera.Following)
        {
            transform.position = new Vector3(0, 0, player.transform.position.z + distance);
            transform.rotation = camera.transform.rotation;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
            
    }
}
