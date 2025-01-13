using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VrButton : MonoBehaviour, IOnHoverImpulsable
{
    [SerializeField] UnityEvent onClickEvent;
    public void OnClicked()
    {
        onClickEvent.Invoke();
    }
}
