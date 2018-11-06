using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thomas : EnemyBehavior
{
    public bool Randomize = true;

    private float time = 0f;
    private GameObject target;

    public override void Active()
    {
        time += Time.deltaTime;

        Quaternion orig = transform.rotation;
        transform.LookAt(target.transform);
        Quaternion look = transform.rotation;

        transform.rotation = Quaternion.Lerp(orig, look, 6f * Time.deltaTime);

        transform.position += transform.forward * (15f * Time.deltaTime);

        if ((transform.position - target.transform.position).magnitude <= 2f)
        {
            transform.position = target.transform.position;
            State = EnemyState.Dead;
        }

        if (time >= 6f)
        {
            Destroy(gameObject);
        }
    }

    public override void Dead()
    {

    }

    public override void EnemyStart()
    {
        renderer.enabled = true;

        if (Randomize)
        {
            Vector3 pos = new Vector3();
            pos.x = UnityEngine.Random.Range(-6f, 6f);
            pos.y = UnityEngine.Random.Range(-4f, 4f);
            pos.z = UnityEngine.Random.Range(-8f, 8f);

            transform.position += pos;
            if (transform.position.y < -12.84f)
            {
                pos = transform.position;
                pos.y = -12.84f;
                transform.position = pos;
            }
        }
    }

    public override void Hit(Collider other)
    {
        
    }

    public override void OnTrigger(Collider other)
    {
        State = EnemyState.Active;
        Trigger.SetActive(false);
        target = other.gameObject;
    }

    public override void Triggered()
    {

    }
}
