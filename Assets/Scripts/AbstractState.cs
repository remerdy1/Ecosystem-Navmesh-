using System;

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
}