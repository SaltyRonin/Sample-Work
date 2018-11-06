using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.75f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Event" || collider.tag == "Shard" || collider.tag.Contains("Laser")) return;

        if (tag.Contains("Enemy")) // I'm an enemy laser
        {
            if (!collider.gameObject.tag.Contains("Enemy")) // and I'm not hitting an enemy
                Destroy(gameObject);
        }
        else if(!collider.tag.Contains("Trigger")) // I'm a player laser and I'm hitting an actual enemy
        {
            if (!collider.gameObject.tag.Contains("Player")) // I'm not hitting myself
                Destroy(gameObject);
        }
    }
}
