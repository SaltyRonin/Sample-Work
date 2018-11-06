using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollapse : MonoBehaviour
{
    private Rigidbody[] pieces;
    private List<bool> colliders;
    bool collapsed = false;

	void Start ()
    {
        pieces = GetComponentsInChildren<Rigidbody>();
        colliders = new List<bool>(pieces.Length);

        for(int i = 0; i < colliders.Capacity; i++)
        {
            pieces[i].isKinematic = true;
            //colliders[i] = pieces[i].isTrigger;
            //pieces[i].isTrigger = true;
        }
    }
	
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(!collapsed && other.tag == "PlayerTrigger")
        {
            for (int i = 0; i < colliders.Capacity; i++)
            {
                pieces[i].isKinematic = false;
                //colliders[i] = pieces[i].isTrigger;
                //pieces[i].isTrigger = true;
            }

            Destroy(gameObject, 8f);
            collapsed = true;
        }
    }
}
