using UnityEngine;

class PreyHungryState : AbstractState<PreyStateMachine.PreyState>
{
    PreyController preyController;

    public PreyHungryState(PreyController preyController) : base(PreyStateMachine.PreyState.Hungry)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Hungry State");
    }

    public override void UpdateState()
    {
        if (preyController.fov.visibleFood.Count > 0)
        {
            Transform closestTarget = preyController.fov.ClosestTargetInView(FOV.TargetType.Food);

            if (preyController.AtTarget())
            {
                preyController.MoveToTarget(closestTarget);
            }
        }
        else if (preyController.fov.foodInViewRadius.Count > 0)
        {
            preyController.ResetPath();
            Transform closestTarget = preyController.fov.ClosestTargetInRadius(FOV.TargetType.Food);
            preyController.RotateTowardTarget(closestTarget);
        }
        else
        {
            if (preyController.AtTarget())
            {
                preyController.MoveToRandomPosition();

            }
        }
    }

    public override void ExitState()
    {
        // Debug.Log("Exiting Hungry State");
    }

}