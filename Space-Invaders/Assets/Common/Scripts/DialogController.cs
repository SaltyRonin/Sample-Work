using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : DialogPrinter
{
    public RectTransform AvatarBox, TextBox;
    public Text Name, DialogText;
    public Sprite[] sprites;

    private enum DialogState
    {
        Opening,
        Opened,
        Closing,
        Closed
    }

    private DialogState state = DialogState.Closed;
    private bool avatarBoxOpen = false;
    private bool textBoxOpen = false;
    private Vector2 avatarBoxOpenSize = new Vector2(150, 150);
    private Vector2 textBoxOpenSize = new Vector2(450, 150);
    private Vector2 avatarBoxClosedSize = new Vector2(150, -1);
    private Vector2 textBoxClosedSize = new Vector2(-1, 150);
    private float percentage = 0f;
    private Image avatar;
    private float time = 0f;

    private Data.DialogEvent dialogEvent;

    void Start ()
    {
        avatar = AvatarBox.gameObject.GetComponent<Image>();
	}
	
	void Update ()
    {
        time += Time.deltaTime;
		switch(state)
        {
            case DialogState.Opening:
                percentage += 5f * Time.deltaTime;
                if (textBoxOpen)
                {
                    state = DialogState.Opened;
                    time = 0f;
                }
                else if(avatarBoxOpen)
                {
                    TextBox.sizeDelta = Vector2.Lerp(textBoxClosedSize, textBoxOpenSize, percentage);
                    if (percentage >= 1f)
                    {
                        Name.gameObject.SetActive(true);
                        DialogText.gameObject.SetActive(true);
                        textBoxOpen = true;
                        percentage = 0f;
                    }
                    else if(percentage >= 0.5f)
                    {
                        avatar.color = Color.white;
                        avatar.sprite = sprites[(int)dialogEvent.Character];
                    }
                }
                else
                {
                    AvatarBox.sizeDelta = Vector2.Lerp(avatarBoxClosedSize, avatarBoxOpenSize, percentage);
                    if (percentage >= 1f)
                    {
                        if (time >= 0.05f)
                        {
                            avatarBoxOpen = true;
                            percentage = 0f;
                        }
                    }
                    else
                    {
                        time = 0f;
                    }
                }
                break;

            case DialogState.Closing:
                percentage += 5f * Time.deltaTime;
                if (textBoxOpen)
                {
                    Name.gameObject.SetActive(false);
                    Name.text = "";
                    DialogText.gameObject.SetActive(false);
                    DialogText.text = "";
                    TextBox.sizeDelta = Vector2.Lerp(textBoxOpenSize, textBoxClosedSize, percentage);
                    if(percentage >= 1f)
                    {
                        if (time >= 0.05f)
                        {
                            avatar.sprite = null;
                            avatar.color = new Color(0.8f, 0.8f, 0f);
                            textBoxOpen = false;
                            percentage = 0f;
                        }
                    }
                    else
                    {
                        time = 0f;
                    }
                }
                else if (avatarBoxOpen)
                {
                    AvatarBox.sizeDelta = Vector2.Lerp(avatarBoxOpenSize, avatarBoxClosedSize, percentage);
                    if (percentage >= 1f)
                    {
                        avatarBoxOpen = false;
                        percentage = 0f;
                    }
                }
                else
                {
                    state = DialogState.Closed;
                }
                break;

            case DialogState.Opened:
                string output = DialogText.text;
                bool finished = DialogPrint(ref output, dialogEvent.Text, Time.deltaTime);
                DialogText.text = output;

                if (finished)
                {
                    if (time >= dialogEvent.Time)
                    {
                        state = DialogState.Closing;
                    }
                }
                else
                {
                    time = 0f;
                }
                break;

            case DialogState.Closed:
                // do nothing
                break;

            default:
                break;
        }
	}

    public void Display(Data.DialogEvent e)
    {
        if(state == DialogState.Closed || (state == DialogState.Closing && textBoxOpen == false))
        {
            DialogPrintReset();
            dialogEvent = e;
            Name.text = e.Name;
            state = DialogState.Opening;
            avatarBoxOpen = false;
        }
    }

    public void Close()
    {
        if(state == DialogState.Opened)
        {
            state = DialogState.Closing;
        }
    }
}
