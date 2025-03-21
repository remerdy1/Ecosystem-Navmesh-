using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> states = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> currentState;

    protected bool IsTransitioningState = false;

    private void Start()
    {
        currentState.EnterState();
    }

    private void Update()
    {
        EState nextStateKey = GetNextState();
        Debug.Log($"Next State: {nextStateKey}");

        if (nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if (!IsTransitioningState)
        {
            Debug.Log($"Transitioning to {nextStateKey}");
            TransitionToState(nextStateKey);
        }
    }

    void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        currentState.ExitState();
        currentState = states[stateKey];
        currentState.EnterState();
        IsTransitioningState = false;
    }

    protected abstract EState GetNextState();
}