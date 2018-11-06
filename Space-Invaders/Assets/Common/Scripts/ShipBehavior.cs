using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    public int Health { get; set; }
    public float Power { get; set; }
    public bool PowerDepleted { get; set; }
    public bool ClampMovement { get; set; }

    public float XYVelocity = 45f;
    public float ZVelocity = 30f;
    public Vector2 TurnAngle = new Vector2(30, 30);
    public Vector2 PositionBounds;
    public GameObject NearReticle, FarReticle;
    public GameObject Explosion, Smoke;
    public GameObject Laser1, Laser2, Laser3;
    public float LaserVelocity = 300f;
    public float BombVelocity = 30f;
    public new CameraMovement camera;

    public Quaternion ViewRotation { get; set; }
    public int LaserState { get; set; }

    public AudioSource laserAudio, hitAudio;
    public AudioSource HealthAudio, PowerUpAudio, PowerDownAudio;
    private new Rigidbody rigidbody;
    private InputManager.ControllerState input;
    private bool onEdgeX, onEdgeY;
    private bool rollL, rollR;
    private bool barrelRollL, barrelRollR;
    private float barrelRollBefore = 0f, barrelRollAngle = 0f;
    private float boostVelocity, speed;
    private Vector3 pushVelocity = new Vector3();
    private Vector2 pushVelTarget = new Vector2();
    private Vector2 pushAngle = new Vector2();
    private Vector2 pushAngleTarget = new Vector2();
    private MeshRenderer[] meshRenderers;
    private List<Color> materialColors = new List<Color>();
    private List<int> hitList = new List<int>();

    private float laserTime;
    private int fireCount;
    private bool firing = false;
    private bool damaged;
    private float damageTime = 0f;
    private bool paused = false;
    private int healthSincePickup = 0;

    void Start()
    {
        GameController.OnPause += Pause;
        GameController.OnUnPause += UnPause;

        speed = ZVelocity;
        LaserState = 0;
        rigidbody = GetComponent<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Health = 100;
        Power = 100;
        ClampMovement = true;
    }

    void OnDestroy()
    {
        GameController.OnPause -= Pause;
        GameController.OnUnPause -= UnPause;
    }

    void Update ()
    {
        if (paused) return;

        // Get inputs
        input = InputManager.GetState();
        rollL = input.L && !input.R;
        rollR = input.R && !input.L;
        if(!barrelRollL) barrelRollL = input.DoubleL;
        if(!barrelRollR) barrelRollR = input.DoubleR;

        // Clamp position
        if (ClampMovement)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -PositionBounds.x, PositionBounds.x);
            onEdgeX = Mathf.Abs(pos.x) == PositionBounds.x;
            pos.y = Mathf.Clamp(pos.y, -PositionBounds.y, PositionBounds.y);
            onEdgeY = Mathf.Abs(pos.y) == PositionBounds.y;
            transform.position = pos;
        }


        // Apply rotation
        float zAngle = 0f, zViewAngle = 0f;
        if (transform.position.y == -PositionBounds.y)
        {
            input.LY *= 0.25f; // Limit angle if on the ground
        }

        if (barrelRollL || barrelRollR)
        {
            zViewAngle = 0f;
            if(barrelRollL)
            {
                zAngle = transform.rotation.eulerAngles.z + 150f;
            }
            else
            {
                zAngle = transform.rotation.eulerAngles.z + -150f;
            }

            barrelRollBefore = transform.rotation.eulerAngles.z;
        }
        else if (rollL || rollR)
        {
            input.LX *= 1.5f; // Increase angle when rolling
            zViewAngle = 0;

            if (rollL)
            {
                zAngle = 90;
            }
            else
            {
                zAngle = -90;
            }
        }
        else
        {
            zAngle = input.LX * -TurnAngle.x;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(input.LY * TurnAngle.y - pushAngle.y, input.LX * TurnAngle.x + pushAngle.x, zAngle), 5f * Time.deltaTime);
        ViewRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, zViewAngle);

        if (barrelRollL || barrelRollR)
        {
            barrelRollAngle += Mathf.Abs(transform.rotation.eulerAngles.z - barrelRollBefore);
            if(barrelRollAngle >= 930f)
            {
                barrelRollL = barrelRollR = false;
                barrelRollAngle = 0f;
            }
        }

        // Prevent reticle distortion (keep parallel to screen)
        NearReticle.transform.rotation = Camera.main.transform.rotation;
        FarReticle.transform.rotation = Camera.main.transform.rotation;


        // Fire Laser
        if (firing || input.ADown)
        {
            firing = true;

            if (input.A)
            {
                if (fireCount == 0 || fireCount <= 2 && laserTime >= 0.125f)
                {
                    Fire();
                    laserTime = 0;
                }
                laserTime += Time.deltaTime;
            }
            else
            {
                fireCount = 0;
                firing = false;
            }
        }

        if (InputManager.GetButtonDown(InputManager.Button.B))
        {
            if(gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                gameObject.layer = LayerMask.NameToLayer("Pixelation");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }

        }

        float targetVelocity = ZVelocity;
        if (InputManager.GetButton(InputManager.Button.RT) && !PowerDepleted)
        {
            //boost
            targetVelocity *= 1.5f;
            camera.FOVExtra = 2f;
            camera.ZExtra = 2f;
            Power -= 40f * Time.deltaTime;
            if (Power < 0) Power = 0;
            PowerDepleted = (Power == 0);
        }
        else if(InputManager.GetButton(InputManager.Button.LT) && !PowerDepleted)
        {
            //brake
            targetVelocity /= 1.5f;
            camera.FOVExtra = -2f;
            camera.ZExtra = -1f;
            Power -= 40f * Time.deltaTime;
            if (Power < 0) Power = 0;
            PowerDepleted = (Power == 0);
        }
        else
        {
            camera.ZExtra = 0f;
            camera.FOVExtra = 0f;
            Power += 30f * Time.deltaTime;
            if (Power > 100) Power = 100;
            if(PowerDepleted) PowerDepleted = (Power < 100);
        }

        speed = Mathf.SmoothDamp(speed, targetVelocity, ref boostVelocity, 0.5f);

        if (damaged && Health > 0)
        {
            damageTime += Time.deltaTime;
            if(damageTime >= 0.5f)
            {
                damaged = false;
                SetTint(Color.white);
            }
        }

        pushVelocity = Vector3.Lerp(pushVelocity, pushVelTarget, Time.deltaTime * 10f);
        pushAngle = Vector2.Lerp(pushAngle, pushAngleTarget, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        if (paused) return;

        // Apply velocity
        Vector3 vel = transform.forward;
        vel.x *= XYVelocity;
        if (rollL || rollR || barrelRollL || barrelRollR)
            vel.y *= XYVelocity;
        else
            vel.y *= (input.LY == 0) ? 0 : XYVelocity; // Correct Y velocity when turning (the roll angle pitches the forward vector up slightly)
        vel.z = speed;
        rigidbody.velocity = vel + pushVelocity;
    }

    void Fire()
    {
        GameObject laserblast;
        switch (LaserState)
        {
            case 1:
                laserblast = Laser2;
                break;
            case 2:
                laserblast = Laser3;
                break;
            default:
                laserblast = Laser1;
                break;
        }

        GameObject laser = Instantiate(laserblast, transform, false);
        laser.transform.parent = null;

        Vector3 velocity = new Vector3((onEdgeX) ? 0 : rigidbody.velocity.x, (onEdgeY) ? 0 : rigidbody.velocity.y, rigidbody.velocity.z);
        laser.GetComponent<Rigidbody>().velocity = LaserVelocity * transform.forward + velocity;

        fireCount++;
        if(laserAudio.isPlaying)
        {
            laserAudio.Stop();
        }
        laserAudio.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        int id = other.gameObject.GetInstanceID();
        hitList.Add(id);
        if (hitList.FindAll(o => o == id).Count > 1) return;

        if(other.tag == "Event")
        {
            if (other.name.Contains("Dialog"))
            {
                int dialogIndex = int.Parse(other.name.Split('_')[1]);
                Data.GameController.TriggerDialog(dialogIndex);
            }
            else
            {
                Data.GameController.TriggerEvent(other.name);
            }
        }
        else if (other.tag == "Enemy" || other.tag == "EnemyLaser" || other.tag == "Ground" || other.tag == "Building" || other.tag == "Asteroid")
        {
            if (other.tag == "Ground" && Health <= 5)
            {
                if (!other.isTrigger)
                {
                    ZVelocity = 0;
                    XYVelocity = 0;
                    speed = 0;
                }
                else
                {
                    other.enabled = false;
                }
                Health = 0;

                Explosion.SetActive(true);
            }
            else if (!damaged)
            {
                if (other.tag == "EnemyLaser")
                {
                    if (barrelRollL || barrelRollR) return; // can't hit me if I do a barrel roll
                    Health -= 10;
                }
                else
                {
                    Health -= 5;

                    if (hitAudio.isPlaying) hitAudio.Stop();
                    hitAudio.Play();
                }

                healthSincePickup += 5;
                if(healthSincePickup >= 30 && LaserState > 0)
                {
                    PowerDownAudio.Play();
                    LaserState--;
                    healthSincePickup = 0;
                }

                if (Health <= 0)
                {
                    Health = 0;
                    Smoke.SetActive(true);
                }
                else SetTint(new Color(1.75f, 0.5f, 0.5f));

                damageTime = 0f;
                damaged = true;
            }
            else if(other.tag != "EnemyLaser")
            {
                if (hitAudio.isPlaying) hitAudio.Stop();
                hitAudio.Play();
            }
        }
        else if(other.tag.Contains("Pickup"))
        {
            if(other.tag.Contains("H"))
            {
                HealthAudio.Play();
                Health += 5;
                if (Health > 100) Health = 100;
            }
            else if(other.tag.Contains("L"))
            {
                PowerUpAudio.Play();
                LaserState++;
                if (LaserState > 2) LaserState = 2;
            }
            healthSincePickup = 0;
            hitList.Remove(id);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Ground" || other.tag == "Building" || other.tag == "Asteroid")
        {
            Vector2 force = new Vector2();
            if(other.tag == "Ground")
            {
                force = new Vector2(0, -0.5f) * speed;
            }
            else
            {
                force = (other.gameObject.transform.position - transform.position).normalized * speed;
            }

            if(other.tag == "Building")
            {
                force.y = 0;
            }

            pushAngleTarget = force * -10f;
            pushVelTarget = force * -5f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        int id = other.gameObject.GetInstanceID();
        hitList.Remove(id);
        if (hitList.Exists(o => o == id)) return;

        if (other.tag == "Enemy" || other.tag == "Ground" || other.tag == "Building" || other.tag == "Asteroid")
        {
            pushVelTarget = Vector2.zero;
            pushAngleTarget = Vector2.zero;
        }
    }

    private void SetTint(Color color)
    { 
        foreach(MeshRenderer mr in meshRenderers)
        {
            foreach (Material m in mr.materials)
            {
                if (m.mainTexture == null)
                {
                    if (color == Color.white)
                    {
                        m.color = materialColors[0];
                        materialColors.RemoveAt(0);
                    }
                    else
                    {
                        materialColors.Add(m.color);
                        m.color = color;
                    }
                }
                else
                {
                    m.color = color;
                }
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
}
