using UnityEngine;
using CustomAttributes;
using System.Collections.Generic;
using UnityEngine.AI;

public class Simulation : MonoBehaviour
{
    [SerializeField] CameraController cameraController;

    // Food
    [SerializeField, ReadOnly] protected List<GameObject> spawnedFood;
    protected int initialFoodCount;
    protected int maxFoodCount;
    [SerializeField] protected GameObject foodObject;
    protected int foodPerSecond;

    // Prey
    [SerializeField, ReadOnly] protected List<GameObject> spawnedPrey;
    protected int initalPreyCount;
    protected int maxPreyCount;
    [SerializeField] protected GameObject preyObject;

    // Predator
    [SerializeField, ReadOnly] protected List<GameObject> spawnedPredator;
    protected int initialPredatorCount;
    protected int maxPredatorCount;
    [SerializeField] protected GameObject predatorObject;

    // UI
    [SerializeField] Canvas startMenu;
    [SerializeField] Canvas overlay;

    private void Start()
    {
        spawnedFood = new List<GameObject>();
        spawnedPrey = new List<GameObject>();

        startMenu.enabled = true;
        overlay.enabled = false;
        cameraController.LockCamera();
    }

    public void SetStats(int initalPreyCount, int maxPreyCount, int initialFoodCount, int maxFoodCount, int initialPredatorCount, int maxPredatorCount, int foodPerSecond)
    {
        this.initalPreyCount = initalPreyCount;
        this.maxPreyCount = maxPreyCount;
        this.initialFoodCount = initialFoodCount;
        this.maxFoodCount = maxFoodCount;
        this.initialPredatorCount = initialPredatorCount;
        this.maxPredatorCount = maxPredatorCount;
        this.foodPerSecond = foodPerSecond;
    }

    public void DestroyFood(GameObject food)
    {
        spawnedFood.Remove(food);
        Destroy(food);
    }

    public int GetCurrentFoodCount()
    {
        return spawnedFood.Count;
    }

    public int GetCurrentPreyCount()
    {
        return spawnedPrey.Count;
    }

    public int GetCurrentPredatorCount()
    {
        return spawnedPredator.Count;
    }

    public void SpawnPrey(Vector3 localPosition, AgentController parentOne = null, AgentController parentTwo = null)
    {
        if (spawnedPrey.Count < maxPreyCount)
        {
            GameObject prey = Instantiate(preyObject, transform);
            prey.transform.localPosition = localPosition;
            spawnedPrey.Add(prey);

            if (parentOne != null && parentTwo != null)
            {
                prey.GetComponent<AgentController>().Init(parentOne, parentTwo);
            }
        }
    }

    public void SpawnPredator(Vector3 localPosition, AgentController parentOne = null, AgentController parentTwo = null)
    {
        if (spawnedPredator.Count < maxPredatorCount)
        {
            GameObject predator = Instantiate(predatorObject, transform);
            predator.transform.localPosition = localPosition;
            spawnedPredator.Add(predator);

            if (parentOne != null && parentTwo != null)
            {
                predator.GetComponent<AgentController>().Init(parentOne, parentTwo);
            }
        }
    }

    public int GetMaxPreyCount()
    {
        return maxPreyCount;
    }

    public int GetMaxPredatorCount()
    {
        return maxPredatorCount;
    }

    public void DestroyPrey(GameObject prey)
    {
        spawnedPrey.Remove(prey);
        Destroy(prey);
    }

    public void DestroyPredator(GameObject predator)
    {
        spawnedPrey.Remove(predator);
        Destroy(predator);
    }

    public void InitializeSimulation()
    {
        InvokeRepeating("SpawnFood", 1, 1);

        // Spawn initial food
        for (int i = 0; i < initialFoodCount && (GetCurrentFoodCount() < maxFoodCount || maxFoodCount < 0); i++)
        {
            GameObject newFood = Instantiate(foodObject, gameObject.transform);
            newFood.transform.localPosition = GetRandomPosition(0.5f);
            spawnedFood.Add(newFood);
        }

        // Spawn initial prey
        for (int i = 0; i < initalPreyCount && (GetCurrentPreyCount() < maxPreyCount || maxPreyCount < 0); i++)
        {
            SpawnPrey(GetRandomPosition());
        }

        // Spawn initial predator
        for (int i = 0; i < initialPredatorCount && (GetCurrentPredatorCount() < maxPredatorCount || maxPredatorCount < 0); i++)
        {
            SpawnPredator(GetRandomPosition());
        }

        startMenu.enabled = false;
        overlay.enabled = true;
        cameraController.UnlockCamera();
    }

    public void Quit()
    {
        Application.Quit();
    }

    protected void SpawnFood()
    {
        for (int i = 0; i < foodPerSecond; i++)
        {
            if (maxFoodCount == -1 || GetCurrentFoodCount() < maxFoodCount)
            {
                GameObject newFood = Instantiate(foodObject, gameObject.transform);
                newFood.transform.localPosition = GetRandomPosition(0.5f);
                spawnedFood.Add(newFood);
            }
        }
    }


    public Vector3 GetRandomPosition(float y = 1f)
    {
        // Attempt 30 times max
        for (int i = 0; i < 30; i++)
        {
            float x = Random.Range(0, 300);
            float z = Random.Range(0, 300);

            // We don't want positions inside of the "Water Area"
            if (x >= 100 && x <= 150) x = Random.Range(0, 100);
            else if (x > 150 && x <= 200) x = Random.Range(201, 300);

            Vector3 randomPos = new Vector3(x, y, z);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, 150, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return new Vector3(150, y, 50);
    }
}