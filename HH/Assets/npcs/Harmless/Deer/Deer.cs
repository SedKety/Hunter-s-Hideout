using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Deer : HarmlessEntity
{
    private Vector3 currentDestination;


    [Header("Deer-only variables")]
    public HarmlessEntityStates startState;

    public float scaredTime;

    //the higher this is the faster it will return to standing/searching state
    public float scaredTimeMultiplier;

    //this decides how far away the deer can be from its position untill it will go to a new one
    public float distanceOffsetTillNextPosition;

    //the higher this value is the higher the chance is that the deer will move sporadically each frame
    //public float sporadicChance;

    //decides how far away the deer can pick a position
    public float distanceRange;

    private bool isRunning;

    [Tooltip("The x is the minumum value and the Y is the maximum value")]
    public Vector2 standStillTime;

    public float standStillChance;
    private Coroutine coroutine;

    public override void Start()
    {
        ActOnState(startState);
    }
    public  override void ActOnState(HarmlessEntityStates _state)
    {
        currentState = _state;
        print("Deer is entering: " + _state + "  state");
        switch (_state)
        {
            case HarmlessEntityStates.scared:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(RunAway()); break;
                }
            case HarmlessEntityStates.running:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine =  StartCoroutine(RunAway()); break;
                }
            case HarmlessEntityStates.standing:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    StartCoroutine(StandStill()); break;
                }
            case HarmlessEntityStates.searching:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    StartCoroutine(Search()); break;
                }
        }
    }

    public override bool GetPosToMoveTo()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * distanceRange;
        print(randomPos);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 100, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
            currentDestination = hit.position;
            print(hit.position);
            return true;
        }
        else
        {
            print("No position found");
        }
        return false;
    }

    public IEnumerator Search()
    {
        GetPosToMoveTo();
        while (currentState == HarmlessEntityStates.searching)
        {
            yield return new WaitForSeconds(0.1f);
            if (Vector3.Distance(transform.position, currentDestination) <= distanceOffsetTillNextPosition)
            {
                if (Random.Range(0, 100) <= standStillChance)
                {
                    ActOnState(HarmlessEntityStates.standing);
                    break;
                }
                GetPosToMoveTo();
            }
        }
    }
    public IEnumerator StandStill()
    {
        yield return new WaitForSeconds(Random.Range(standStillTime.x, standStillTime.y));
        ActOnState(HarmlessEntityStates.searching);
    }
    public IEnumerator RunAway()
    {
        ChangeSpeed(runSpeed);
        isRunning = true;
        GetPosToMoveTo();
        float timeTillEnd = scaredTime;
        while (timeTillEnd > 0)
        {
            print(timeTillEnd.ToString());
            timeTillEnd -= Time.deltaTime * scaredTimeMultiplier;
            if (Vector3.Distance(transform.position, currentDestination) <= distanceOffsetTillNextPosition)
            {
                GetPosToMoveTo();
            }
            yield return null;
        }
        ChangeSpeed(normalSpeed);
        ActOnState(HarmlessEntityStates.searching);
        isRunning = false;
        yield return null;
    }
    public override void OnDeath()
    {

    }
    
    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
        ActOnState(HarmlessEntityStates.scared);
    }

    public void ChangeSpeed(float speed)
    {
        agent.speed = speed;
    }
}
