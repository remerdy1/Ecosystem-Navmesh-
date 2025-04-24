public class PredatorStateMachine : AbstractStateMachine<PredatorStateMachine.PredatorState>
{
    public enum PredatorState
    {
        Hungry,
        Thirsty,
        SearchForMate,
        Mate,
        Wander
    }

    PredatorController predatorController;

    PredatorHungryState predatorHungryState;
    AgentWanderState<PredatorState> predatorWanderState;
    PredatorSearchForMateState predatorSearchForMateState;
    AgentMateState<PredatorState> predatorMateState;
    AgentThirstyState<PredatorState> predatorThirstyState;

    PredatorState priority = PredatorState.Hungry;

    private void Awake()
    {
        predatorController = GetComponent<PredatorController>();

        predatorSearchForMateState = new PredatorSearchForMateState(predatorController);
        predatorHungryState = new PredatorHungryState(predatorController);
        predatorWanderState = new AgentWanderState<PredatorState>(predatorController, PredatorState.Wander);
        predatorThirstyState = new AgentThirstyState<PredatorState>(predatorController, PredatorState.Thirsty);
        predatorMateState = new AgentMateState<PredatorState>(predatorController, PredatorState.Mate);

        states.Add(PredatorState.Hungry, predatorHungryState);
        states.Add(PredatorState.Wander, predatorWanderState);
        states.Add(PredatorState.SearchForMate, predatorSearchForMateState);
        states.Add(PredatorState.Mate, predatorMateState);
        states.Add(PredatorState.Thirsty, predatorThirstyState);

        currentState = states[GetNextState()];
    }

    protected override PredatorState GetNextState()
    {
        if (predatorController.FoundMate())
        {
            return PredatorState.Mate;
        }
        else if (predatorController.IsHungry() && !predatorController.IsThirsty())
        {
            priority = PredatorState.Hungry;
            return PredatorState.Hungry;
        }
        else if (predatorController.IsThirsty() && !predatorController.IsHungry())
        {
            priority = PredatorState.Thirsty;
            return PredatorState.Thirsty;
        }
        else if (predatorController.IsThirsty() && predatorController.IsHungry())
        {
            return priority;
        }
        else if (predatorController.CanMate() && predatorController.IsMale())
        {
            return PredatorState.SearchForMate;
        }

        return PredatorState.Wander;
    }

    protected override void UpdateStateText(string text)
    {
        predatorController.UpdateText(text);
    }

}