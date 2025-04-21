using System;
using UnityEngine;

public class PreyController : AgentController
{
    private bool chased;
    [SerializeField] private GameObject preyPrefab;

    void Start()
    {
        chased = false;
    }

    public bool IsChased()
    {
        return chased;
    }

    public override GameObject GetPrefab()
    {
        return preyPrefab;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            hunger = Math.Min(hunger + 10, 100); //todo change depending on matabolism
            fov.foodInViewRadius.Remove(other.gameObject.transform);
            Destroy(other.gameObject);
        }
    }
}