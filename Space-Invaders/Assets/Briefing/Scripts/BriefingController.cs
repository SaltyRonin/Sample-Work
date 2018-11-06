using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BriefingController : DialogPrinter
{
    private enum BriefingState
    {
        FadeIn,
        CommanderAppear,
        Dialog,
        FadeOut,
    }
    private BriefingState state = BriefingState.FadeIn;

    private enum DialogState
    {
        Opening,
        Printing,
        Closing
    }
    private DialogState dialogState = DialogState.Opening;

    public Text DialogText, DialogName;
    public RectTransform CommanderPanel, PlayerPanel, OnePanel, TwoPanel, CommanderBox;
    public GameObject CommanderImage;
    public Image Fader;
    public AudioSource Music;

    private Vector2 commanderBoxOpen = new Vector2(196, 196);
    private Vector2 commanderBoxClosed = new Vector2(196, -1);
    private Vector2 commanderOpen = new Vector2(220, 196);
    private Vector2 commanderClosed = new Vector2(196, 196);
    private Vector2 otherOpen = new Vector2(128, 182);
    private Vector2 otherClosed = new Vector2(128, 128);

    private int dialogIndex = 0;
    private Data.DialogEvent dialogEvent;
    private float time = 0f;
    private float percentage = 0f;
    private float fadePercentage = 0f;

    void Start ()
    {
        dialogEvent = Data.GetDialog(dialogIndex);
	}
	
	void Update ()
    {
        InputManager.Update();
        InputManager.MenuInputState menu = InputManager.GetMenuState();
        if (state != BriefingState.FadeIn && (menu.Start || menu.Cancel))
        {
            state = BriefingState.FadeOut;
        }

        string text = "";
        bool printFinished = false;

        switch(state)
        {
            case BriefingState.FadeIn:
                fadePercentage += Time.deltaTime;
                Fader.color = Color.Lerp(Color.black, Color.clear, fadePercentage);

                if (fadePercentage >= 1.5f)
                {
                    fadePercentage = 0f;
                    state = BriefingState.CommanderAppear;
                }
                break;

            case BriefingState.CommanderAppear:
                percentage += 3f * Time.deltaTime;
                CommanderBox.sizeDelta = Vector2.Lerp(commanderBoxClosed, commanderBoxOpen, percentage);

                if(percentage >= 1f)
                {
                    percentage = 0f;
                    CommanderImage.gameObject.SetActive(true);
                    CommanderPanel.gameObject.SetActive(true);
                    state = BriefingState.Dialog;
                }
                break;

            case BriefingState.Dialog:
                switch(dialogState)
                {
                    case DialogState.Opening:
                        percentage += 4f * Time.deltaTime;
                        switch (dialogEvent.Character)
                        {
                            case Data.Name.Commander:
                                CommanderPanel.sizeDelta = Vector2.Lerp(commanderClosed, commanderOpen, percentage);
                                break;
                            case Data.Name.Player:
                                PlayerPanel.sizeDelta = Vector2.Lerp(otherClosed, otherOpen, percentage);
                                break;
                            case Data.Name.One:
                                OnePanel.sizeDelta = Vector2.Lerp(otherClosed, otherOpen, percentage);
                                break;
                            case Data.Name.Two:
                                TwoPanel.sizeDelta = Vector2.Lerp(otherClosed, otherOpen, percentage);
                                break;
                            default:
                                break;
                        }

                        if (percentage >= 1f)
                        {
                            percentage = 0f;
                            DialogName.text = dialogEvent.Name;
                            dialogState = DialogState.Printing;
                        }
                        break;

                    case DialogState.Printing:
                        text = DialogText.text;
                        printFinished = DialogPrint(ref text, dialogEvent.Text, Time.deltaTime);
                        DialogText.text = text;

                        if (printFinished)
                        {
                            time += Time.deltaTime;
                            if (time >= dialogEvent.Time)
                            {
                                time = 0f;
                                DialogText.text = "";
                                DialogName.text = "";
                                dialogState = DialogState.Closing;
                            }
                        }
                        break;

                    case DialogState.Closing:
                        percentage += 4f * Time.deltaTime;
                        switch (dialogEvent.Character)
                        {
                            case Data.Name.Commander:
                                CommanderPanel.sizeDelta = Vector2.Lerp(commanderOpen, commanderClosed, percentage);
                                break;
                            case Data.Name.Player:
                                PlayerPanel.sizeDelta = Vector2.Lerp(otherOpen, otherClosed, percentage);
                                break;
                            case Data.Name.One:
                                OnePanel.sizeDelta = Vector2.Lerp(otherOpen, otherClosed, percentage);
                                break;
                            case Data.Name.Two:
                                TwoPanel.sizeDelta = Vector2.Lerp(otherOpen, otherClosed, percentage);
                                break;
                            default:
                                break;
                        }

                        if (percentage >= 1f)
                        {
                            percentage = 0f;
                            dialogIndex++;
                            if (dialogIndex >= Data.EventLength)
                            {
                                state = BriefingState.FadeOut;
                            }
                            else
                            {
                                dialogEvent = Data.GetDialog(dialogIndex);
                                DialogPrintReset();
                            }
                            dialogState = DialogState.Opening;
                        }
                        break;

                    default:
                        break;
                }
                break;

            case BriefingState.FadeOut:
                fadePercentage += Time.deltaTime;
                Fader.color = Color.Lerp(Color.clear, Color.black, fadePercentage);
                Music.volume = Mathf.Lerp(Music.volume, 0f, 3f * Time.deltaTime);

                if (fadePercentage >= 1f)
                {
                    fadePercentage = 0f;
                    Data.LoadNextLevel();
                }
                break;

            default:
                break;
        }
	}
}
