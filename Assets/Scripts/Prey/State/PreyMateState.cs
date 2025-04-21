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
        // Debug.Log("Entering Mating State");

        Transform mate = preyController.GetMate();
        mateController = mate.GetComponent<PreyController>();

        // Find a suitable mating position
        matingPosition = FindMatingPosition();

        isMating = false;
        matingTimer = 0f;

        preyController.MoveToPosition(matingPosition);

        // Debug.Log($"Mating Position: {matingPosition}");
    }

    public override void UpdateState()
    {
        if (!isMating)
        {
            if (preyController.AtTarget() && mateController.AtTarget())
            {
                isMating = true;
                matingTimer = 0f;

                preyController.RotateTowardTarget(mateController.transform);

                Debug.Log($"{preyController.gameObject.name} starting mating process");
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

        Vector3 midPoint = (preyController.transform.position + mate.position) / 2f;

        midPoint.y = preyController.transform.position.y;

        // Make sure it's on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(midPoint, out hit, 10f, NavMesh.AllAreas))
        {
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