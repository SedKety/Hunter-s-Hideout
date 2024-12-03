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
    private NavMeshAgent agent;
    public DroneRestPad droneRestPad;

    private GameObject currentTarget;

    [SerializeField] Transform heldItemSpawnPoint;
    [SerializeField] GameObject currentHeldItemGO;
    [SerializeField] GameObject packageGO;

    public DroneStates state;

    public float floatingHeight;

    public bool canDropPackage;

    public float interactionRange;


    public float movementSpeed;

    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        state = DroneStates.resting;
    }

    public void ActOnState(DroneStates newState)
    {
        state = newState;
        switch (state)
        {
            case DroneStates.resting:
                {
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

    private IEnumerator FlyToLastWaypointThenTarget(Entity entity)
    {
        // Get the waypoints in the original order
        Transform[] waypoints = droneRestPad.droneWaypoints.ToArray();

        // Disable NavMeshAgent during waypoint traversal
        agent.enabled = false;

        // Follow all the waypoints in order (using direct movement instead of NavMeshAgent)
        foreach (Transform waypoint in waypoints)
        {
            while (Vector3.Distance(transform.position, waypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * movementSpeed);
                yield return null;
            }
        }

        // Now, enable NavMeshAgent to move to the entity's position
        agent.enabled = true;
        agent.SetDestination(entity.transform.position);

        // Wait until the drone reaches the target GameObject
        while (Vector3.Distance(transform.position, entity.transform.position) > interactionRange)
        {
            yield return null;
        }

        // Retrieve the HarmlessEntity component from the target
        HarmlessEntity harmlessEntity = entity.GetComponent<HarmlessEntity>();
        if (harmlessEntity != null && harmlessEntity.headGO != null)
        {
            // Instantiate the EntityHead at the heldItemSpawnPoint
            GameObject entityHeadInstance = Instantiate(harmlessEntity.headGO, heldItemSpawnPoint.position, heldItemSpawnPoint.rotation);

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
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, Time.deltaTime * movementSpeed);
                yield return null;
            }
        }

        // Return to the initial state
        ActOnState(DroneStates.resting);
        Debug.Log("Drone has completed the waypoint path in reverse.");
    }




    protected override void OnDeath()
    {
        if (canDropPackage)
        {

        }
    }
}
