using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Controller : LevelController
{
    float LX = 0f, LY = 0f;
    bool damped = false;

    void Awake()
    {
#if UNITY_EDITOR
        Data.Level = 3;
        if (SceneManager.sceneCount < 2) // might already be loaded while editing
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
#endif
    }

    public void Event_99()
    {
        Data.Player.gameObject.SetActive(false);
    }

    public override void Finished()
    {
        if(damped)
        {
            LX = Mathf.Clamp(-Data.Player.transform.position.x / 5, -1f, 1f);
            LY = Mathf.Clamp(Data.Player.transform.position.y / 5, -1f, 1f);
        }
        else
        {
            LX = Mathf.Clamp(Mathf.Lerp(LX, -Data.Player.transform.position.x / 2, 3f * Time.deltaTime), -1f, 1f);
            LY = Mathf.Clamp(Mathf.Lerp(LY, Data.Player.transform.position.y / 2, 3f * Time.deltaTime), -1f, 1f);

            damped = (LX == -1f || LX == 1f || LY == -1f || LY == 1f);
        }

        InputManager.SimulateAxis(InputManager.Axis.LX, LX);
        InputManager.SimulateAxis(InputManager.Axis.LY, LY);
    }
}
