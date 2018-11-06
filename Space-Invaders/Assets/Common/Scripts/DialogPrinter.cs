using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPrinter : MonoBehaviour
{
    private float printTime = 0;
    private float printInterval = 0f;
    private float speed = 0f;
    private List<string> tags = new List<string>();
    private int tagsLength = 0;
    private int printIndex = 0;

    public bool DialogPrint(ref string current, string target, float deltaTime)
    {
        printTime += deltaTime;
        if (printIndex < target.Length)
        {
            if (printTime < printInterval)
                return false;

            printTime -= printInterval;
            char thisChar = target[printIndex++];
            bool pause = false;
            float pauseLength = 0f;
            while (thisChar == '<')
            {
                bool closeTag = (target[printIndex] == '/');
                if (closeTag) printIndex++;
                string tag = target.Substring(printIndex, target.IndexOf('>', printIndex) - printIndex);
                string richTag = "", richValue = "";
                printIndex += tag.Length + 1;

                if (closeTag)
                {
                    if (tag == "speed")
                    {
                        speed = 0f;
                    }
                    else if (tag == tags[tags.Count - 1])
                    {
                        tagsLength -= tag.Length + 3;
                        tags.RemoveAt(tags.Count - 1);
                    }
                }
                else
                {
                    int richLength = tag.IndexOf('=');
                    if (richLength == -1)
                    {
                        richTag = tag;
                    }
                    else
                    {
                        richTag = tag.Substring(0, richLength);
                        richValue = tag.Substring(richLength + 1, tag.Length - richLength - 1);
                    }

                    if (richTag == "pause")
                    {
                        pause = true;
                        pauseLength = float.Parse(richValue);
                    }
                    else if (richTag == "speed")
                    {
                        speed = float.Parse(richValue);
                    }
                    else
                    {
                        current = current.Insert(current.Length - tagsLength, "<" + tag + "></" + richTag + ">");
                        tags.Add(richTag);
                        tagsLength += richTag.Length + 3;
                    }
                }

                if (printIndex < target.Length)
                    thisChar = target[printIndex++];
                else
                    return true;
            }

            if (tags.Count > 0)
            {
                current = current.Insert(current.Length - tagsLength, thisChar.ToString());
            }
            else
            {
                current += thisChar;
            }

            if(pause)
            {
                printInterval = pauseLength;
            }
            else
            {
                if (speed > 0)
                {
                    printInterval = speed;
                }
                else
                {
                    printInterval = 0.04f;
                }
            }
            return false;
        }
        return true;
    }

    public void DialogPrintReset()
    {
        printTime = 0f;
        printIndex = 0;
        tags.Clear();
        tagsLength = 0;
    }
}
