using System;
using UnityEngine;
using UnityEngine.AI;

class AgentMateState<T> : AbstractState<T> where T : Enum
{
    AgentController agentController;
    AgentController mateController;
    bool isMating;
    float matingTimer;
    float matingDuration = 3f;
    Vector3 matingPosition;

    public AgentMateState(AgentController agentController, T stateEnum) : base(stateEnum)
    {
        this.agentController = agentController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Mating State");

        Transform mate = agentController.GetMate();
        mateController = mate.GetComponent<AgentController>();

        // Find a suitable mating position
        matingPosition = FindMatingPosition();

        isMating = false;
        matingTimer = 0f;

        agentController.MoveToPosition(matingPosition);

        // Debug.Log($"Mating Position: {matingPosition}");
    }

    public override void UpdateState()
    {
        if (!isMating)
        {
            if (agentController.AtTarget() && mateController.AtTarget())
            {
                isMating = true;
                matingTimer = 0f;

                // agentController.RotateTowardTarget(mateController.transform);

                // Debug.Log($"{agentController.gameObject.name} starting mating process");
            }
        }

        if (isMating)
        {
            matingTimer += Time.deltaTime;

            agentController.RotateTowardTarget(mateController.transform);

            if (matingTimer >= matingDuration)
            {
                // Mating complete
                CompleteMating();
            }
        }
    }

    private Vector3 FindMatingPosition()
    {
        Transform mate = agentController.GetMate();
        if (mate == null)
        {
            return agentController.transform.position;
        }

        Vector3 midPoint = (agentController.transform.position + mate.position) / 2f;

        midPoint.y = agentController.transform.position.y;

        // Make sure it's on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(midPoint, out hit, 10f, NavMesh.AllAreas))
        {
            Vector3 finalPosition = hit.position;
            finalPosition.y = agentController.transform.position.y;
            return finalPosition;
        }

        // Fallback to current position
        return agentController.transform.position;
    }

    private void CompleteMating()
    {
        // Debug.Log($"{agentController.gameObject.name} completed mating");

        // Only the female gives birth
        if (agentController.sex == AgentController.Esex.FEMALE)
        {
            agentController.Reproduce(mateController);
        }

        agentController.SetMate(null);
        isMating = false;

        agentController.StartCoroutine(agentController.ResetCanMate());
    }

    public override void ExitState()
    {
        // Debug.Log("Exiting Mating State");

        // Make sure to clear references when exiting
        agentController.SetMate(null);
        mateController = null;
        isMating = false;
        matingTimer = 0f;
    }
}