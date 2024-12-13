using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionMangager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> resolutionList;
    private int currentResolutionIndex = 0;
    private const string ResolutionIndexKey = "ResolutionIndex";

    [System.Obsolete]
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionList = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        float savedRefreshRate = PlayerPrefs.GetFloat("RefreshRate", Screen.currentResolution.refreshRate);
        foreach (var res in resolutions)
        {
            if (res.refreshRate == savedRefreshRate)
            {
                resolutionList.Add(res);
            }
        }
        List<string> options = new();
        foreach (var res in resolutionList)
        {
            string option = $"{res.width}x{res.height} {res.refreshRate} Hz";
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        if (PlayerPrefs.HasKey(ResolutionIndexKey))
        {
            currentResolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey);
        }
        else
        {
            for (int i = 0; i < resolutionList.Count; i++)
            {
                if (resolutionList[i].width == Screen.width && resolutionList[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }
        }
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ApplyResolution(currentResolutionIndex);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    [System.Obsolete]
    public void SetResolution(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex;
        ApplyResolution(currentResolutionIndex);
        PlayerPrefs.SetInt(ResolutionIndexKey, currentResolutionIndex);
        PlayerPrefs.SetFloat("RefreshRate", resolutionList[resolutionIndex].refreshRate);
        PlayerPrefs.Save();
    }
    private void ApplyResolution(int resolutionIndex)
    {
        Resolution resolution = resolutionList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
