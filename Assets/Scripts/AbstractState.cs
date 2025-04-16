using System;
using UnityEngine;

public abstract class AbstractState<EState> where EState : Enum
{
    public EState stateKey { get; private set; }

    public AbstractState(EState key)
    {
        stateKey = key;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    // public abstract EState GetNextState();
    // public abstract void OnTriggerEnter(Collider other);
}