using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public enum EntityStates
{
    searching,
    standing,
    running,
    attacking,
    fleeing,
    dead,
    nothing,
}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class Entity : MonoBehaviour, IDamagable
{
    public EntityStats npcStats;
    protected NavMeshAgent agent;
    protected Animator animator;

    //decides how close the entity can be away from an object before interacting with it
    #region npcstat variables

    //Health variables
    protected int _health;
    protected bool _canHeal;
    protected int _healthRegen;
    protected float _healthRegenTimer;

    //Scared variables
    protected float _scaredTime;
    protected Vector2 _standStillTime;
    protected float _scaredTimeMultiplier;

    //Movement variables
    protected float _distanceOffsetTillNextPosition;
    protected float _movementSpeedNormal;
    protected float _movementSpeedSprint;
    protected float _distanceRange;

    //Attacking variables
    protected int _damage;
    protected int _hitCount;
    protected float _hitDelay;
    protected int _contactWithdrawalChance;
    protected float _attackChance;
    
    //Misc variables
    [HideInInspector] public GameObject _headGo;
    protected EntityStates _startState;
    #endregion

    //The position the entity will/wants to move to
    protected Vector3 currentDestination;

    protected bool isDead;

    [SerializeField] protected EntityStates _currentState;


    protected Coroutine coroutine;


    protected bool isRunning;


    protected float standStillChance;


    [SerializeField] bool shouldDebugPrint;

    public AudioSource hit;
    //Take the stats from the npcStats scriptable object and applies those to this object
    protected virtual void Awake()
    {
        isDead = false;
        InitStats();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ActOnState(_startState);
    }
    protected virtual void Start()
    {

    }
    public virtual void InitStats()
    {
        //Movement variables
        _distanceRange = npcStats.distanceRange;
        _movementSpeedNormal = npcStats.movementSpeedNormal;
        _movementSpeedSprint = npcStats.movementSpeedSprint;
        _distanceOffsetTillNextPosition = npcStats.distanceOffsetTillNextPosition;

        //Scared Variables
        _scaredTime = npcStats.scaredTime;
        _standStillTime = npcStats.standStillTime;
        _scaredTimeMultiplier = npcStats.scaredTimeMultiplier;

        //Attacking variables
        _damage = npcStats.damage;
        _hitCount = npcStats.hitCount;
        _contactWithdrawalChance = npcStats.contactWithdrawalChance;
        _hitDelay = npcStats.hitDelay;

        //Health variables
        _health = npcStats.health;
        if (npcStats.canHeal == true)
        {
            _healthRegen = npcStats.healthRegen;
            _healthRegenTimer = npcStats.healthRegenTimer;
            _canHeal = npcStats.canHeal;
        }

        //Misc variables
        _startState = npcStats.startState;
        _headGo = npcStats.headGO;
    }
    //Makes the entity respond to certain states
    protected virtual void ActOnState(EntityStates _state)
    {
        if (shouldDebugPrint) { print(_state); }
        if (isDead) return;
        _currentState = _state;
        //print("Entity is entering: " + _state + "  state");
        switch (_state)
        {
            case EntityStates.fleeing:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(RunAway()); break;
                }
            case EntityStates.running:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(RunAway()); break;
                }
            case EntityStates.standing:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    StartCoroutine(StandStill()); break;
                }
            case EntityStates.searching:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    StartCoroutine(Search()); break;
                }
            case EntityStates.dead:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    agent.destination = transform.position;
                    isDead = true;
                    break;
                }
            case EntityStates.attacking:
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    StartCoroutine(Attack());
                    break;
                }
            case EntityStates.nothing:
                break;
        }
    }

    [ContextMenu("TakeDamage")]
    public virtual void TakeDamage(int damageTaken)
    {
        hit.Play();
        if (shouldDebugPrint) { print(gameObject.name + " Has taken " + damageTaken + ": Damage");  } 
        _health -= damageTaken;
        if (_health <= 0)
        {
            OnDeath();
        }
    }


    [ContextMenu("OnDeath")]
    protected virtual void OnDeath()
    {
        if (shouldDebugPrint) { print(gameObject.name + " Has died"); }
        if (!isDead)
        {
            EntityManager.CallOnEntityDeath(this);
            ActOnState(EntityStates.dead);
            animator.SetBool("IsDead", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunnning", false);
            animator.SetBool("IsIdle", false);
        }
    }


    protected virtual IEnumerator StandStill()
    {
        //doesnt animate anymore
        animator.SetBool("IsIdle", true);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunnning", false);
        yield return new WaitForSeconds(Random.Range(_standStillTime.x, _standStillTime.y));
        if (_currentState != EntityStates.standing) { yield return null; }
        ActOnState(EntityStates.searching);
    }
    protected virtual IEnumerator Search()
    {
        GetRandomPosToMoveTo();
        ChangeSpeed(_movementSpeedNormal);
        //switches to the movementAnimation
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsRunnning", false);
        while (_currentState == EntityStates.searching)
        {
            yield return new WaitForSeconds(0.1f);
            if (Vector3.Distance(transform.position, currentDestination) <= _distanceOffsetTillNextPosition)
            {
                if (Random.Range(0, 100) <= standStillChance)
                {
                    ActOnState(EntityStates.standing);
                    break;
                }
                GetRandomPosToMoveTo();
            }
        }
    }
    //Tries to generate a position to move to, will return positive if it has found one
    public virtual bool GetRandomPosToMoveTo()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * _distanceRange;
        //print(randomPos);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 100, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
            currentDestination = hit.position;
            // print(hit.position);
            return true;
        }
        else
        {
            print("No position found");
        }
        return false;
    }

    protected virtual IEnumerator RunAway()
    {
        ChangeSpeed(_movementSpeedSprint);
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        GetRandomPosToMoveTo();
        float timeTillEnd = _scaredTime;
        while (timeTillEnd > 0)
        {
            timeTillEnd -= Time.deltaTime * _scaredTimeMultiplier;
            if (Vector3.Distance(transform.position, currentDestination) <= _distanceOffsetTillNextPosition)
            {
                GetRandomPosToMoveTo();
            }
            yield return null;
        }
        ChangeSpeed(_movementSpeedNormal);
        ActOnState(EntityStates.searching);
        yield return null;
    }
    protected virtual void ChangeSpeed(float speed)
    {
        agent.speed = speed;
    }

    protected virtual IEnumerator Attack()
    {
        currentDestination = Hideout.GetHideOutPosClosestToV3(transform.position).position;
        ChangeSpeed(_movementSpeedSprint);
        agent.destination = currentDestination;

        //Checks whether the entity has reached the destination of the agent
        while(Vector3.Distance(transform.position, currentDestination) >= _distanceOffsetTillNextPosition)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        //Rolls a random number to see whether the entity should or should not flee and should continue attacking
        var chance = Random.Range(0, 100);
        if (chance <= _contactWithdrawalChance) { ActOnState(EntityStates.fleeing); yield return null; }

        bool isAttacking = true;

        while (isAttacking)
        {
            for (int i = 0; i < _hitCount; i++)
            {
                Hideout.instance.TakeDamage(_damage);
                yield return new WaitForSeconds(_hitDelay);
            }
            isAttacking = false;
        }
        ActOnState(EntityStates.fleeing);
        yield return null;
    }
}
