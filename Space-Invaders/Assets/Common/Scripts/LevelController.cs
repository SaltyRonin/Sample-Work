using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class LevelController : MonoBehaviour
{
    public Image MissionScreen, MissionComplete, MissionFailure;
    public Text MissionText, MissionTitle, CompleteText, ScoreText, ContinueText, FailText, RetryText;

    private bool screenFaded, textFaded;
    private float percentage = 0f;

    public abstract void Finished();

    public void OnFinish()
    {
        ScoreText.text = string.Format("MEMES DESTROYED: {0}", Data.LevelScore);
        MissionComplete.gameObject.SetActive(true);
    }

    public void OnFailure()
    {
        MissionFailure.gameObject.SetActive(true);
    }

    public void DisableMissionText()
    {
        MissionText.gameObject.SetActive(false);
        MissionTitle.gameObject.SetActive(false);
    }

    public bool FadeIn()
    {
        percentage += Time.deltaTime;

        if (!screenFaded)
        {
            MissionScreen.color = Color.Lerp(Color.black, Color.clear, percentage);
            if (percentage >= 1f)
            {
                percentage = 0f;
                screenFaded = true;
            }
        }
        else if (!textFaded)
        {
            MissionText.color = Color.Lerp(Color.white, Color.clear, percentage);
            MissionTitle.color = Color.Lerp(Color.white, Color.clear, percentage);
            if (percentage >= 1f)
            {
                percentage = 0f;
                textFaded = true;
            }
        }
        else
        {
            textFaded = false;
            screenFaded = false;
            MissionScreen.gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    public bool FadeOut()
    {
        percentage += Time.deltaTime;

        if (!textFaded)
        {
            CompleteText.color = Color.Lerp(Color.clear, Color.white, percentage);
            ScoreText.color = Color.Lerp(Color.clear, Color.white, percentage);
            if (percentage >= 1f)
            {
                percentage = 0f;
                textFaded = true;
            }
        }
        else if (!screenFaded)
        {
            MissionComplete.color = Color.Lerp(Color.clear, Color.black, percentage);
            if(ContinueText)
            {
                ContinueText.color = Color.Lerp(Color.clear, Color.white, (percentage - 1f));
                if (percentage >= 2f)
                {
                    percentage = 0f;
                    screenFaded = true;
                }
            }
            else
            {
                if (percentage >= 1f)
                {
                    percentage = 0f;
                    screenFaded = true;
                }
            }
        }
        else
        {
            textFaded = false;
            screenFaded = false;
            return true;
        }

        return false;
    }

    public bool FadeToNext()
    {
        percentage += Time.deltaTime;

        if (!textFaded)
        {
            CompleteText.color = Color.Lerp(Color.white, Color.clear, percentage);
            ScoreText.color = Color.Lerp(Color.white, Color.clear, percentage);
            if(ContinueText) ContinueText.color = Color.Lerp(Color.white, Color.clear, percentage);
            FailText.color = Color.Lerp(Color.white, Color.clear, percentage);
            RetryText.color = Color.Lerp(Color.white, Color.clear, percentage);
            if (percentage >= 1.5f)
            {
                percentage = 0f;
                textFaded = true;
            }
        }
        else
        {
            textFaded = false;
            return true;
        }

        return false;
    }

    public bool Failure()
    {
        percentage += Time.deltaTime;

        if (!textFaded)
        {
            FailText.color = Color.Lerp(Color.clear, Color.white, percentage);
            if (percentage >= 1f)
            {
                percentage = 0f;
                textFaded = true;
            }
        }
        else if (!screenFaded)
        {
            MissionFailure.color = Color.Lerp(Color.clear, Color.black, percentage);
            RetryText.color = Color.Lerp(Color.clear, Color.white, (percentage - 1f));
            if (percentage >= 2f)
            {
                percentage = 0f;
                screenFaded = true;
            }
        }
        else
        {
            textFaded = false;
            screenFaded = false;
            return true;
        }

        return false;
    }
}
