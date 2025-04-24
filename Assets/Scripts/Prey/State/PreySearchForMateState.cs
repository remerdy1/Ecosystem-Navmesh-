using System.Collections.Generic;
using UnityEngine;

class PreySearchForMateState : AbstractState<PreyStateMachine.PreyState>
{
    PreyController preyController;
    Transform potentialMate;
    PreyController potentialMateController;

    public PreySearchForMateState(PreyController preyController) : base(PreyStateMachine.PreyState.SearchForMate)
    {
        this.preyController = preyController;
    }

    public override void EnterState()
    {
        // Debug.Log("Entering Search For Mate State");
        potentialMate = null;
        potentialMateController = null;
    }

    public override void UpdateState()
    {
        if (potentialMate == null)
        {
            if (preyController.fov.preyInViewRadius.Count > 0)
            {
                // Debug.Log("Potential Mate In FOV");

                // Find closest prey not in rejection list
                List<Transform> preyInViewRadius = preyController.fov.preyInViewRadius;

                foreach (Transform prey in preyInViewRadius)
                {
                    if (prey != null)
                    {
                        PreyController controller = prey.GetComponent<PreyController>();
                        if (!preyController.rejectionList.Contains(prey) && controller.IsFemale() && !controller.FoundMate())
                        {
                            potentialMate = prey;
                            potentialMateController = controller;
                        }
                    }
                }

                if (potentialMate != null)
                {
                    // male makes request to female 
                    // Debug.Log($"Making Request to {potentialMateController}");

                    preyController.StartCoroutine(potentialMateController.MateRequest(preyController, (success) =>
                    {
                        // Debug.Log($"Request Made - Result: {success}");
                        if (success && potentialMateController != null)
                        {
                            potentialMateController.SetMate(preyController.gameObject.transform);
                            preyController.SetMate(potentialMate);
                            preyController.DrawLine(preyController.transform.position, potentialMate.transform.position, Color.green);
                        }
                        else if (potentialMate != null)
                        {
                            preyController.DrawLine(preyController.transform.position, potentialMate.transform.position, Color.red);
                            preyController.rejectionList.Add(potentialMate);
                            potentialMateController.rejectionList.Add(preyController.transform);
                            potentialMate = null;
                            potentialMateController = null;
                        }
                    }));
                }
            }
        }
        else if (preyController.AtTarget())
        {
            preyController.MoveToRandomPosition();
        }
    }


    public override void ExitState()
    {
        // Debug.Log("Exiting Search For State");
        potentialMate = null;
        potentialMateController = null;
    }
}