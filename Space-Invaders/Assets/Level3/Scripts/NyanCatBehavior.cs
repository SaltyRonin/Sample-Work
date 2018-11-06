using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyanCatBehavior : MonoBehaviour
{
    public float Speed = 30f;
	
	void Update ()
    {
        transform.RotateAround(transform.parent.position, transform.forward, Time.deltaTime * -Speed);
	}
}
