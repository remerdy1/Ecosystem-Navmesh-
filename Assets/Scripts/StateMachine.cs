using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, AbstractState<EState>> states = new Dictionary<EState, AbstractState<EState>>();
    [field: SerializeField] protected AbstractState<EState> currentState;

    protected bool IsTransitioningState = false;

    private void Start()
    {
        currentState.EnterState();
        UpdateStateText(currentState.stateKey.ToString());
    }

    private void Update()
    {
        EState nextStateKey = GetNextState();
        // Debug.Log($"Next State Key: {nextStateKey} Current State Key: {currentState.stateKey} Equal: {nextStateKey.Equals(currentState.stateKey)}");

        if (nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if (!IsTransitioningState)
        {
            // Debug.Log($"Transitioning to {nextStateKey}");
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
        UpdateStateText(stateKey.ToString());
    }

    protected abstract EState GetNextState();
    protected abstract void UpdateStateText(string text);
}