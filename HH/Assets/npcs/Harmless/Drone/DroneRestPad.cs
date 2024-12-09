using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRestPad : MonoBehaviour
{
    [Header("Drone Waypoints")]
    public Transform[] droneWaypoints;
    public Transform[] dronePackageWaypoints;
    public Transform restPoint;


    [Header("Drone variables")]
    [SerializeField] GameObject droneGO;
    public Drone drone;

    public void Start()
    {
        DroneManager.restPads.Add(this);
        drone = Instantiate(droneGO, restPoint.position, Quaternion.identity).GetComponent<Drone>();
        drone.droneRestPad = this;
    }
}
