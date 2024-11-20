using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabHandler : MonoBehaviour
{
    public GameObject heldObject;
    private IXRSelectInteractor currentInteractor;

    void Start()
    {
        XRBaseInteractable[] interactables = FindObjectsOfType<XRBaseInteractable>();
        foreach (var interactable in interactables)
        {
            interactable.selectEntered.AddListener(OnGrab);
            interactable.selectExited.AddListener(OnRelease);
        }
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;
        XRBaseInteractable grabbedInteractable = (XRBaseInteractable)args.interactableObject;
        GameObject grabbedObject = grabbedInteractable.gameObject;

        heldObject = grabbedObject;
        currentInteractor = interactor;

        Debug.Log("Grabbed object: " + grabbedObject.name);
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;

        if (currentInteractor == interactor)
        {
            heldObject = null;
            currentInteractor = null;

            Debug.Log("Object has been dropped");
        }
    }

    void OnDestroy()
    {
        XRBaseInteractable[] interactables = FindObjectsOfType<XRBaseInteractable>();
        foreach (var interactable in interactables)
        {
            interactable.selectEntered.RemoveListener(OnGrab);
            interactable.selectExited.RemoveListener(OnRelease);
        }
    }
}
