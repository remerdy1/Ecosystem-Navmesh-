using UnityEngine;

class PreyThirstyState : AbstractState<PreyStateMachine.PreyState>
{
    PreyController preyController;

    public PreyThirstyState(PreyController preyController) : base(PreyStateMachine.PreyState.Thirsty)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Thirsty State");
        preyController.GoToClosestWaterEdge();
    }

    public override void UpdateState()
    {
        if (preyController.AtTarget())
        {
            if (!preyController.IsThirsty())
            {
                // Debug.Log("Drinking...");
            }
        }
    }

    public override void ExitState()
    {
        // Debug.Log("Exiting Thirsty State");
    }
}