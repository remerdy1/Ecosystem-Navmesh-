using UnityEngine;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

class PredatorController : AgentController
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Prey" && hunger < hungerThreshold)
        {
            hunger = Math.Min(hunger + 30, 100);
            fov.preyInViewRadius.Remove(other.transform);
            simulation.DestroyPrey(other.gameObject);
            simulation.AddTextToDialogue("A Prey has been eaten!");
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
                simulation.SpawnPredator(hit.position, this, mateController);
            }
            else
            {
                simulation.SpawnPredator(transform.position, this, mateController);
            }
            simulation.AddTextToDialogue("A new Predator is born!");
        }
    }

    protected override void Die()
    {
        simulation.DestroyPredator(gameObject);
        simulation.AddTextToDialogue($"A Predator has died!");
    }
}