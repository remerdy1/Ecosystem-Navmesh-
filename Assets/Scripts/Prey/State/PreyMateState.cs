using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;

class PreyMateState : AbstractState<PreyStateMachine.PreyState>
{
    PreyController preyController;
    PreyController mateController;
    bool isMating;
    float matingTimer;
    float matingDuration = 3f;
    Vector3 matingPosition;

    public PreyMateState(PreyController preyController) : base(PreyStateMachine.PreyState.Mate)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Mating State");

        Transform mate = preyController.GetMate();
        mateController = mate.GetComponent<PreyController>();

        // Find a suitable mating position
        matingPosition = FindMatingPosition();

        isMating = false;
        matingTimer = 0f;

        if (preyController.sex == AgentController.Esex.FEMALE)
        {
            // Move to offest position
            Vector3 femalePosition = matingPosition + new Vector3(3f, 0, 0);
            preyController.MoveToPosition(femalePosition);
        }
        else
        {
            preyController.MoveToPosition(matingPosition);
        }

        Debug.Log($"Mating Position: {matingPosition}");
    }

    public override void UpdateState()
    {
        if (!isMating)
        {
            Vector2 agentPos2D = new Vector2(preyController.transform.position.x, preyController.transform.position.z);
            Vector2 targetPos2D;

            if (preyController.sex == AgentController.Esex.FEMALE)
            {
                Vector3 femalePosition = matingPosition + new Vector3(3f, 0, 0);
                targetPos2D = new Vector2(femalePosition.x, femalePosition.z);
            }
            else
            {
                targetPos2D = new Vector2(matingPosition.x, matingPosition.z);
            }

            float distToTarget = Vector2.Distance(agentPos2D, targetPos2D);

            if (distToTarget < 0.5f)
            {
                Vector2 matePos2D = new Vector2(mateController.transform.position.x, mateController.transform.position.z);
                float distanceToMate = Vector2.Distance(agentPos2D, matePos2D);

                Debug.Log($"Distance To Mate: {distanceToMate}");

                if (distanceToMate <= 3.5f)
                {
                    isMating = true;
                    matingTimer = 0f;

                    preyController.RotateTowardTarget(mateController.transform);

                    Debug.Log($"{preyController.gameObject.name} starting mating process");
                }
            }
        }

        if (isMating)
        {
            matingTimer += Time.deltaTime;

            preyController.RotateTowardTarget(mateController.transform);

            if (matingTimer >= matingDuration)
            {
                // Mating complete
                CompleteMating();
            }
        }
    }

    private Vector3 FindMatingPosition()
    {
        Transform mate = preyController.GetMate();
        if (mate == null)
        {
            return preyController.transform.position;
        }

        // Find a position between the two agents
        Vector3 midPoint = (preyController.transform.position + mate.position) / 2f;

        midPoint.y = preyController.transform.position.y;

        // Make sure it's on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(midPoint, out hit, 10f, NavMesh.AllAreas))
        {
            // Create a new Vector3 with the desired Y value
            Vector3 finalPosition = hit.position;
            finalPosition.y = preyController.transform.position.y;
            return finalPosition;
        }

        // Fallback to current position
        return preyController.transform.position;
    }

    private void CompleteMating()
    {
        Debug.Log($"{preyController.gameObject.name} completed mating");

        // Only the female gives birth
        if (preyController.sex == AgentController.Esex.FEMALE)
        {
            preyController.Reproduce(mateController);
        }

        preyController.SetMate(null);
        isMating = false;

        preyController.StartCoroutine(preyController.ResetCanMate());
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Mating State");

        // Make sure to clear references when exiting
        preyController.SetMate(null);
        mateController = null;
        isMating = false;
        matingTimer = 0f;

        // Ensure the agent has a destination when exiting
        if (!preyController.HasDestination())
        {
            preyController.MoveToRandomPosition();
        }
    }
}