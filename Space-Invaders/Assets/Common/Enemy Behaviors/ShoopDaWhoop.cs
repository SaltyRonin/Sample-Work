using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoopDaWhoop : EnemyBehavior
{
    private GameObject laser;
    private float time = 0f;
    private bool fired = false, looking = false, playing = false;

    public override void EnemyStart()
    {
        laser = transform.FindChild("Laser").gameObject;
    }

    public override void Triggered()
    {
        
    }

    public override void Active()
    {
        time += Time.deltaTime;

        if(!fired)
        {
            if (time >= 4f)
            {
                laser.SetActive(true);
                fired = true;
            }
            else if (time >= 3.75f && !looking)
            {
                laser.transform.LookAt(ActualPlayerPosition);
                looking = true;
            }
            else if (time >= 1f && !playing)
            {
                primaryAudio.Play();
                playing = true;
            }
        }
        else
        {
            if (time >= 12f)
            {
                Destroy(gameObject);
            }
            else if (time >= 7f)
            {
                Following = false;
            }
            else if (time > 6f && laser.activeSelf)
            {
                laser.SetActive(false);
                CameraMovement.Shake = false;
            }
            else if (laser.activeSelf)
            {
                CameraMovement.Shake = true;
            }
        }
    }

    public override void Dead()
    {
        CameraMovement.Shake = false;
    }

    public override void Hit(Collider other)
    {
        
    }

    public override void OnTrigger(Collider other)
    {
        renderer.enabled = true;
        State = EnemyState.Active;
    }
}
