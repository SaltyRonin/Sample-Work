using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Range(-720f, 720f)] public float Speed = -1f;
	void Update ()
    {
        transform.Rotate(0, 0, Speed * Time.deltaTime);
	}
}
