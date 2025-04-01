using UnityEngine;


class PreyWanderState : BaseState<PreyStateMachine.PreyState>
{
    PreyController preyController;

    public PreyWanderState(PreyController preyController) : base(PreyStateMachine.PreyState.Wander)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Wander State");
        preyController.ResetPath();
        preyController.MoveToRandomPosition();
    }

    public override void UpdateState()
    {
        Debug.Log($"{preyController.gameObject} is wandering");
        if (preyController.AtTarget())
        {
            preyController.MoveToRandomPosition();
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Wandering State");
    }

}