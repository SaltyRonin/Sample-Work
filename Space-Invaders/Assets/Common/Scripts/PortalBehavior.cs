using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    private bool disappearing = false;
    private float time = 0f;
    private Vector3 speed = new Vector3();

	void Update ()
    {
		if(disappearing)
        {
            time += Time.deltaTime;
            if(time >= 0.5f)
            {
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.zero, ref speed, 0.15f);
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerTrigger")
        {
            disappearing = true;
            Destroy(gameObject, 2f);
        }
    }
}
