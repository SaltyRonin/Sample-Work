using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level3Controller : LevelController
{
    public Text GameOver, TotalScore, PressAnyButton;
    public AudioSource Music;

    private float time = 0f, dialogTime = 0f;
    private bool fadeIn = false, fadeOut = false;
    private int finalEnemyCount = 0;
    private bool finish = false, dialog = false;
    void Awake()
    {
#if UNITY_EDITOR
        Data.Level = 4;
        if (SceneManager.sceneCount < 2) // might already be loaded while editing
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }
#endif
    }

    void Start()
    {
        GameObject ship = Data.Player.transform.FindChild(Data.ShipName).gameObject;

        ship.layer = LayerMask.NameToLayer("Pixelation");
        foreach (Transform t in ship.transform)
        {
            t.gameObject.layer = LayerMask.NameToLayer("Pixelation");
        }
    }

    public void Update()
    {
        dialogTime += Time.deltaTime;
        if (dialog && dialogTime >= 3f)
        {
            Data.GameController.TriggerDialog(1);
            dialogTime = 0f;
            dialog = false;
            finish = true;
        }
        if (finish && dialogTime >= 4f)
        {
            Data.GameController.TriggerEvent("Finished");
            finish = false;
        }
    }

    public void Event_0()
    {
        finalEnemyCount++;
        if (finalEnemyCount >= 3)
        {
            dialog = true;
            dialogTime = 0f;
        }
    }

    public override void Finished()
    {
        time += Time.deltaTime;
        if (!fadeIn && time >= 6f)
        {
            CompleteText.color = Color.Lerp(Color.white, Color.clear, time - 6f);
            ScoreText.color = Color.Lerp(Color.white, Color.clear, time - 6f);

            TotalScore.text = string.Format("TOTAL SCORE: {0}", Data.TotalScore + Data.LevelScore);
            GameOver.color = Color.Lerp(Color.clear, Color.white, time - 7f);
            TotalScore.color = Color.Lerp(Color.clear, Color.white, time - 8f);
            PressAnyButton.color = Color.Lerp(Color.clear, Color.white, time - 9f);
            if (time >= 10f)
            {
                fadeIn = true;
                time = 0f;
            }
        }
        else
        {
            if (InputManager.GetButtonDown(InputManager.Button.Any))
            {
                fadeOut = true;
                time = 0f;
            }
        }

        if(fadeOut)
        {
            GameOver.color = Color.Lerp(Color.white, Color.clear, time);
            TotalScore.color = Color.Lerp(Color.white, Color.clear, time);
            PressAnyButton.color = Color.Lerp(Color.white, Color.clear, time);
            Music.volume = Mathf.Lerp(Music.volume, 0f, 3f * Time.deltaTime);

            if (time >= 2f)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
