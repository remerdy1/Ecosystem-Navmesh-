using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class AgentController : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    //todo make private
    public FOV fov { get; protected set; }
    public List<Transform> rejectionList { get; private set; } = new List<Transform>(); //todo forget after x minutes

    public enum Esex
    {
        MALE = 0,
        FEMALE = 1
    }

    // Stats
    [SerializeField] protected float hunger;
    [SerializeField] protected float thirst;
    [SerializeField] protected float energy;

    [SerializeField] protected float hungerDecreaseRate;
    [SerializeField] protected float thirstDecreaseRate;
    [SerializeField] protected float energyDecreaseRate;

    [SerializeField] protected float hungerThreshold;
    [SerializeField] protected float thirstThreshold;

    [SerializeField] public Esex sex { get; private set; }
    [SerializeField] public int attractiveness { get; private set; }
    [SerializeField] protected bool canMate;

    [SerializeField] protected float rotationSpeed = 1f;

    //todo
    //todo Speed
    //todo Metabolism
    //todo Field Of View

    // Movement
    public float movementRange { get; protected set; }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FOV>();

        //todo randomise stats
        hungerThreshold = 30f;
        thirstThreshold = 30f;
        movementRange = 30f;

        hunger = Random.Range(hungerThreshold, 100);
        thirst = Random.Range(hungerThreshold, 100);
        energy = Random.Range(hungerThreshold, 100);

        sex = (Esex)Random.Range(0, 1);
        attractiveness = Random.Range(1, 10);
        canMate = false;

        hungerDecreaseRate = 5f;
        thirstDecreaseRate = 5f;
        energyDecreaseRate = 5f;
    }

    public bool IsHungry()
    {
        return hunger < hungerThreshold;
    }

    public bool IsThirsty()
    {
        return thirst < thirstThreshold;
    }

    public bool CanMate()
    {
        return canMate;
    }

    public void MoveToRandomPosition()
    {
        Vector3 newPosition = GetRandomPosition(transform.position, movementRange);

        Debug.DrawRay(newPosition, Vector3.up, Color.blue, 1.0f);
        navMeshAgent.SetDestination(newPosition);
    }

    public void MoveToTarget(Transform target)
    {
        Debug.Log($"Moving to {target.position}");
        Debug.DrawRay(target.position, Vector3.up, Color.green, 1f);
        navMeshAgent.SetDestination(target.position);
    }

    public void ResetPath()
    {
        navMeshAgent.ResetPath();
    }

    public void RotateTowardTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public bool AtTarget()
    {
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    public float DistanceToTarget(Transform target)
    {
        return Vector3.Distance(target.position, transform.position);
    }

    public Transform FindPotentialMate()
    {
        List<Transform> preyInViewRadius = fov.preyInViewRadius;

        foreach (Transform other in preyInViewRadius)
        {
            AgentController otherController = other.GetComponent<AgentController>();

            if (otherController.sex != sex && canMate && otherController.canMate)
            {
                return other;
            }
            else
            {
                rejectionList.Add(other);
            }
        }

        return null;
    }

    public bool MateRequest(AgentController other)
    {
        float chance = Math.Max((other.attractiveness / 10) * 100, 30);

        float roll = Random.Range(0, 100);

        if (roll > chance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Vector3 GetRandomPosition(Vector3 currentPosition, float movementRange)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPosition = currentPosition + Random.insideUnitSphere * movementRange;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPosition, out hit, movementRange, NavMesh.AllAreas))
            {
                return hit.position;

            }
        }

        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            hunger += 10; //todo change depending on matabolism
            fov.foodInViewRadius.Remove(other.gameObject.transform);
            fov.visibleFood.Remove(other.gameObject.transform);
            Destroy(other.gameObject);
        }
    }
}