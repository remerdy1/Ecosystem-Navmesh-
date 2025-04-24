using UnityEngine;

class PredatorHungryState : AbstractState<PredatorStateMachine.PredatorState>
{
    PredatorController predatorController;

    public PredatorHungryState(PredatorController predatorController) : base(PredatorStateMachine.PredatorState.Hungry)
    {
        this.predatorController = predatorController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Hungry State");
    }

    public override void UpdateState()
    {
        if (predatorController.fov.preyInViewRadius.Count > 0)
        {
            Transform closestTarget = predatorController.fov.ClosestTargetInRadius(FOV.TargetType.Prey);
            predatorController.MoveToTarget(closestTarget);

        }
        else
        {
            if (predatorController.AtTarget())
            {
                predatorController.MoveToRandomPosition();

            }
        }
    }

    public override void ExitState()
    {
        // Debug.Log("Exiting Hungry State");
    }

}