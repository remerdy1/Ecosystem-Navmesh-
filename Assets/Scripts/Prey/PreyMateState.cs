using UnityEngine;

class PreyMateState : BaseState<PreyStateMachine.PreyState>
{
    PreyController preyController;

    public PreyMateState(PreyController preyController) : base(PreyStateMachine.PreyState.Mate)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        Debug.Log("Entering mating state");
    }

    public override void UpdateState()
    {
        if (preyController.fov.predatorsInViewRadius.Count == 0)
        {
            preyController.MoveToRandomPosition();
        }
        else
        {
            Transform potentialMate = preyController.FindPotentialMate();
            // Approach
            preyController.MoveToTarget(potentialMate);
            // Send mate request
            PreyController otherController = potentialMate.GetComponent<PreyController>();
            bool success = otherController.MateRequest(this.preyController);
            if (success)
            {
                //todo if successful - reproduce
            }
            else
            {
                this.preyController.rejectionList.Add(potentialMate);
            }
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting mating state");
    }

}