using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PreyStateMachine : AbstractStateMachine<PreyStateMachine.PreyState>
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
    PreyThirstyState preyThirstyState;

    private void Awake()
    {
        preyController = GetComponent<PreyController>();

        preyWanderState = new PreyWanderState(preyController);
        preyHungryState = new PreyHungryState(preyController);
        preySearchForMateState = new PreySearchForMateState(preyController);
        preyMateState = new PreyMateState(preyController);
        preyThirstyState = new PreyThirstyState(preyController);

        states.Add(PreyState.Hungry, preyHungryState);
        states.Add(PreyState.Wander, preyWanderState);
        states.Add(PreyState.SearchForMate, preySearchForMateState);
        states.Add(PreyState.Mate, preyMateState);
        states.Add(PreyState.Thirsty, preyThirstyState);

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

    protected override void UpdateStateText(string text)
    {
        preyController.UpdateText(text);
    }
}
