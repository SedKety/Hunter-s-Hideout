using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
[RequireComponent(typeof(XRGrabInteractable))]
public abstract class Holdable : MonoBehaviour, IUsable
{
    //this method is called to perform tasks assigned to certain holdables upon using, such as healing, shooting and filling hunger.
    public abstract void OnItemUse();
}
