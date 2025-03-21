using UnityEngine;

public class PreyController : AgentController
{
    private bool chased;

    void Start()
    {
        chased = false;
    }

    public bool IsChased()
    {
        return chased;
    }
}