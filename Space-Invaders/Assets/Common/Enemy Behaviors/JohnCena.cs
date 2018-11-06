using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnCena : EnemyBehavior
{
    public GameObject shatter;
    public Material baseMaterial;
    public RenderTexture cameraTexture;

    private Rigidbody[] children;
    private Collider[] colliders;
    private GameObject shatterInstance;
    private Texture2D tex;
    private Material material;
    private bool explode = false, disableColliders = false;
    private float time = 0f, fireTime = 0f;
    private bool dead = false;

    int fireCount = 0;
    int fireLoop = 0;

    public override void EnemyStart()
    {
        tex = new Texture2D(800, 600, TextureFormat.RGB24, false);
        material = new Material(baseMaterial);
        material.mainTexture = tex;
    }

	public override void Active()
    {
        time += Time.deltaTime;
        fireTime += Time.deltaTime;

        switch(fireCount)
        {
            case 0:
                if(fireTime >= 0.4f)
                {
                    Fire();
                    fireCount++;
                }
                break;
            case 1:
                if (fireTime >= 0.7f)
                {
                    Fire();
                    fireCount++;
                }
                break;
            case 2:
                if (fireTime >= 0.9f)
                {
                    Fire();
                    fireCount++;
                }
                break;
            case 3:
                if (fireTime >= 1.25f)
                {
                    Fire();
                    fireCount++;
                }
                break;
            case 4:
                if(fireTime >= 2.75f)
                {
                    fireLoop++;
                    fireTime = 0f;

                    if (fireLoop >= 4)
                    {
                        Data.GameController.UnDuckMusic();
                        fireCount++;
                    }
                    else
                    {
                        fireCount = 0;
                    }
                }
                break;
            default:
                if (fireTime >= 3f)
                {
                    Following = false;
                    if(!dead)
                    {
                        Destroy(gameObject, 4f);
                        dead = true;
                    }
                }
                break;
        }

        material.color = Color.Lerp(Color.white, Color.clear, (time - 1f) / 2f);
    }

    public override void Triggered()
    {
        time += Time.deltaTime;

        if(time >= 0.8f)
        {
            State = EnemyState.Active;

            Graphics.CopyTexture(cameraTexture, tex);
            shatterInstance.SetActive(true);
            shatterInstance.transform.parent = null;
            renderer.enabled = true;
            Destroy(shatterInstance, 4f);
            explode = true;

            time = 0f;
            fireTime = -0.1f;
        }
    }

    void FixedUpdate()
    {
        if (explode)
        {
            foreach (Rigidbody r in children)
            {
                Vector3 force = r.transform.position - transform.position;
                force.x = 1f / force.x;
                force.y = 1f / force.y;
                //force.z = 1f / force.z;
                force.z = 7500f;
                r.AddForce(force * 1.5f);
                //r.AddForce(new Vector3(0, 0, 15000));
            }
            explode = false;
            disableColliders = true;
        }
        else if (disableColliders)
        {
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
            }
            disableColliders = false;
        }
    }

    public override void OnTrigger(Collider other)
    {
        shatterInstance = Instantiate(shatter, Camera.main.transform, false);
        children = shatterInstance.GetComponentsInChildren<Rigidbody>();
        colliders = shatterInstance.GetComponentsInChildren<Collider>();
        foreach (MeshRenderer mr in shatterInstance.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = material;
        }

        Data.GameController.DuckMusic();
        primaryAudio.Play();
    }

    public override void Hit(Collider other)
    {
        
    }

    public override void Dead()
    {
        Data.GameController.UnDuckMusic();
    }
}
