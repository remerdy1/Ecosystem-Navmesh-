using System.Collections.Generic;
using UnityEngine;

class PredatorSearchForMateState : AbstractState<PredatorStateMachine.PredatorState>
{
    PredatorController predatorController;
    Transform potentialMate;
    PredatorController potentialMateController;

    public PredatorSearchForMateState(PredatorController predatorController) : base(PredatorStateMachine.PredatorState.SearchForMate)
    {
        this.predatorController = predatorController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Search For Mate State");
        potentialMate = null;
        potentialMateController = null;
    }

    public override void UpdateState()
    {
        if (predatorController.AtTarget())
        {
            predatorController.MoveToRandomPosition();
        }

        if (potentialMate == null)
        {
            if (predatorController.fov.predatorsInViewRadius.Count > 0)
            {
                // Debug.Log("Potential Mate In FOV");
                List<Transform> predatorInViewRadius = predatorController.fov.predatorsInViewRadius;

                foreach (Transform predator in predatorInViewRadius)
                {
                    if (predator != null)
                    {
                        PredatorController controller = predator.GetComponent<PredatorController>();

                        if (!predatorController.rejectionList.Contains(predator) && controller.IsFemale() && !controller.FoundMate())
                        {
                            potentialMate = predator;
                            potentialMateController = controller;
                        }
                    }
                }

                if (potentialMate != null)
                {
                    // male makes request to female 
                    // Debug.Log($"Making Request to {potentialMateController}");

                    predatorController.StartCoroutine(potentialMateController.MateRequest(predatorController, (success) =>
                    {
                        // Debug.Log($"Request Made - Result: {success}");
                        if (success && potentialMateController != null)
                        {
                            potentialMateController.SetMate(predatorController.gameObject.transform);
                            predatorController.SetMate(potentialMate);
                            predatorController.DrawLine(predatorController.transform.position, potentialMate.transform.position, Color.green);
                        }
                        else if (potentialMateController != null)
                        {
                            predatorController.DrawLine(predatorController.transform.position, potentialMate.transform.position, Color.red);
                            predatorController.rejectionList.Add(potentialMate);
                            potentialMateController.rejectionList.Add(predatorController.transform);
                            potentialMate = null;
                            potentialMateController = null;
                        }
                    }));
                }
            }
        }
    }

    public override void ExitState()
    {
        // Debug.Log("Exiting Search For State");
        potentialMate = null;
        potentialMateController = null;
    }
}