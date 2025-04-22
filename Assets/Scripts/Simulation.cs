using UnityEngine;
using CustomAttributes;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    [SerializeField] CameraController cameraController;

    // Food
    [SerializeField, ReadOnly] protected List<GameObject> spawnedFood;
    protected int initialFoodCount;
    protected int maxFoodCount = -1;
    [SerializeField] protected GameObject foodObject;
    protected int foodPerSecond;

    // Prey
    [SerializeField, ReadOnly] protected List<GameObject> spawnedPrey;
    protected int initalPreyCount;
    protected int maxPreyCount = -1;
    [SerializeField] protected GameObject preyObject;

    // Plane
    [SerializeField] Collider preySpawnArea;

    // UI
    [SerializeField] Canvas startMenu;
    [SerializeField] Canvas overlay;

    private void Start()
    {
        startMenu.enabled = true;
        overlay.enabled = false;
        cameraController.LockCamera();
    }

    public void SetStats(int initalPreyCount, int maxPreyCount, int initialFoodCount, int maxFoodCount, int foodPerSecond)
    {
        this.initalPreyCount = initalPreyCount;
        this.maxPreyCount = maxPreyCount;
        this.initalPreyCount = initialFoodCount;
        this.maxFoodCount = maxFoodCount;
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

    public void SpawnPrey(Vector3 localPosition)
    {
        GameObject prey = Instantiate(preyObject, gameObject.transform);
        spawnedPrey.Add(prey);
        prey.transform.localPosition = localPosition;
    }

    public int GetMaxPreyCount()
    {
        return this.maxPreyCount;
    }

    public void DestroyPrey(GameObject prey)
    {
        spawnedPrey.Remove(prey);
        Destroy(prey);
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
        for (int i = 0; i < initalPreyCount && (GetCurrentPreyCount() < maxPreyCount || maxPreyCount < 1); i++)
        {
            SpawnPrey(GetRandomPosition());
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

    /// <summary>
    /// Returns a valid random position within the terrain
    /// </summary>
    /// <param name="y">
    /// The y value of the positon (defaults to 1)
    /// </param>
    public Vector3 GetRandomPosition(float y = 1f)
    {
        Bounds colliderBounds = preySpawnArea.bounds;
        return new Vector3(
            Random.Range(colliderBounds.min.x, colliderBounds.max.x),
            y,
            Random.Range(colliderBounds.min.z, colliderBounds.max.z)
        );
    }
}