using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum DroneStates
{
    active,
    resting,
}
public class Drone : Entity
{
    public DroneRestPad droneRestPad;

    private GameObject currentTarget;

    [SerializeField]private Transform heldItemSpawnPoint;
    private GameObject currentHeldItemGO;

    [SerializeField] private Transform[] propellors;
    [SerializeField] private bool shouldRotatePropellors;
    [SerializeField] private float propellorRotationSpeed;
    public DroneStates state;

    public float floatingHeight;

    public bool canDropPackage;

    public float interactionRange;


    public float headRetrievementSpeed;
    public float packageRetrievementSpeed;

    public AudioSource droneFly;

    protected override void Awake()
    {
        base.Awake();
        state = DroneStates.resting;
        agent.speed = headRetrievementSpeed;
        agent.enabled = false;
    }

    public void ActOnState(DroneStates newState)
    {
        state = newState;
        switch (state)
        {
            case DroneStates.resting:
                {
                    shouldRotatePropellors = false;
                    break;
                }
            case DroneStates.active:
                {
                    break;
                }
        }
    }
    public void ActOnStateRetrieve(Entity entity)
    {
        ActOnState(DroneStates.active);
        StartCoroutine(FlyToLastWaypointThenTarget(entity));
    }
    public IEnumerator RetrievePackage(GameObject package)
    {
        shouldRotatePropellors = true;
        StartCoroutine(RotatePropellors());
        ActOnState(DroneStates.active);
        agent.enabled = false;
        Transform[] waypoints = droneRestPad.dronePackageWaypoints;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == waypoints.Last()) { break; }
            while (Vector3.Distance(transform.position, waypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * packageRetrievementSpeed);
                yield return null;
            }
        }

        var heldItem = Instantiate(package, heldItemSpawnPoint.position, heldItemSpawnPoint.rotation);

        heldItem.transform.SetParent(heldItemSpawnPoint);
        if (heldItem.GetComponent<Rigidbody>() != null)
        {
            heldItem.GetComponent<Rigidbody>().isKinematic = true;
        }
        //The drone flies back to his assigned restpad

        waypoints.Reverse();

        foreach (Transform waypoint in waypoints)
        {
            while (Vector3.Distance(transform.position, waypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * packageRetrievementSpeed);
                yield return null;
            }
        }

        while (Vector3.Distance(transform.position, droneRestPad.packageDropoffPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, droneRestPad.packageDropoffPoint.position, Time.deltaTime * headRetrievementSpeed);
            yield return null;
        }
        if (heldItem != null)
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Rigidbody>().isKinematic = false;
        }


        while (Vector3.Distance(transform.position, droneRestPad.restPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, droneRestPad.restPoint.position, Time.deltaTime * packageRetrievementSpeed);
            yield return null;
        }
        ActOnState(DroneStates.resting);
        yield return null;
    }
    private IEnumerator FlyToLastWaypointThenTarget(Entity entity)
    {
        shouldRotatePropellors = true;
        StartCoroutine(RotatePropellors());
        Transform[] waypoints = droneRestPad.droneWaypoints;

        agent.enabled = false;

        // Follow all the waypoints in order
        foreach (Transform waypoint in waypoints)
        {
            while (Vector3.Distance(transform.position, waypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * headRetrievementSpeed);
                yield return null;
            }
        }


        agent.enabled = true;
        agent.SetDestination(entity.transform.position);

        // Wait until the drone reaches the target GameObject
        while (Vector3.Distance(transform.position, entity.transform.position) > interactionRange)
        {
            yield return null;
        }

        // Retrieve the HarmlessEntity component from the target
        Entity harmlessEntity = entity.GetComponent<Entity>();
        GameObject entityHeadInstance = null;
        if (harmlessEntity != null && harmlessEntity._headGo != null)
        {
            // Instantiate the EntityHead at the heldItemSpawnPoint
            entityHeadInstance = Instantiate(harmlessEntity._headGo, heldItemSpawnPoint.position, heldItemSpawnPoint.rotation);

            // Optionally, parent the instantiated object to the heldItemSpawnPoint
            entityHeadInstance.transform.SetParent(heldItemSpawnPoint);
            entityHeadInstance.GetComponent<Rigidbody>().isKinematic = true;

            // Update the currentHeldItemGO
            currentHeldItemGO = entityHeadInstance;

            Debug.Log("EntityHead successfully retrieved and held.");
        }
        else
        {
            Debug.LogWarning("EntityHead is null or HarmlessEntity component is missing.");
        }
        Destroy(entity.gameObject);

        // Move back to the original waypoint

        agent.SetDestination(waypoints.Last().position);

        while (Vector3.Distance(transform.position, waypoints.Last().position) > interactionRange)
        {
            yield return null;
        }
        agent.enabled = false;

        // Reverse the waypoints array to follow them in reverse order
        waypoints = waypoints.Reverse().ToArray();

        // Follow the waypoints in reverse order (again, using direct movement)
        foreach (Transform waypoint in waypoints)
        {
            while (Vector3.Distance(transform.position, waypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * headRetrievementSpeed);
                yield return null;
            }
        }
        //Go to the dropPoint
        while (Vector3.Distance(transform.position, droneRestPad.packageDropoffPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, droneRestPad.packageDropoffPoint.position, Time.deltaTime * headRetrievementSpeed);
            yield return null;
        }
        //Drop the head
        if (entityHeadInstance != null)
        {
            entityHeadInstance.transform.SetParent(null);
            entityHeadInstance.GetComponent<Rigidbody>().isKinematic = false;
        }

        //Go to the RestPoint
        while (Vector3.Distance(transform.position, droneRestPad.restPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, droneRestPad.restPoint.position, Time.deltaTime * headRetrievementSpeed);
            yield return null;
        }
        transform.rotation = droneRestPad.restPoint.rotation;
        // Return to the initial state
        ActOnState(DroneStates.resting);
        Debug.Log("Drone has completed the waypoint path in reverse.");
    }

    protected IEnumerator RotatePropellors()
    {
        droneFly.Play();
        while (shouldRotatePropellors)
        {
            foreach (Transform propellor in propellors)
            {
                propellor.Rotate(0, propellorRotationSpeed, 0);
            }
            yield return new WaitForSeconds(0.01f);
        }
        droneFly.Stop();
        yield return null;
    }
}
