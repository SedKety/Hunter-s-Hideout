using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Flags]
public enum GrabState : byte
{
    OnSphereTrigger = 1,
    OnRaycast = 2,
}


[CreateAssetMenu(fileName = "Default Settings", menuName = "VR Interaction/Interaction Settings")]
public class InteractionSettingsSO : ScriptableObject
{
    [Header("What pickup methods to use")]
    public GrabState grabState = (GrabState)3;

    [Header("What layer should the interactables be on?")]
    public LayerMask interactablesLayer;
     
    [Header("Raycast forward pickup range")]
    public float pickupRayCastRange = 0.65f;
    public float interactRayCastRange = 1.5f;

    [Header("Distance to hand pickup range")]
    public float overlapSphereSize = .2f;


    [Header("Can your hand pickup an object in you other hand?")]
    public bool canSwapItemFromHands = true;

    [Header("Should movement velocity be added to throw velocity?")]
    public bool shouldThrowVelAddMovementVel = true;

    [Header("Should a newly picked up object gain hand previous velocity?")]
    public bool pickupsUseOldHandVel = true;


    [Header("Does not affect the velocity clamp of interactables")]
    public float throwVelocityMultiplier = 1.5f;

    [Header("Array Size of the OverlapSphere")]
    public int maxExpectedObjectInSphere = 15;

    [Header("Vibration of controller when object is picked up")]
    public VibrationParamaters pickupVibrationParams = new VibrationParamaters(0.6f, 0.25f);

    [Header("\nVibration of controller when object is selected for picking up")]
    public VibrationParamaters selectPickupVibrationParams = new VibrationParamaters(.05f, 0.1f);
}