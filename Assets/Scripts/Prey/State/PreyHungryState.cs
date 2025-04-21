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
        if (preyController.fov.foodInViewRadius.Count > 0)
        {
            Transform closestTarget = preyController.fov.ClosestTargetInRadius(FOV.TargetType.Food);

            if (preyController.AtTarget())
            {
                preyController.MoveToTarget(closestTarget);
            }
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