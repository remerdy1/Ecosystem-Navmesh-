using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PreyStateMachine : StateMachine<PreyStateMachine.PreyState>
{
    public enum PreyState
    {
        Flee,
        Hungry,
        Thirsty,
        SearchForMate,
        Mate,
        Wander
    }

    PreyController preyController;

    PreyHungryState preyHungryState;
    PreyWanderState preyWanderState;
    PreySearchForMateState preySearchForMateState;
    PreyMateState preyMateState;

    private void Awake()
    {
        preyController = GetComponent<PreyController>();

        preyWanderState = new PreyWanderState(preyController);
        preyHungryState = new PreyHungryState(preyController);
        preySearchForMateState = new PreySearchForMateState(preyController);
        preyMateState = new PreyMateState(preyController);

        states.Add(PreyState.Hungry, preyHungryState);
        states.Add(PreyState.Wander, preyWanderState);
        states.Add(PreyState.SearchForMate, preySearchForMateState);
        states.Add(PreyState.Mate, preyMateState);

        currentState = states[GetNextState()];
    }

    protected override PreyState GetNextState()
    {
        if (preyController.IsChased())
        {
            //todo cancel any mating in progress
            return PreyState.Flee;
        }
        else if (preyController.FoundMate())
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
}
