using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItem : MonoBehaviour
{
    public Holdable heldItem;
    public Transform heldItemPos;

    public void Update()
    {
        if (Input.GetButton("XRI_Right_Grip"))
        {

        }
    }
    public void OnButtonPressed()
    {
        heldItem.OnHeldItemUse();
    }
}
