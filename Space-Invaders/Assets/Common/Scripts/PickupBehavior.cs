using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    private GameObject letter;

	void Start ()
    {
        letter = transform.FindChild("Letter").gameObject;
	}
	
	void Update ()
    {
        transform.Rotate(0, 315 * Time.deltaTime, 0);
        letter.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerTrigger")
        {
            Destroy(gameObject);
        }
    }
}
