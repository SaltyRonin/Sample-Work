using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trollface : EnemyBehavior
{
    [Range(0, 3)] public int StartPos = 0;

    private SphereCollider LaserTrig;
    private Vector3[] pos = new Vector3[4];
    private int posIndex;
    private Vector3 moveVelocity = new Vector3();
    private Vector3 laughPos = new Vector3();
    private float time = 0f;
    private bool laugh = false;
    private int laughCount = 0;

    public override void Active()
    {
        time += Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, laughPos, ref moveVelocity, 0.05f);
        Vector3 position = transform.position;
        position.z = 40;
        Position = position;

        if (laugh)
        {
            if(time >= 0.1f)
            {
                laugh = false;
                laughPos = transform.position + new Vector3(0, 1.5f, 0);
                time = 0f;
                laughCount++;
            }
        }
        else
        {
            if (time >= 0.1f)
            {
                laugh = true;
                laughPos = transform.position + new Vector3(0, -1.5f, 0);
                time = 0f;
                laughCount++;
            }
        }

        if(laughCount > 3)
        {
            laughCount = 0;
            State = EnemyState.Inactive;
            LaserTrig.enabled = true;
            time = 0f;
        }
    }

    public override void Dead()
    {
        
    }

    public override void EnemyStart()
    {
        posIndex = StartPos;
        pos[0] = new Vector3(-10, 10, transform.position.z);
        pos[1] = new Vector3(10, 10, transform.position.z);
        pos[2] = new Vector3(10, -10, transform.position.z);
        pos[3] = new Vector3(-10, -10, transform.position.z);

        LaserTrig = GetComponentInChildren<SphereCollider>();
    }

    public override void Hit(Collider other)
    {
        
    }

    public override void OnTrigger(Collider other)
    {
        LaserTriggerable = true;
        renderer.enabled = true;
        float rand = UnityEngine.Random.Range(0, 3);
        if(posIndex + (int)rand % 4 == posIndex)
        {
            posIndex = (posIndex + 1) % 4;
        }
        else
        {
            posIndex = (posIndex + (int)rand) % 4;
        }

        Position = pos[posIndex];
    }

    public override void Triggered()
    {
        time += Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, pos[posIndex], ref moveVelocity, 0.1f);
        Vector3 position = transform.position;
        position.z = 40;
        Position = position;

        if (time >= 0.5f)
        {
            laughPos = transform.position + new Vector3(0, 1.5f, 0);
            State = EnemyState.Active;
            LaserTrig.enabled = false;
            moveVelocity = Vector3.zero;
            time = 0f;
        }
    }
}
