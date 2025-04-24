using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public abstract class AgentController : MonoBehaviour
{
    [SerializeField] protected Simulation simulation;

    protected NavMeshAgent navMeshAgent;
    public FOV fov { get; protected set; }
    public List<Transform> rejectionList { get; private set; } = new List<Transform>(); //todo forget after x minutes
    [field: SerializeField] public WaterController waterController;
    [SerializeField] Material maleMaterial;
    [SerializeField] Material femaleMaterial;

    public enum Esex
    {
        MALE = 0,
        FEMALE = 1
    }

    // Stats
    [field: SerializeField] protected float hunger;
    [field: SerializeField] protected float thirst;

    [field: SerializeField] protected float hungerDecreaseRate;
    [field: SerializeField] protected float thirstDecreaseRate;

    [field: SerializeField] protected float hungerThreshold;
    [field: SerializeField] protected float thirstThreshold;

    [field: SerializeField] protected float rotationSpeed = 1f;

    // Reproduction
    [field: SerializeField] public Esex sex { get; private set; }
    [field: SerializeField] public float attractiveness { get; private set; }
    [field: SerializeField] protected bool canMate;
    [field: SerializeField] protected float canMateResetTimer;
    [field: SerializeField] protected Transform mate;

    // UI
    [field: SerializeField] protected AgentUIController agentUI;

    public abstract void Reproduce(AgentController mateController);
    protected abstract void Die();

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FOV>();
        simulation = transform.parent.GetComponent<Simulation>();

        hungerThreshold = Random.Range(30f, 50f);
        thirstThreshold = Random.Range(30f, 50f);

        hunger = Random.Range(hungerThreshold, 100);
        thirst = Random.Range(hungerThreshold, 100);

        sex = (Esex)Random.Range(0, 2);
        GetComponent<MeshRenderer>().material = (sex == Esex.FEMALE) ? femaleMaterial : maleMaterial;

        attractiveness = Random.Range(0f, 10f);
        canMateResetTimer = Random.Range(30f, 120f);
        StartCoroutine(ResetCanMate(30)); // Can't mate for the first 30 seconds of spawning
        mate = null;

        hungerDecreaseRate = Random.Range(1, 21) * 0.05f;
        thirstDecreaseRate = Random.Range(1, 21) * 0.05f;

        InvokeRepeating("DecrementHunger", 1, 1);
        InvokeRepeating("DecrementThirst", 1, 1);

        navMeshAgent.speed = Random.Range(2, 5f);
        fov.radius = Random.Range(13, 30f);
    }

    public void Init(AgentController parentOne, AgentController parentTwo)
    {
        // hungerDecreaseRate
        hungerDecreaseRate = Random.value > 0.5 ? parentOne.hungerDecreaseRate : parentTwo.hungerDecreaseRate;
        hungerDecreaseRate = MutateStat(hungerDecreaseRate, 0.25f, 1);
        // thirstDecreaseRate
        thirstDecreaseRate = Random.Range(0, 100) > 50 ? parentOne.thirstDecreaseRate : parentTwo.thirstDecreaseRate;
        thirstDecreaseRate = MutateStat(thirstDecreaseRate, 0.25f, 1);
        // attractiveness
        attractiveness = Random.Range(0, 100) > 50 ? parentOne.attractiveness : parentTwo.attractiveness;
        attractiveness = MutateStat(attractiveness, 0, 10);
        // canMateResetTimer
        canMateResetTimer = Random.Range(0, 100) > 50 ? parentOne.canMateResetTimer : parentTwo.canMateResetTimer;
        canMateResetTimer = MutateStat(canMateResetTimer, 30, 120);
        // hungerThreshold
        hungerThreshold = Random.Range(0, 100) > 50 ? parentOne.hungerThreshold : parentTwo.hungerThreshold;
        hungerThreshold = MutateStat(hungerThreshold, 30f, 50f);
        // thirstThreshold
        thirstThreshold = Random.Range(0, 100) > 50 ? parentOne.thirstThreshold : parentTwo.thirstThreshold;
        thirstThreshold = MutateStat(thirstThreshold, 30f, 50f);
        // Speed
        navMeshAgent.speed = Random.Range(0, 100) > 50 ? parentOne.navMeshAgent.speed : parentTwo.navMeshAgent.speed;
        navMeshAgent.speed = MutateStat(navMeshAgent.speed, 2f, 12f);
        //Field Of View
        fov.radius = Random.Range(0, 100) > 50 ? parentOne.fov.radius : parentTwo.fov.radius;
        fov.radius = MutateStat(fov.radius, 13, 100);
    }
    void Update()
    {
        agentUI.UpdateHungerSlider(hunger);
        agentUI.UpdateThirstSlider(thirst);

        if (hunger <= 0 || thirst <= 0)
        {
            Die();
        }
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

    public void UpdateText(string text)
    {
        agentUI.UpdateText(text);
    }

    public void MoveToRandomPosition()
    {
        Vector3 newPosition = GetRandomPosition(transform.position, fov.radius);
        MoveToPosition(newPosition);
    }

    public void MoveToTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            MoveToPosition(newPosition);
        }
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(newPosition, out hit, 5.0f, NavMesh.AllAreas))
        {
            // Debug.Log($"Moving to {hit.position}");
            Debug.DrawRay(hit.position, Vector3.up, Color.blue, 1.0f);
            navMeshAgent.SetDestination(hit.position);
        }
    }

    public void ResetPath()
    {
        if (navMeshAgent != null) navMeshAgent.ResetPath();
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
        return !navMeshAgent.pathPending &&
               navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
               (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }

    public float DistanceToTarget(Transform target)
    {
        return Vector3.Distance(target.position, transform.position);
    }

    public IEnumerator MateRequest(AgentController other, Action<bool> onComplete)
    {
        // Debug.Log("Request Reveived");
        // thinking...
        yield return new WaitForSeconds(3f);

        if (IsMale() || !canMate)
        {
            onComplete(false);
            yield break;
        }
        // testing
        /*         onComplete(true);
                yield break; */
        ResetPath();

        float chance = Mathf.Max(other.attractiveness * 10, 20); // minimum 20% chance to reproduce
        float roll = Random.Range(0, 100);

        onComplete(roll <= chance);
    }

    public void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        GameObject line = new GameObject("MateRequestLine");
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;

        GameObject.Destroy(line, 1f);
    }

    public bool IsMale()
    {
        return sex == Esex.MALE;
    }

    public bool IsFemale()
    {
        return sex == Esex.FEMALE;
    }


    private float MutateStat(float stat, float min, float max, float mutationStrength = 1.2f, float totalMutationChance = 0.3f)
    {
        // Debug.Log("Check for mutation!");
        float roll = Random.value;

        if (roll < totalMutationChance / 2)
        {
            // Debug.Log("Mutate!");
            return Mathf.Clamp(stat * Random.Range(1f, mutationStrength), min, max);
        }
        else if (roll < totalMutationChance)
        {
            // Debug.Log("Mutate!");
            return Mathf.Clamp(stat / Random.Range(1f, mutationStrength), min, max);
        }

        return Mathf.Clamp(stat, min, max);
    }

    public IEnumerator ResetCanMate(float time = -1)
    {
        canMate = false;
        yield return new WaitForSeconds(time < 0 ? canMateResetTimer : time);
        canMate = true;
    }

    private Vector3 GetRandomPosition(Vector3 currentPosition, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPosition = currentPosition + Random.insideUnitSphere * radius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPosition, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;

            }
        }

        // Debug.Log("Valid Path Not Found: Returning Zeroes");
        return Vector3.zero;
    }

    public void GoToClosestWaterEdge()
    {
        Vector3 closestPosition = waterController.GetClosestPositionMarker(transform.position);
        // Debug.Log($"Closest Position: {closestPosition}");
        MoveToPosition(closestPosition);
        waterController.OccupyPosition(closestPosition, this);
    }

    public bool HasDestination()
    {
        return navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
    }

    private void DecrementHunger()
    {
        hunger -= hungerDecreaseRate;
    }

    private void DecrementThirst()
    {
        thirst -= thirstDecreaseRate;
    }

    float time = 0;
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Water")
        {
            Debug.Log("Entered Water");

            time += Time.deltaTime;
            if (time >= 1)
            {
                thirst = Mathf.Min(thirst + 4, 100);
                time = time % 1;
            }
        }
    }
}