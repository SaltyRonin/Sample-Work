using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level1Controller : LevelController
{
    private float left = 0f, time = 0f;
    private bool finish = false, dialog = false;

    void Awake()
    {
#if UNITY_EDITOR
        Data.Level = 1;
        if (SceneManager.sceneCount < 2) // might already be loaded while editing
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
#endif
    }

    public void Update()
    {
        time += Time.deltaTime;
        if (dialog && time >= 3f)
        {
            Data.GameController.TriggerDialog(7);
            time = 0f;
            dialog = false;
            finish = true;
        }
        else if (finish && time >= 4f)
        {
            Data.GameController.TriggerEvent("Finished");
            finish = false;
        }
    }

    public void Event_0()
    {
        dialog = true;
        time = 0f;
    }

    public override void Finished()
    {
        left = Mathf.Lerp(left, -1f, Time.deltaTime);
        InputManager.SimulateAxis(InputManager.Axis.LY, left);
    }
}
