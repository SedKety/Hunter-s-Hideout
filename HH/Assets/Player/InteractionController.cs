using System;
using Unity.Burst;
using UnityEngine;
using UnityEngine.InputSystem;


[BurstCompile]
public class InteractionController : MonoBehaviour
{
    private Hand hand;
    public InteractionSettingsSO settings;


    [SerializeField]
    private Transform rayTransform;

    [SerializeField]
    private Transform overlapSphereTransform;

    public Transform heldItemHolder;


    public Holdable heldObject;

    public bool isHoldingObject;

    private Holdable toPickupObject;
    public bool objectSelected;


    private Collider[] hitObjectsInSphere;
    private RaycastHit rayHit;

    private bool facingInteractable;
    private IOnHoverImpulsable currentHoveringImpulsable;
    public LayerMask interactableLayerMask;

    [SerializeField]
    private float lerpSpeed = 10f; 

    [BurstCompile]
    public void OnClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isHoldingObject == false && objectSelected)
        {
            Pickup();
        }

        if (ctx.canceled && isHoldingObject)
        {
            Drop();
        }

        if (facingInteractable && !isHoldingObject)
        {
            InteractWithInteractable();
        }
    }


    public void OnActivateClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isHoldingObject == true)
        {
            heldObject.OnItemUse();
        }
    }


    [BurstCompile]
    private  void Start()
    {
        hand = GetComponent<Hand>();

        hitObjectsInSphere = new Collider[settings.maxExpectedObjectInSphere];


        if (settings.shouldThrowVelAddMovementVel)
        {
            bodyMovementTransform = transform.root;
        }

        savedLocalVelocity = new Vector3[frameAmount];
        savedAngularVelocity = new Vector3[frameAmount];
    }


    [BurstCompile]
    private void Update()
    {
        //if you are holding nothing, scan for objects by using a raycast and a sphere around you hand
        if (isHoldingObject == false)
        {
            facingInteractable = CheckForInteractable();
            UpdateToPickObject();
        }

        //if you are holding something and it is throwable (Or "pickupsUseOldHandVel" is true), start doing velocity calculations
        if (settings.pickupsUseOldHandVel || (heldObject != null && heldObject))
        {
            CalculateHandVelocity();
        }
    }
    public void StopHoldingObject()
    {
        heldObject = null;
        isHoldingObject = false;
    }
    private bool CheckForInteractable()
    {
        // Use rayTransform.forward for the direction of the ray
        Ray ray = new Ray(rayTransform.position, rayTransform.forward);
        //Debug.DrawRay(rayTransform.position, rayTransform.forward * settings.interactRayCastRange, Color.red, 10);

        if (Physics.Raycast(ray, out RaycastHit hit, settings.interactRayCastRange, interactableLayerMask))
        {
            //print(hit.transform.name);
            if (hit.transform.gameObject.GetComponent<IOnHoverImpulsable>() != null)
            {
                currentHoveringImpulsable = hit.transform.gameObject.GetComponent<IOnHoverImpulsable>();
                hand.SendVibration(settings.selectPickupVibrationParams);
                return true;
            }
        }

        //if (Physics.Raycast(ray, out RaycastHit hirt, settings.interactRayCastRange))
        //{
        //    print(hirt.transform.name);
        //}

        return false;
    }

    private void InteractWithInteractable()
    {
        if (currentHoveringImpulsable != null)
        {
            currentHoveringImpulsable.OnClicked();
        }
    }


    [BurstCompile]
    private void UpdateToPickObject()
    {
        int interactablesLayer = settings.interactablesLayer;

        //if "GrabState.OnSphereTriger" is true (OverlapSphere is enabled)
        if (settings.grabState.HasFlag(GrabState.OnSphereTrigger))
        {
            Vector3 overlapSphereTransformPos = overlapSphereTransform.position;
            float overlapSphereSize = settings.overlapSphereSize;

            //get all objects near your hand
            int objectsInSphereCount = Physics.OverlapSphereNonAlloc(overlapSphereTransformPos, overlapSphereSize, hitObjectsInSphere, interactablesLayer);


            //resize array if there are too little spots in the Collider Array "hitObjectsInSphere"
            if (objectsInSphereCount > hitObjectsInSphere.Length)
            {
                Debug.LogWarning("Too Little Interaction Slots, Sphere check was resized");

                hitObjectsInSphere = Physics.OverlapSphere(overlapSphereTransformPos, overlapSphereSize, interactablesLayer);
            }


            //if there is atleast 1 object in the sphere
            if (objectsInSphereCount > 0)
            {

                float closestObjectDistance = 10000;
                Holdable new_ToPickupObject = null;
                Holdable targetObject;


                //calculate closest object
                for (int i = 0; i < objectsInSphereCount; i++)
                {
                    targetObject = hitObjectsInSphere[i].GetComponent<Holdable>();

                    float distanceToTargetObject = Vector3.Distance(overlapSphereTransformPos, targetObject.transform.position);

                    if (distanceToTargetObject - targetObject.objectSize < closestObjectDistance)
                    {
                        new_ToPickupObject = targetObject;
                        closestObjectDistance = distanceToTargetObject;
                    }
                }

                //if you are holding nothing, or the new_ToPickupObject isnt already selected, select the object and deselect potential previous selected object
                if (objectSelected == false || new_ToPickupObject != toPickupObject)
                {
                    SelectNewObject(new_ToPickupObject);
                }

                return;
            }
        }


        //if "GrabState.OnRaycast" is true (rayCasts are enabled) and there are no objects near your hand, check if there is one in front of your hand
        if (settings.grabState.HasFlag(GrabState.OnRaycast) && Physics.Raycast(rayTransform.position, rayTransform.forward, out rayHit, settings.pickupRayCastRange, interactablesLayer, QueryTriggerInteraction.Collide))
        {
            if (rayHit.transform.TryGetComponent(out Holdable new_ToPickupObject))
            {
                //if you are holding nothing, or the new_ToPickupObject isnt already selected, select the object and deselect potential previous selected object
                if (objectSelected == false || new_ToPickupObject != toPickupObject)
                {
                    SelectNewObject(new_ToPickupObject);
                }

                return;
            }
        }

        //deselect potential previous selected object
        DeSelectObject();
    }




    #region Select/Deselect Object

    [BurstCompile]
    private void SelectNewObject(Holdable new_ToPickupObject)
    {
        if (objectSelected)
        {
            toPickupObject.DeSelect();
        }

        toPickupObject = new_ToPickupObject;

        objectSelected = true;

        hand.SendVibration(settings.selectPickupVibrationParams);
    }

    [BurstCompile]
    private void DeSelectObject()
    {
        if (objectSelected)
        {
            toPickupObject.DeSelect();
        }

        objectSelected = false;
    }

    #endregion




    #region Drop and Pickup

    [BurstCompile]
    public void Pickup()
    {
        //if the object that is trying to be picked up by this hand, is held by the other hand and canSwapItemFromHands is false, return
        if (toPickupObject.interactable == false || (toPickupObject.heldByPlayer && settings.canSwapItemFromHands == false))
        {
            return;
        }

        toPickupObject.Pickup(this);

        heldObject = toPickupObject;
        isHoldingObject = true;

        hand.SendVibration(settings.pickupVibrationParams);
    }


    [BurstCompile]
    private void Drop()
    {
        //drop item if it is throwable
        if (heldObject.isThrowable)
        {
            Vector3 velocity = Vector3.zero;
            for (int i = 0; i < frameAmount; i++)
            {
                velocity += savedLocalVelocity[i] / frameAmount;
            }

            Vector3 angularVelocity = Vector3.zero;
            for (int i = 0; i < frameAmount; i++)
            {
                angularVelocity += savedAngularVelocity[i] / frameAmount;
            }

            heldObject.Throw(velocity, angularVelocity);
        }
        else
        {
            heldObject.Drop();
        }

        heldObject = null;
        isHoldingObject = false;
    }

    #endregion




    #region CalculateHandVelocity

    private Transform bodyMovementTransform;
    private Vector3 prevbodyTransformPos;

    private Vector3 prevTransformPos;
    private Vector3[] savedLocalVelocity;

    private Quaternion lastRotation;
    private Vector3[] savedAngularVelocity;

    public int frameAmount;
    private int frameIndex;


    [BurstCompile]
    private void CalculateHandVelocity()
    {
        //Calculate velocity based on hand movement
        savedLocalVelocity[frameIndex] = bodyMovementTransform.rotation * (transform.localPosition - prevTransformPos) * settings.throwVelocityMultiplier / Time.deltaTime;

        prevTransformPos = transform.localPosition;


        //Add velocity based on player body
        if (settings.shouldThrowVelAddMovementVel)
        {
            savedLocalVelocity[frameIndex] += (bodyMovementTransform.localPosition - prevbodyTransformPos) / Time.deltaTime;

            prevbodyTransformPos = bodyMovementTransform.localPosition;
        }


        //Calculate Angular velocity based on hand rotation
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;
        savedAngularVelocity[frameIndex] = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);

        lastRotation = transform.rotation;




        frameIndex += 1;
        if (frameIndex == frameAmount)
        {
            frameIndex = 0;
        }
    }

    #endregion

}