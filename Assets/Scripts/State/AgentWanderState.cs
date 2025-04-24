using System;

public class AgentWanderState<T> : AbstractState<T> where T : Enum
{
    protected AgentController agentController;

    public AgentWanderState(AgentController agentController, T stateEnum) : base(stateEnum)
    {
        this.agentController = agentController;
    }

    public override void EnterState()
    {
        agentController.ResetPath();
        agentController.MoveToRandomPosition();
    }

    public override void UpdateState()
    {
        if (agentController.AtTarget())
        {
            agentController.MoveToRandomPosition();
        }
    }

    public override void ExitState()
    {
        // Clean up if needed
    }
}