using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    [SerializeField] string defaultInitialFood = "7";
    [SerializeField] string defaultMaxPrey = "50";
    [SerializeField] string defaultInitialPrey = "10";
    [SerializeField] string defaultMaxFood = "100";

    [SerializeField] Simulation simulation;

    [SerializeField] private TMP_InputField initialPreyCount;
    [SerializeField] private TMP_InputField maxPreyCount;

    [SerializeField] private TMP_InputField initialFoodCount;
    [SerializeField] private TMP_InputField maxFoodCount;
    [SerializeField] private Slider foodPerSecond;

    void Start()
    {
        initialPreyCount.text = "10";
        maxPreyCount.text = "50";
        initialFoodCount.text = "7";
        maxFoodCount.text = "100";
        foodPerSecond.value = 10;
    }

    public void StartSimulation()
    {
        if (string.IsNullOrEmpty(initialFoodCount.text)) initialFoodCount.text = defaultInitialFood;
        if (string.IsNullOrEmpty(maxPreyCount.text)) maxPreyCount.text = defaultMaxPrey;
        if (string.IsNullOrEmpty(initialPreyCount.text)) initialPreyCount.text = defaultInitialPrey;
        if (string.IsNullOrEmpty(maxFoodCount.text)) maxFoodCount.text = defaultMaxFood;

        simulation.SetStats(int.Parse(initialPreyCount.text), int.Parse(maxPreyCount.text), int.Parse(initialFoodCount.text), int.Parse(maxFoodCount.text), (int)foodPerSecond.value);
        simulation.InitializeSimulation();
    }
}
