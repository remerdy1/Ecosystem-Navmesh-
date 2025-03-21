using UnityEngine;

public class PreyStateMachine : StateMachine<PreyStateMachine.PreyState>
{
    public enum PreyState
    {
        Flee,
        Hungry,
        Thirsty,
        Mate,
        Wander
    }

    PreyController preyController;

    PreyHungryState preyHungryState;
    PreyWanderState preyWanderState;

    private void Awake()
    {
        preyController = GetComponent<PreyController>();

        preyWanderState = new PreyWanderState(preyController);
        preyHungryState = new PreyHungryState(preyController);

        states.Add(PreyState.Hungry, preyHungryState);
        states.Add(PreyState.Wander, preyWanderState);

        //todo base initial state from randomise stats, 
        currentState = states[PreyState.Wander];
    }

    protected override PreyState GetNextState()
    {
        if (preyController.IsChased())
        {
            return PreyState.Flee;
        }
        if (preyController.IsHungry())
        {
            return PreyState.Hungry;
        }
        if (preyController.IsThirsty())
        {
            return PreyState.Thirsty;
        }
        if (preyController.CanMate())
        {
            return PreyState.Mate;
        }

        return PreyState.Wander;
    }
}
