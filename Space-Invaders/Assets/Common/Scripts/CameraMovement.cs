using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool Following = false;
    public static bool Shake;
    public float ZExtra { get; set; }
    public float FOVExtra { get; set; }

    public ShipBehavior Target;
    public float ZDistance = 10;
    public float PositionScaleX = 0.66f;
    public float PositionScaleY = 0.66f;
    public float RotationSensitivity = 0.25f;

    private new Camera camera;
    private float extraFOV = 0f;
    private float extraFOVVelocity = 0f;
    private float extraDistance = 0f;
    private float extraDistanceVelocity = 0f;
    private Transform pixelShip;
    private Vector3 pixelShipPos;
    private Vector3 pixelShipScale;
    private bool catchUp = true;
    private float percentage = 0f, lerp = 0f;
    private bool paused = false;

    public void Start()
    {
        GameController.OnPause += Pause;
        GameController.OnUnPause += UnPause;

        camera = GetComponent<Camera>();
        pixelShip = transform.Find("PixelShip");
        pixelShipPos = pixelShip.localPosition;
        pixelShipScale = pixelShip.localScale;
    }

    void LateUpdate()
    {
        if (paused) return;

        if (!Following)
        {
            transform.LookAt(Target.transform);
        }
        else
        {
            // Apply position
            Vector3 newPos = Target.transform.position;
            newPos.x *= PositionScaleX; // Move a percentage of the target's position
            newPos.y *= PositionScaleY; // Move a percentage of the target's position
            newPos.z = Target.transform.position.z - ZDistance;
            extraDistance = Mathf.SmoothDamp(extraDistance, ZExtra, ref extraDistanceVelocity, 0.5f);
            newPos.z -= extraDistance;

            // Keep pixelated plane at same distance
            pixelShip.localPosition = pixelShipPos + new Vector3(0, 0, extraDistance);

            // Apply rotation
            Quaternion targetRotation = Quaternion.Lerp(Quaternion.identity, Target.ViewRotation, RotationSensitivity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

            // Apply FOV
            extraFOV = Mathf.SmoothDamp(extraFOV, FOVExtra, ref extraFOVVelocity, 0.5f);
            camera.fieldOfView = 60f + extraFOV;

            // Scale pixelated plane to match
            pixelShip.localScale = pixelShipScale * ((camera.fieldOfView / 60f) * (pixelShip.localPosition.z / pixelShipPos.z));

            if (catchUp)
            {
                percentage += 0.25f * Time.deltaTime;
                lerp = Mathf.Lerp(lerp, 1f, percentage);
                transform.position = Vector3.Lerp(transform.position, newPos, lerp);

                if (lerp >= 1f)
                {
                    lerp = 0f;
                    percentage = 0f;
                    catchUp = false;
                }
            }
            else
            {
                transform.position = newPos;
                if (Shake) transform.position += new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
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
