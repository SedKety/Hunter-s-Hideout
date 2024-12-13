using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FpsManager : MonoBehaviour
{
    public TMP_InputField inputText;
    private const string FrameRateKey = "TargetFrameRate";
    private void Start()
    {
        int savedFPS = PlayerPrefs.GetInt("TargetFPS", -1);
        inputText.text = savedFPS.ToString();
        ChangeFPS(savedFPS.ToString());
    }
    public void ChangeFPS(string target)
    {
        int fps = int.Parse(target);
        if (fps == 0)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = fps;
        }
        PlayerPrefs.SetInt("TargetFPS", fps);
    }
}
