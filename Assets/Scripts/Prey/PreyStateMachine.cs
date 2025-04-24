public class PreyStateMachine : AbstractStateMachine<PreyStateMachine.PreyState>
{
    public enum PreyState
    {
        Hungry,
        Thirsty,
        SearchForMate,
        Mate,
        Wander
    }

    PreyController preyController;

    PreyHungryState preyHungryState;
    AgentWanderState<PreyState> preyWanderState;
    PreySearchForMateState preySearchForMateState;
    AgentMateState<PreyState> preyMateState;
    AgentThirstyState<PreyState> preyThirstyState;

    private void Awake()
    {
        preyController = GetComponent<PreyController>();

        preySearchForMateState = new PreySearchForMateState(preyController);
        preyHungryState = new PreyHungryState(preyController);
        preyWanderState = new AgentWanderState<PreyState>(preyController, PreyState.Wander);
        preyThirstyState = new AgentThirstyState<PreyState>(preyController, PreyState.Thirsty);
        preyMateState = new AgentMateState<PreyState>(preyController, PreyState.Mate);

        states.Add(PreyState.Hungry, preyHungryState);
        states.Add(PreyState.Wander, preyWanderState);
        states.Add(PreyState.SearchForMate, preySearchForMateState);
        states.Add(PreyState.Mate, preyMateState);
        states.Add(PreyState.Thirsty, preyThirstyState);

        currentState = states[GetNextState()];
    }

    protected override PreyState GetNextState()
    {
        if (preyController.FoundMate())
        {
            return PreyState.Mate;
        }
        else if (preyController.IsHungry())
        {
            return PreyState.Hungry;
        }
        else if (preyController.IsThirsty())
        {
            return PreyState.Thirsty;
        }
        else if (preyController.CanMate() && preyController.IsMale())
        {
            return PreyState.SearchForMate;
        }

        return PreyState.Wander;
    }

    protected override void UpdateStateText(string text)
    {
        preyController.UpdateText(text);
    }
}
