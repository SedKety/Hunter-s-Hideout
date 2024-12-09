using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Laptop : MonoBehaviour
{
    public Transform appParent;  
    public LaptopApp[] apps;     
    public GameObject appIconGO; 

    void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < apps.Length; i++)
        {
            var appIcon = Instantiate(appIconGO, appParent);

            var appIconButton = appIcon.GetComponent<Button>();


            var iconImage = appIcon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = apps[i].appIcon;
            }
            else
            {
                Debug.LogWarning("App Icon GameObject is missing an Image component.");
            }
            appIcon.name = $"AppIcon_{i}";
        }
    }
}
