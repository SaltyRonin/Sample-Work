using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject HealthPickup, LaserPickup;
    public Material Alternate;

    private new MeshRenderer renderer;
    private new Rigidbody rigidbody;
    private float hitTime = 0f;
    private bool hit = false;
    private Color color;
    private int Health;
    private Vector3 rot;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        color = renderer.material.color;

        Vector3 scale = Vector3.one;

        if (Random.value >= 0.75f)
        {
            float rand1 = Random.Range(-2f, -0.5f);
            float rand2 = Random.Range(0.5f, 2f);
            if (Random.value >= 0.5) scale.x = rand1;
            else scale.x = rand2;

            rand1 = Random.Range(-2f, -0.5f);
            rand2 = Random.Range(0.5f, 2f);
            if (Random.value >= 0.5) scale.y = rand1;
            else scale.y = rand2;

            rand1 = Random.Range(-2f, -0.5f);
            rand2 = Random.Range(0.5f, 2f);
            if (Random.value >= 0.5) scale.z = rand1;
            else scale.z = rand2;

            transform.localScale = scale;

            rand1 = Random.Range(-20, 20);
            rand2 = Random.Range(-20, 20);
            float rand3 = Random.Range(-20, 20);

            rot = new Vector3(rand1, rand2, rand3);
        }

        // for some reason, inflated mesh colliders don't work properly after scaling,
        // so scale the object before enabling the collider
        GetComponent<MeshCollider>().enabled = true;

        Health = (int)scale.magnitude * 5;

        Vector3 pos = new Vector3();
        pos.x = Random.Range(-6f, 6f);
        pos.y = Random.Range(-4f, 4f);
        pos.z = Random.Range(-8f, 8f);

        transform.position += pos;

        if(Random.value >= 0.5)
        {
            renderer.material = Alternate;
        }

    }
	
	void Update ()
    {
        hitTime += Time.deltaTime;

        transform.Rotate(rot * Time.deltaTime);

        if (hit && hitTime >= 0.1f)
        {
            hit = false;
            renderer.material.color = color;
            hitTime = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hit && other.tag.Contains("Laser"))
        {
            if (other.tag.Contains("1"))
            {
                Health -= 2;
            }
            else if (other.tag.Contains("2"))
            {
                Health -= 4;
            }
            else if (other.tag.Contains("3"))
            {
                Health -= 6;
            }
            else
            {
                Health -= 2;
            }

            if (Health <= 0)
            {
                Health = 0;
                Explosion.SetActive(true);
                Explosion.transform.parent = null;
                Destroy(Explosion, 6f);

                GameObject pickup = null;

                if (Random.value >= 0.75f)
                {
                    pickup = Instantiate(LaserPickup, transform, false);
                }
                else
                {
                    pickup = Instantiate(HealthPickup, transform, false);
                }

                if(pickup)
                {
                    pickup.transform.parent = null;
                    pickup.transform.localScale = Vector3.one;
                    pickup.transform.rotation = Quaternion.identity;
                }

                Destroy(gameObject);
            }
            else
            {
                hit = true;
                renderer.material.color = new Color(1.75f, 0.5f, 0.5f);
                hitTime = 0f;
            }
        }
    }
}
