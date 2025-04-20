using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public abstract class AgentController : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    //todo make private
    public FOV fov { get; protected set; }
    public List<Transform> rejectionList { get; private set; } = new List<Transform>(); //todo forget after x minutes
    [field: SerializeField] public WaterController waterController;

    public enum Esex
    {
        MALE = 0,
        FEMALE = 1
    }

    // Stats
    [field: SerializeField] protected float hunger;
    [field: SerializeField] protected float thirst;
    // [field: SerializeField] protected float energy;

    [field: SerializeField] protected float hungerDecreaseRate;
    [field: SerializeField] protected float thirstDecreaseRate;
    // [field: SerializeField] protected float energyDecreaseRate;

    [field: SerializeField] protected float hungerThreshold;
    [field: SerializeField] protected float thirstThreshold;

    [field: SerializeField] public Esex sex { get; private set; }
    [field: SerializeField] public int attractiveness { get; private set; }
    [field: SerializeField] protected bool canMate;
    [field: SerializeField] protected float canMateResetTimer;
    [field: SerializeField] protected Transform mate;

    [field: SerializeField] protected float rotationSpeed = 1f;

    //todo
    //todo Speed
    //todo Metabolism
    //todo Field Of View

    // Movement
    public float movementRange { get; protected set; }


    public abstract GameObject GetPrefab();

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FOV>();

        hungerThreshold = Random.Range(30, 51);
        thirstThreshold = Random.Range(30, 51);
        movementRange = Random.Range(30, 51);

        hunger = Random.Range(hungerThreshold, 100);
        thirst = Random.Range(hungerThreshold, 100);
        // energy = Random.Range(hungerThreshold, 100);

        sex = (Esex)Random.Range(0, 2);
        attractiveness = Random.Range(1, 11);
        canMateResetTimer = Random.Range(30, 120);
        StartCoroutine(ResetCanMate(30)); // Can't mate for the first 30 seconds of spawning
        mate = null;

        hungerDecreaseRate = GetDecreaseRate();
        thirstDecreaseRate = GetDecreaseRate();
        // energyDecreaseRate = GetRandomFloat(0, 1, 0.25f);

        InvokeRepeating("DecrementHunger", 1, 1);
        InvokeRepeating("DecrementThirst", 1, 1);
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

    public bool FoundMate()
    {
        return mate != null;
    }

    public void SetMate(Transform mate)
    {
        this.mate = mate;
    }

    public Transform GetMate()
    {
        return mate;
    }

    public void MoveToRandomPosition()
    {
        Vector3 newPosition = GetRandomPosition(transform.position, movementRange);

        Debug.DrawRay(newPosition, Vector3.up, Color.blue, 1.0f);
        navMeshAgent.SetDestination(newPosition);
    }

    public void MoveToTarget(Transform target)
    {
        // Debug.Log($"Moving to {target.position}");
        Debug.DrawRay(target.position, Vector3.up, Color.green, 1f);
        navMeshAgent.SetDestination(target.position);
    }

    public void MoveToPosition(Vector3 position)
    {
        // Debug.Log($"Moving to {position}");
        Debug.DrawRay(position, Vector3.up, Color.green, 1f);
        navMeshAgent.SetDestination(position);
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

    public void RotateTowardPosition(Vector3 position)
    {
        Vector3 directionToTarget = (position - transform.position).normalized;
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

    public IEnumerator MateRequest(AgentController other, Action<bool> onComplete)
    {
        Debug.Log("Request Reveived");

        // thinking...
        yield return new WaitForSeconds(3f);



        if (IsMale() || !canMate)
        {
            onComplete(false);
            yield break;
        }

        onComplete(true);
        yield break;
        ResetPath();

        float chance = Math.Max(other.attractiveness * 10, 20); // minimum 20% chance to reproduce
        float roll = Random.Range(0, 100);

        onComplete(roll <= chance);
    }

    public bool IsMale()
    {
        return sex == Esex.MALE;
    }

    public bool IsFemale()
    {
        return sex == Esex.FEMALE;
    }

    public void Reproduce()
    {
        if (sex == Esex.FEMALE)
        {
            Vector3 offset = new Vector3(Random.Range(1, 5), 0, Random.Range(1, 5));

            // Make sure the spawn position is on the NavMesh
            NavMeshHit hit;
            Vector3 spawnPos = transform.position + offset;

            if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
            {
                Instantiate(GetPrefab(), hit.position, transform.rotation, transform.parent);
                Debug.Log("Offspring created!");
            }
            else
            {
                Instantiate(GetPrefab(), transform.position, transform.rotation, transform.parent);
            }
        }
    }

    public IEnumerator ResetCanMate(float time = -1)
    {
        canMate = false;
        yield return new WaitForSeconds(time < 0 ? canMateResetTimer : time);
        canMate = true;
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

    public void GoToClosestWaterEdge()
    {
        Vector3 closestPosition = waterController.GetClosestPositionMarker(transform.position);
        Debug.Log($"Closest Position: {closestPosition}");
        MoveToPosition(closestPosition);
    }

    public bool HasDestination()
    {
        return navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
    }

    private float GetDecreaseRate()
    {
        float[] possibleValues = { 0.25f, 0.5f, 0.75f, 1 };
        return possibleValues[Random.Range(0, possibleValues.Length)];
    }

    private void DecrementHunger()
    {
        hunger -= hungerDecreaseRate;
    }

    private void DecrementThirst()
    {
        thirst -= thirstDecreaseRate;
    }

    /*     private void DecrementEnergy()
        {
            energy -= energyDecreaseRate;
        } */


    float time = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Water")
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                thirst = Math.Min(thirst + 10, 100);
                time = time % 1;
            }
        }
    }
}