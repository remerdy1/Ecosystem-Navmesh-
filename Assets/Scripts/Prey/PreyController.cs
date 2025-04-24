using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PreyController : AgentController
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            hunger = Math.Min(hunger + 10, 100); //todo change depending on matabolism
            fov.foodInViewRadius.Remove(other.gameObject.transform);
            simulation.DestroyFood(other.gameObject);
        }
    }

    public override void Reproduce(AgentController mateController)
    {
        if (sex == Esex.FEMALE)
        {
            Vector3 offset = new Vector3(Random.Range(1, 5), 0, Random.Range(1, 5));

            // Make sure the spawn position is on the NavMesh
            NavMeshHit hit;
            Vector3 spawnPos = transform.position + offset;

            if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
            {
                simulation.SpawnPrey(hit.position, this, mateController);
            }
            else
            {
                simulation.SpawnPrey(transform.position, this, mateController);
            }

            simulation.AddTextToDialogue("A new Prey is born!");
        }
    }

    protected override void Die()
    {
        simulation.DestroyPrey(gameObject);
        simulation.AddTextToDialogue($"A Prey has died!");
    }
}