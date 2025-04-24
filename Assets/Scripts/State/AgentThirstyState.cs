using System;

public class AgentThirstyState<T> : AbstractState<T> where T : Enum
{
    AgentController agentController;

    public AgentThirstyState(AgentController agentController, T stateEnum) : base(stateEnum)
    {
        this.agentController = agentController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Thirsty State");
        agentController.GoToClosestWaterEdge();
    }

    public override void UpdateState()
    {
        if (agentController.AtTarget())
        {
            if (!agentController.IsThirsty())
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