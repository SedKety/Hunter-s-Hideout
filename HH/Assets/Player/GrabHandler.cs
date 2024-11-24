using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabHandler : MonoBehaviour
{
    public GameObject heldObject;
    [Tooltip("This will decide on how much the player has to hold the trigger button to activate")]
    [Range(0f, 1f)]
    public float itemUseThreshold;
    public InputActionReference triggerInput;
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

    private void Update()
    {
        if (triggerInput.action.ReadValue<float>() >= itemUseThreshold & IsValidObject())
        {
            heldObject.GetComponent<IUsable>().OnItemUse();
        }
    }
    //Checks whether there is an heldObject and whether the heldObject has the Holdable script
    public bool IsValidObject()
    {
        if (heldObject != null && heldObject.GetComponent<IUsable>() != null)
        {
            return true;
        }
        return false;
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
