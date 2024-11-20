using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Holdable : MonoBehaviour
{
    //this method is called when the item is been used
    public abstract void OnHeldItemUse();
}
