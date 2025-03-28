using System.IO.Pipes;
using UnityEngine;

class PreyMateState : BaseState<PreyStateMachine.PreyState>
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
            Transform mate = preyController.GetMate();
            mateController = mate.GetComponent<PreyController>();
        }
    }

    public override void UpdateState()
    {
        // Check if we've reached our position
        if (!isMating && preyController.AtTarget())
        {
            Debug.Log("AT TARGET");

            // Wait for mate to also reach position
            float distanceToMate = Vector3.Distance(preyController.transform.position, mateController.transform.position);

            if (distanceToMate < 3f)
            {
                // Both agents are in position, start mating
                isMating = true;
                matingTimer = 0f;

                // Face each other
                preyController.RotateTowardPosition(matingPosition);

                Debug.Log($"{preyController.gameObject.name} starting mating process");
            }
        }

        if (isMating)
        {
            // During mating
            matingTimer += Time.deltaTime;

            // Keep facing each other
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

        // Make sure it's on the NavMesh
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(midPoint, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
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
            preyController.Mate();
        }

        // Clear the mate reference
        preyController.SetMate(null);

        // Reset state
        isMating = false;
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Mating State");

        // Make sure to clear references when exiting
        preyController.SetMate(null);
        mateController = null;
    }
}