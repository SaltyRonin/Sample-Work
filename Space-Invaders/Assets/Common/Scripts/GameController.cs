using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        FadeIn,
        Gameplay,
        Paused,
        Complete,
        Failure,
    }
    public GameState State { get; private set; }

    public delegate void Pause();
    public static event Pause OnPause;
    public delegate void UnPause();
    public static event UnPause OnUnPause;
    public delegate void Stop();
    public static event Stop OnStop;

    private bool inputDisabled;
    private AudioSource Music;
    private LevelController levelController;
    public Camera MainCamera;
    public Camera UICamera;
    private GameObject ground, groundDup;
    private bool groundAlternate = true;
    private float groundDistance = 18000f;
    public DialogController dialog;
    public ShipBehavior Ship;
    public GameObject PauseScreen, PauseMenu, QuitDialog;
    public GameObject PauseArrows, QuitArrows;
    public GameObject[] PauseMenuItems, QuitDialogItems;
    public AudioSource menuNav, menuSelect, menuDeSelect;
    public AudioSource dialogOpen;
    public AudioClip Success, Failure;
    private int pauseSelection = 0, quitSelection = 0;
    private bool quitActive = false;
    private new CameraMovement camera;

    private bool screenFaded = false, fadeOut = false;
    private float time = 0f;
    private float volumeTarget;
    private int duckCount = 0;

    // UI Elements
    public Text Score;
    public RectTransform Health;
    public RectTransform Power;
    public Image Laser2, Laser3;
    private Image powerImage;
    private Color powerColor, powerDepletedColor;

    void Awake()
    {
        AudioListener.pause = false;
        State = GameState.FadeIn;
        Time.timeScale = 0f;
        inputDisabled = true;
        Data.ShipSelection = 0;

        Settings.Load();
        InputManager.Init();

        camera = MainCamera.GetComponent<CameraMovement>();

        Data.GameController = this;
        Data.Player = Ship;
        Ship.transform.FindChild(Data.ShipName).gameObject.SetActive(true);
    }

    void Start()
    {
        powerImage = Power.gameObject.GetComponent<Image>();
        powerColor = powerImage.color;
        powerDepletedColor = powerColor;
        powerDepletedColor.a = 0.4f;

        levelController = GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>();

        if(Data.Retry)
        {
            levelController.DisableMissionText();
        }

        Music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        ground = GameObject.FindGameObjectWithTag("Ground");
        groundDup = GameObject.FindGameObjectWithTag("GroundDup");

        volumeTarget = Music.volume;
    }

    void Update()
    {
        InputManager.Update();
        InputManager.ControllerState control = InputManager.GetState();
        InputManager.MenuInputState menu = InputManager.GetMenuState();

        switch (State)
        {
            case GameState.FadeIn:
                time += Time.unscaledDeltaTime;
                if (time >= 1f && !Music.isPlaying)
                {
                    Music.Play();
                }
                if (time >= 2.5f)
                {
                    Time.timeScale = 1f;
                    if (!screenFaded)
                    {
                        screenFaded = levelController.FadeIn();
                    }
                    else if(time >= 6f)
                    {
                        time = 0f;
                        camera.Following = true;
                        UICamera.gameObject.SetActive(true);
                        inputDisabled = false;
                        screenFaded = false;
                        State = GameState.Gameplay;
                    }
                }
                break;

            case GameState.Gameplay:
                if (control.StartDown)
                {
                    State = GameState.Paused;
                    AudioListener.pause = true;
                    if (OnPause != null) OnPause();
                    pauseSelection = 0;
                    PauseArrows.transform.position = PauseMenuItems[0].transform.position;
                    PauseScreen.SetActive(true);
                    PauseMenu.SetActive(true);
                    inputDisabled = true;
                    Time.timeScale = 0f;
                }
                else if (control.BackDown)
                {
                    Time.timeScale = (Time.timeScale == 0.15f) ? 1f : 0.15f;
                }

                if (Ship.Health == 0)
                {
                    State = GameState.Failure;
                    time = 0f;
                    levelController.OnFailure();
                    if (OnStop != null) OnStop();

                    Music.Stop();
                    Music.clip = Failure;
                    Music.Play();
                    Music.volume = volumeTarget = 1f;

                    UICamera.gameObject.SetActive(false);
                    InputManager.Simulated = true;
                    Ship.ClampMovement = false;

                    InputManager.SimulateAxis(InputManager.Axis.LY, 0.5f);
                }
                break;

            case GameState.Paused:
                if (quitActive)
                {
                    if (menu.Up)
                    {
                        if (quitSelection > 0)
                        {
                            quitSelection--;
                            menuNav.Play();
                        }
                    }
                    else if (menu.Down)
                    {
                        if (quitSelection < QuitDialogItems.Length - 1)
                        {
                            quitSelection++;
                            menuNav.Play();
                        }
                    }
                    else if (menu.Submit)
                    {
                        if (quitSelection == 0)
                        {
                            menuSelect.Play();
                            if (pauseSelection == 1)
                            {
                                Time.timeScale = 1f;
                                Data.RestartLevel();
                            }
                            else
                            {
                                Time.timeScale = 1f;
                                SceneManager.LoadScene("Menu");
                            }
                        }
                        else
                        {
                            menuDeSelect.Play();
                            PauseMenu.SetActive(true);
                            QuitDialog.SetActive(false);
                            quitSelection = 0;
                            quitActive = false;
                        }
                    }
                    else if (menu.Cancel)
                    {
                        menuDeSelect.Play();
                        PauseMenu.SetActive(true);
                        QuitDialog.SetActive(false);
                        quitSelection = 0;
                        quitActive = false;
                    }
                    QuitArrows.transform.localPosition = QuitDialogItems[quitSelection].transform.localPosition;
                }
                else
                {
                    if (menu.Start)
                    {
                        State = GameState.Gameplay;
                        AudioListener.pause = false;
                        if (OnUnPause != null) OnUnPause();
                        PauseScreen.SetActive(false);
                        PauseMenu.SetActive(false);
                        inputDisabled = false;
                        Time.timeScale = 1f;
                        InputManager.Clear();
                    }
                    else if (menu.Up)
                    {
                        if (pauseSelection > 0)
                        {
                            pauseSelection--;
                            menuNav.Play();
                        }
                    }
                    else if (menu.Down)
                    {
                        if (pauseSelection < PauseMenuItems.Length - 1)
                        {
                            pauseSelection++;
                            menuNav.Play();
                        }
                    }
                    else if (menu.Submit)
                    {
                        menuSelect.Play();
                        if (pauseSelection == 0)
                        {
                            State = GameState.Gameplay;
                            AudioListener.pause = false;
                            if (OnUnPause != null) OnUnPause();
                            PauseScreen.SetActive(false);
                            PauseMenu.SetActive(false);
                            inputDisabled = false;
                            Time.timeScale = 1f;
                            InputManager.Clear();
                        }
                        else
                        {
                            PauseMenu.SetActive(false);
                            QuitDialog.SetActive(true);
                            quitActive = true;
                        }
                    }
                    PauseArrows.transform.localPosition = PauseMenuItems[pauseSelection].transform.localPosition;
                }
                break;

            case GameState.Complete:
                time += Time.deltaTime;
                levelController.Finished();
                if(time >= 3f)
                {
                    if (!screenFaded)
                    {
                        screenFaded = levelController.FadeOut();
                    }
                    else
                    {
                        InputManager.Simulated = false;
                        inputDisabled = true;
                        Ship.ZVelocity = 0f;

                        if (fadeOut)
                        {
                            Music.volume = Mathf.Lerp(Music.volume, 0f, 3f * Time.deltaTime);
                            if (levelController.FadeToNext())
                            {
                                Data.LoadNextLevel();
                            }
                        }
                        else if (Data.Level < 4 && InputManager.GetButtonDown(InputManager.Button.Any))
                        {
                            fadeOut = true;
                        }
                    }
                }
                break;

            case GameState.Failure:
                time += Time.deltaTime;
                if (time >= 2.5f)
                {
                    if (!screenFaded)
                    {
                        screenFaded = levelController.Failure();
                    }
                    else
                    {
                        InputManager.Simulated = false;
                        inputDisabled = true;

                        if (fadeOut)
                        {
                            Music.volume = Mathf.Lerp(Music.volume, 0f, 3f * Time.deltaTime);
                            if (levelController.FadeToNext())
                            {
                                Data.RestartLevel();
                            }
                        }
                        else if (InputManager.GetButtonDown(InputManager.Button.Any))
                        {
                            fadeOut = true;
                        }
                    }
                }
                break;

            default:
                break;
        }

        if (inputDisabled)
        {
            InputManager.Clear();
        }

        // Update UI
        Score.text = string.Format("{0:000}", Data.LevelScore);
        Vector2 size = Health.sizeDelta;
        size.x = 2 * Ship.Health;
        Health.sizeDelta = size;
        size = Power.sizeDelta;
        size.x = Ship.Power;
        Power.sizeDelta = size;
        powerImage.color = (Ship.PowerDepleted) ? powerDepletedColor : powerColor;
        Laser2.color = (Ship.LaserState >= 1) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Laser3.color = (Ship.LaserState == 2) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);

        // Adjust Music Volume
        if (Music && State == GameState.Gameplay)
            Music.volume = Mathf.Lerp(Music.volume, volumeTarget, 2f * Time.deltaTime);

        // Move Ground
        if (Ship.transform.position.z >= groundDistance)
        {
            Vector3 pos;
            if (groundAlternate)
            {
                pos = groundDup.transform.position;
                pos.z += 10000f;
                ground.transform.position = pos;
            }
            else
            {
                pos = ground.transform.position;
                pos.z += 10000f;
                groundDup.transform.position = pos;
            }
            groundDistance += 10000f;
            groundAlternate = !groundAlternate;
        }
    }

    public void TriggerDialog(int index)
    {
        dialog.Display(Data.GetDialog(index));
        dialogOpen.Play();
    }

    public void TriggerEvent(string e)
    {
        if (e == "Finished")
        {
            if (Ship.Health > 0)
            {
                State = GameState.Complete;
                Music.Stop();
                Music.clip = Success;
                Music.Play();
                camera.Following = false;
                UICamera.gameObject.SetActive(false);
                InputManager.Simulated = true;
                Ship.ClampMovement = false;
                time = 0f;
                levelController.OnFinish();
            }
        }
        else
        {
            levelController.SendMessage(e);
        }
    }

    public void DuckMusic()
    {
        if(duckCount == 0)
        {
            volumeTarget *= 0.25f;
        }
        duckCount++;
    }

    public void UnDuckMusic()
    {
        if(duckCount > 0)
        {
            duckCount--;

            if(duckCount == 0)
            {
                volumeTarget *= 4f;
            }
        }
    }
}
