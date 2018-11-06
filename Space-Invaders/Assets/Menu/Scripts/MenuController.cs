using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    public enum MenuPage
    {
        Main,
        Settings,
        HowToPlay,
        About,
        ChooseShip,
        StartGame,
        Quit
    }

    public RectTransform[] MainList, SettingsList, QuitList;
    public Transform[] ShipList;
    public RectTransform MainArrowL, MainArrowR, SettingsArrowL, SettingsArrowR, QuitArrowL, QuitArrowR;
    public RectTransform ShipArrowT, ShipArrowB, ShipArrowL, ShipArrowR;
    public Slider InputSlider, MusicSlider, SFXSlider;
    public GameObject QuitDialog, HowToArrows, AboutArrows;
    public AudioMixer Audio;
    public Image StartButton;
    public Text StartText;
    public AudioSource menuNav, menuSelect, menuDeSelect, testMusic;

    private MenuPage page = MenuPage.Main;
    private int mainSelection = 0, settingsSelection = 0, shipSelection = 0, quitSelection = 0;
    private int shipPrevious = 0;
    private bool shipSelected = false;
    private Quaternion[] origRot = new Quaternion[3];
    private bool transitioning = false, inputPaused = false;
    private float transitionAngle = 0f, transitionVelocity = 0f;
    private Quaternion start = Quaternion.identity, target = Quaternion.identity;
    private bool resetSettings = true, resetShips = false;

    void Awake()
    {
        AudioListener.pause = false;
        Data.Reset();
        Settings.Load();
        InputManager.Init();
    }
    
    void Start()
    {
        origRot[0] = ShipList[0].localRotation;
        origRot[1] = ShipList[1].localRotation;
        origRot[2] = ShipList[2].localRotation;
    }

    void SetVolumes()
    {
        if(MusicSlider.value == 0)
        {
            Audio.SetFloat("MusicVolume", -80f);
        }
        else
        {
            Audio.SetFloat("MusicVolume", 20f * Mathf.Log10(MusicSlider.value));
        }
        if (SFXSlider.value == 0)
        {
            Audio.SetFloat("SFXVolume", -80f);
        }
        else
        {
            Audio.SetFloat("SFXVolume", 20f * Mathf.Log10(SFXSlider.value));
        }
    }

    void Update ()
    {
        InputManager.Update();
        InputManager.MenuInputState menu = InputManager.GetMenuState();

        if (transitioning)
        {
            transitionAngle = Mathf.SmoothDampAngle(transitionAngle, 90, ref transitionVelocity, 0.5f);
            Camera.main.transform.rotation = Quaternion.RotateTowards(start, target, transitionAngle);
            if (Mathf.Abs(90 - transitionAngle) < 0.01f)
            {
                Camera.main.transform.rotation = target;
                start = target;
                transitionAngle = 0f;
                transitioning = false;
            }
            else if(inputPaused && Mathf.Abs(90 - transitionAngle) < 15f)
            {
                inputPaused = false;
                EnableArrows();
            }
        }
        if(!inputPaused)
        {
            switch (page)
            {
                case MenuPage.Main:
                    if (resetSettings)
                    {
                        settingsSelection = 0;
                        InputSlider.value = (Settings.UseController) ? 1 : 0;
                        MusicSlider.value = Settings.MusicVolume;
                        SFXSlider.value = Settings.SFXVolume;
                        SetVolumes();
                        resetSettings = false;
                    }
                    if(resetShips)
                    {
                        shipSelection = 0;
                        foreach(Transform t in ShipList)
                        {
                            Vector3 pos = t.localPosition;
                            pos.z = 240;
                            t.localPosition = pos;
                        }
                        ShipList[0].localRotation = origRot[0];
                        ShipList[1].localRotation = origRot[1];
                        ShipList[2].localRotation = origRot[2];
                        resetShips = false;
                    }

                    if (menu.Submit)
                    {
                        menuSelect.Play();
                        switch(mainSelection)
                        {
                            case 0:
                                target = Quaternion.Euler(-90, 0, 0);
                                page = MenuPage.ChooseShip;
                                transitioning = true;
                                break;
                            case 1:
                                page = MenuPage.Settings;
                                target = Quaternion.Euler(0, 90, 0);
                                transitioning = true;
                                break;
                            case 2:
                                page = MenuPage.HowToPlay;
                                target = Quaternion.Euler(0, -90, 0);
                                transitioning = true;
                                break;
                            case 3:
                                page = MenuPage.About;
                                target = Quaternion.Euler(90, 0, 0);
                                transitioning = true;
                                break;
                            case 4:
                                page = MenuPage.Quit;
                                QuitDialog.SetActive(true);
                                transitioning = false;
                                DisableArrows();
                                break;
                            default:
                                transitioning = false;
                                break;
                        }
                        inputPaused = transitioning;
                    }
                    else if(menu.Up)
                    {
                        if (mainSelection > 0)
                        {
                            menuNav.Play();
                            mainSelection--;
                        }
                    }
                    else if (menu.Down)
                    {
                        menuNav.Play();
                        if (mainSelection < 4)
                        {
                            menuNav.Play();
                            mainSelection++;
                        }
                    }
                    else if(menu.Cancel)
                    {
                        menuDeSelect.Play();
                        page = MenuPage.Quit;
                        QuitDialog.SetActive(true);
                        DisableArrows();
                    }
                    break;

                case MenuPage.Settings:
                    if (menu.Up)
                    {
                        if (settingsSelection > 2)
                        {
                            menuNav.Play();
                            settingsSelection = 2;
                        }
                        else if (settingsSelection > 0)
                        {
                            menuNav.Play();
                            settingsSelection--;
                        }
                        if (testMusic.isPlaying) testMusic.Stop();
                    }
                    else if (menu.Down)
                    {
                        if (settingsSelection < 3)
                        {
                            menuNav.Play();
                            settingsSelection++;
                        }
                        if (testMusic.isPlaying) testMusic.Stop();
                    }
                    else if (menu.Left)
                    {
                        switch(settingsSelection)
                        {
                            case 0:
                                InputSlider.value = 0f;
                                menuDeSelect.Play();
                                break;
                            case 1:
                                MusicSlider.value = MusicSlider.value - 0.1f;
                                SetVolumes();
                                if (!testMusic.isPlaying) testMusic.Play();
                                break;
                            case 2:
                                SFXSlider.value = SFXSlider.value - 0.1f;
                                SetVolumes();
                                break;
                            case 3:
                                // do nothing
                                break;
                            case 4:
                                settingsSelection = 3;
                                break;
                        }
                        if(settingsSelection != 0) menuNav.Play();
                    }
                    else if (menu.Right)
                    {
                        switch (settingsSelection)
                        {
                            case 0:
                                InputSlider.value = 1f;
                                menuSelect.Play();
                                break;
                            case 1:
                                MusicSlider.value = MusicSlider.value + 0.1f;
                                SetVolumes();
                                if (!testMusic.isPlaying) testMusic.Play();
                                break;
                            case 2:
                                SFXSlider.value = SFXSlider.value + 0.1f;
                                SetVolumes();
                                break;
                            case 3:
                                settingsSelection = 4;
                                break;
                            case 4:
                                // do nothing
                                break;
                        }
                        if(settingsSelection != 0) menuNav.Play();
                    }

                    if (menu.Submit && settingsSelection == 3)
                    {
                        menuSelect.Play();
                        Settings.UseController = (InputSlider.value == 1);
                        Settings.MusicVolume = MusicSlider.value;
                        Settings.SFXVolume = SFXSlider.value;
                        Settings.Save();
                        resetSettings = true;
                        page = MenuPage.Main;
                        target = Quaternion.identity;
                        transitioning = true;
                        inputPaused = true;
                    }
                    if (menu.Cancel || (menu.Submit && settingsSelection == 4))
                    {
                        if (menu.Cancel) menuDeSelect.Play();
                        else menuSelect.Play();

                        resetSettings = true;
                        page = MenuPage.Main;
                        target = Quaternion.identity;
                        transitioning = true;
                        inputPaused = true;

                        if (testMusic.isPlaying) testMusic.Stop();
                    }
                    break;

                case MenuPage.ChooseShip:
                    if (shipSelected)
                    {
                        if(menu.Submit)
                        {
                            menuSelect.Play();
                            Data.ShipSelection = shipSelection;
                            page = MenuPage.StartGame;
                            target = Quaternion.Euler(-180, 0, 0);
                            transitioning = true;
                            inputPaused = true;
                        }
                        else if(menu.Cancel)
                        {
                            menuDeSelect.Play();
                            shipSelected = false;
                            ShipArrowT.gameObject.SetActive(true);
                            ShipArrowB.gameObject.SetActive(true);
                            ShipArrowL.gameObject.SetActive(false);
                            ShipArrowR.gameObject.SetActive(false);
                            Color color = StartButton.color;
                            color.a = 0.5f;
                            StartButton.color = color;
                            color = StartText.color;
                            color.a = 0.5f;
                            StartText.color = color;
                        }
                    }
                    else
                    {
                        if(menu.Submit)
                        {
                            menuSelect.Play();
                            shipSelected = true;
                            ShipArrowT.gameObject.SetActive(false);
                            ShipArrowB.gameObject.SetActive(false);
                            ShipArrowL.gameObject.SetActive(true);
                            ShipArrowR.gameObject.SetActive(true);
                            Color color = StartButton.color;
                            color.a = 1f;
                            StartButton.color = color;
                            color = StartText.color;
                            color.a = 1f;
                            StartText.color = color;
                        }
                        else if (menu.Cancel)
                        {
                            menuDeSelect.Play();
                            page = MenuPage.Main;
                            target = Quaternion.identity;
                            transitioning = true;
                            inputPaused = true;
                            resetShips = true;
                        }
                        else if (menu.Right)
                        {
                            if (shipSelection < 2)
                            {
                                menuNav.Play();
                                shipPrevious = shipSelection;
                                shipSelection++;
                            }
                        }
                        else if (menu.Left)
                        {
                            if (shipSelection > 0)
                            {
                                menuNav.Play();
                                shipPrevious = shipSelection;
                                shipSelection--;
                            }
                        }
                    }
                    Vector3 targetPos = new Vector3(ShipList[shipSelection].localPosition.x, ShipList[shipSelection].localPosition.y, -60);
                    ShipList[shipSelection].localPosition = Vector3.Lerp(ShipList[shipSelection].localPosition, targetPos, 2f * Time.deltaTime);
                    ShipList[shipSelection].Rotate(ShipList[shipSelection].parent.up, -120 * Time.deltaTime);

                    if (shipSelection != shipPrevious)
                    {
                        targetPos = new Vector3(ShipList[shipPrevious].localPosition.x, ShipList[shipPrevious].localPosition.y, 240);
                        ShipList[shipPrevious].localPosition = Vector3.Lerp(ShipList[shipPrevious].localPosition, targetPos, 2f * Time.deltaTime);
                        ShipList[shipPrevious].localRotation = origRot[shipPrevious];
                    }
                    break;

                case MenuPage.StartGame:
                    SceneManager.LoadScene("Briefing");
                    break;

                case MenuPage.Quit:
                    if (menu.Up)
                    {
                        if (quitSelection > 0)
                        {
                            menuNav.Play();
                            quitSelection--;
                        }
                    }
                    else if (menu.Down)
                    {
                        if (quitSelection < 1)
                        {
                            menuNav.Play();
                            quitSelection++;
                        }
                    }
                    else if(menu.Cancel || (menu.Submit && quitSelection == 1))
                    {
                        if (menu.Cancel) menuDeSelect.Play();
                        else menuSelect.Play();

                        page = MenuPage.Main;
                        QuitDialog.SetActive(false);
                        quitSelection = 0;
                        EnableArrows();
                    }
                    else if(menu.Submit && quitSelection == 0)
                    {
                        menuSelect.Play();
                        #if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
                        #else
                            Application.Quit();
                        #endif
                    }
                    break;

                case MenuPage.HowToPlay:
                    if (menu.Submit || menu.Cancel)
                    {
                        if (menu.Cancel) menuDeSelect.Play();
                        else menuSelect.Play();

                        page = MenuPage.Main;
                        target = Quaternion.identity;
                        transitioning = true;
                        inputPaused = true;
                    }
                    break;

                case MenuPage.About:
                    if (menu.Submit || menu.Cancel)
                    {
                        if (menu.Cancel) menuDeSelect.Play();
                        else menuSelect.Play();

                        page = MenuPage.Main;
                        target = Quaternion.identity;
                        transitioning = true;
                        inputPaused = true;
                    }
                    break;

                default:
                    break;
            }

            if(inputPaused)
            {
                transitionAngle = 0f;
                transitionVelocity = 0f;
                start = Camera.main.transform.rotation;
                DisableArrows();
            }
        }

        Vector3 newPosL = MainList[mainSelection].localPosition;
        Vector3 newPosR = newPosL;
        newPosL.x = MainList[mainSelection].localPosition.x - MainList[mainSelection].sizeDelta.x / 2 - 12;
        newPosR.x = MainList[mainSelection].localPosition.x + MainList[mainSelection].sizeDelta.x / 2 + 12;
        MainArrowL.localPosition = newPosL;
        MainArrowR.localPosition = newPosR;
        newPosL = SettingsList[settingsSelection].localPosition;
        newPosR = newPosL;
        newPosL.x = SettingsList[settingsSelection].localPosition.x - SettingsList[settingsSelection].sizeDelta.x / 2 - 12;
        newPosR.x = SettingsList[settingsSelection].localPosition.x + SettingsList[settingsSelection].sizeDelta.x / 2 + 12;
        SettingsArrowL.localPosition = newPosL;
        SettingsArrowR.localPosition = newPosR;
        newPosL = QuitList[quitSelection].localPosition;
        newPosR = newPosL;
        newPosL.x = QuitList[quitSelection].localPosition.x - QuitList[quitSelection].sizeDelta.x / 2 - 12;
        newPosR.x = QuitList[quitSelection].localPosition.x + QuitList[quitSelection].sizeDelta.x / 2 + 12;
        QuitArrowL.localPosition = newPosL;
        QuitArrowR.localPosition = newPosR;
        newPosL = ShipList[shipSelection].localPosition;
        newPosR = newPosL;
        newPosL.y = ShipArrowT.localPosition.y;
        newPosR.y = ShipArrowB.localPosition.y;
        newPosL.z = 0;
        newPosR.z = 0;
        ShipArrowT.localPosition = newPosL;
        ShipArrowB.localPosition = newPosR;
    }

    void DisableArrows()
    {
        MainArrowL.gameObject.SetActive(false);
        MainArrowR.gameObject.SetActive(false);
        SettingsArrowL.gameObject.SetActive(false);
        SettingsArrowR.gameObject.SetActive(false);
        HowToArrows.SetActive(false);
        AboutArrows.SetActive(false);
        ShipArrowT.gameObject.SetActive(false);
        ShipArrowB.gameObject.SetActive(false);
    }

    void EnableArrows()
    {
        MainArrowL.gameObject.SetActive(true);
        MainArrowR.gameObject.SetActive(true);
        SettingsArrowL.gameObject.SetActive(true);
        SettingsArrowR.gameObject.SetActive(true);
        HowToArrows.SetActive(true);
        AboutArrows.SetActive(true);
        ShipArrowT.gameObject.SetActive(true);
        ShipArrowB.gameObject.SetActive(true);
    }
}

