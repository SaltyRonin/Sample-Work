using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyBehavior : MonoBehaviour
{
    public int Health;
    public enum EnterDirection
    {
        None,
        Left,
        Right,
        Top
    }
    public EnterDirection Direction;

    public enum DropItem
    {
        None,
        Health,
        Laser,
        Either
    }
    public DropItem Drop;

    public bool FollowPlayer = true;
    public bool HitWhileInactive = false;
    public bool ExplodeOnImpact = false;
    public bool TriggerEvent = false;
    public int EventNumber = 0;
    public GameObject Laser;
    public GameObject Explosion;
    public GameObject HealthPickup, LaserPickup;

    protected bool Following { get; set; }

    protected Vector3 Position { get; set; }
    protected Vector3 Velocity { get; set; }
    protected Vector3 PlayerPosition
    {
        get
        {
            Vector3 pos = Data.Player.transform.position;
            pos.x += Random.Range(-3f, 3f);
            pos.y += Random.Range(-3f, 3f);
            pos.z += Random.Range(-3f, 3f);
            return pos;
        }
    }
    protected Vector3 ActualPlayerPosition { get { return Data.Player.transform.position; } }
    public enum EnemyState
    {
        Inactive,
        Triggered,
        Active,
        Dead,
        Stopped,
    }

    protected EnemyState State = EnemyState.Inactive;
    private new Rigidbody rigidbody;
    protected new Renderer renderer;
    protected GameObject Body, Trigger;
    protected Collider triggerCol;
    private float hitTime = 0f;
    private bool hit = false;
    protected AudioSource primaryAudio, secondaryAudio;
    private bool paused = false;
    protected bool LaserTriggerable = false;
    private bool enemyDead = false;

    void Start()
    {
        GameController.OnPause += Pause;
        GameController.OnUnPause += UnPause;
        GameController.OnStop += Stop;

        Following = false;
        Body = transform.FindChild("Body").gameObject;
        Trigger = transform.FindChild("Trigger").gameObject;
        triggerCol = Trigger.GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        renderer = Body.GetComponent<Renderer>();
        renderer.enabled = false;

        Transform t;
        if (t = transform.FindChild("PrimaryAudio"))
        {
            primaryAudio = t.GetComponent<AudioSource>();
        }
        if (t = transform.FindChild("SecondaryAudio"))
        {
            secondaryAudio = t.GetComponent<AudioSource>();
        }

        Position = transform.position;
        EnemyStart();
    }

    void OnDestroy()
    {
        if (TriggerEvent) Data.GameController.TriggerEvent("Event_" + EventNumber);

        GameController.OnPause -= Pause;
        GameController.OnUnPause -= UnPause;
        GameController.OnStop -= Stop;
    }

    void Update ()
    {
        if (paused) return;

        hitTime += Time.deltaTime;
        Vector3 pos = transform.position;
        switch (State)
        {
            case EnemyState.Inactive:
                break;

            case EnemyState.Triggered:
                Triggered();
                break;
            
            case EnemyState.Active:
                Active();
                break;

            case EnemyState.Dead:
                if (!enemyDead)
                {
                    if (Explosion)
                    {
                        Explosion.SetActive(true);
                        Explosion.transform.parent = null;
                        Destroy(Explosion, 6f);
                    }
                    Dead();
                    if (primaryAudio) primaryAudio.Stop();
                    if (secondaryAudio) secondaryAudio.Stop();

                    GameObject pickup = null;

                    switch (Drop)
                    {
                        case DropItem.Health:
                            pickup = Instantiate(HealthPickup, transform, false);
                            break;

                        case DropItem.Laser:
                            pickup = Instantiate(LaserPickup, transform, false);
                            break;

                        case DropItem.Either:
                            if (Random.value >= 0.75f)
                            {
                                pickup = Instantiate(LaserPickup, transform, false);
                            }
                            else
                            {
                                pickup = Instantiate(HealthPickup, transform, false);
                            }
                            break;

                        default:
                            break;
                    }

                    if (pickup)
                    {
                        pickup.transform.parent = null;
                        pickup.transform.position += new Vector3(0, 0, 30);
                        pickup.transform.localScale = Vector3.one;
                    }

                    renderer.enabled = false;
                    Destroy(gameObject, 0.25f);
                    enemyDead = true;
                }
                break;

            default:
                break;
        }

        if (Following)
        {
            pos = Vector3.Lerp(transform.position, Position, Time.deltaTime * 2f);
            pos.z = ActualPlayerPosition.z + Position.z;
            transform.position = pos;
        }

        if (hit && hitTime >= 0.35f)
        {
            hit = false;
            renderer.material.color = Color.white;
            hitTime = 0f;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (State == EnemyState.Inactive)
        {
            if (other.tag == "PlayerTrigger")
            {
                Vector3 pos = transform.position;
                pos.z = transform.position.z - ActualPlayerPosition.z;
                Position = pos;
                switch(Direction)
                {
                    case EnterDirection.Left:
                        transform.position += new Vector3(-100f, 0);
                        break;
                    case EnterDirection.Right:
                        transform.position += new Vector3(100f, 0);
                        break;
                    case EnterDirection.Top:
                        transform.position += new Vector3(0, 50f);
                        break;
                    default:
                        break;
                }
                Trigger.SetActive(false);
                Following = FollowPlayer;
                State = EnemyState.Triggered;
                OnTrigger(other);
                return;
            }
            else if(LaserTriggerable && other.tag.Contains("Laser"))
            {
                State = EnemyState.Triggered;
                OnTrigger(other);
                return;
            }
        }
        if(!hit && (State == EnemyState.Active || (HitWhileInactive && State == EnemyState.Inactive)))
        {
            if (name.Contains("Thomas") && Trigger.activeSelf && (transform.position - other.transform.position).magnitude >= 30) return; // fixes thomas

            if(other.tag.Contains("Laser") && !other.tag.Contains("Enemy"))
            {
                if(other.tag.Contains("1"))
                {
                    Health -= 3;
                }
                else if(other.tag.Contains("2"))
                {
                    Health -= 5;
                }
                else if(other.tag.Contains("3"))
                {
                    Health -= 7;
                }

                if(Health <= 0)
                {
                    Health = 0;
                    Data.LevelScore++;
                    State = EnemyState.Dead;
                }
                else
                {
                    hit = true;
                    renderer.material.color = new Color(1.75f, 0.5f, 0.5f);
                    Hit(other);
                    hitTime = 0f;
                }
            }
            else if (ExplodeOnImpact && other.tag == "PlayerTrigger" && State == EnemyState.Active)
            {
                State = EnemyState.Dead;
            }
        }
    }

    public void Pause()
    {
        paused = true;
    }

    public void UnPause()
    {
        paused = false;
    }

    public void Stop()
    {
        State = EnemyState.Stopped;
        if(primaryAudio) primaryAudio.Stop();
        if(secondaryAudio) secondaryAudio.Stop();
    }

    public void Fire()
    {
        if (!Laser) return;

        GameObject laser = Instantiate(Laser, transform, false);
        laser.transform.parent = null;
        laser.transform.LookAt(PlayerPosition);

        if (laser.name.Contains("JohnCena"))
        {
            GameObject laser1 = laser.transform.GetChild(0).gameObject;
            GameObject laser2 = laser.transform.GetChild(1).gameObject;
            laser1.transform.parent = null;
            laser2.transform.parent = null;
            laser1.GetComponent<Rigidbody>().velocity = laser.transform.forward * 300f;
            laser2.GetComponent<Rigidbody>().velocity = laser.transform.forward * 300f;
            Destroy(laser);
        }
        else
        {
            laser.GetComponent<Rigidbody>().velocity = laser.transform.forward * 300f;
        }

        if (secondaryAudio)
        {
            if (secondaryAudio.isPlaying)
            {
                secondaryAudio.Stop();
            }
            secondaryAudio.Play();
        }
    }


    public abstract void EnemyStart();
    public abstract void OnTrigger(Collider other);
    public abstract void Triggered();
    public abstract void Active();
    public abstract void Hit(Collider other);
    public abstract void Dead();
}
