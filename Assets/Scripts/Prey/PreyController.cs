using UnityEngine;

public class PreyController : AgentController
{
    private bool chased;
    [SerializeField] private GameObject preyPrefab;

    void Start()
    {
        chased = false;
    }

    public bool IsChased()
    {
        return chased;
    }

    public override GameObject GetPrefab()
    {
        return preyPrefab; 
    }
}