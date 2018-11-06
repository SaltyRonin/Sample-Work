using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThomasHoarde : MonoBehaviour
{
    private AudioSource primary, secondary;
    private bool triggered = false;

    void Start()
    {
        primary = transform.Find("PrimaryAudio").GetComponent<AudioSource>();
        secondary = transform.Find("SecondaryAudio").GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!triggered && other.tag == "PlayerTrigger")
        {
            triggered = true;
            primary.Play();
            secondary.Play();
            Destroy(gameObject, 30f);
        }
    }

    private void Update()
    {
        if(triggered)
        {
            transform.Translate(0, 0, 15f * Time.deltaTime);
        }
    }
}
