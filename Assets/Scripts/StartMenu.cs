using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    string defaultInitialFood = "100";
    int defaultFoodPerSecond = 7;
    string defaultMaxPrey = "50";
    string defaultInitialPrey = "10";
    string defaultMaxFood = "500";

    [SerializeField] Simulation simulation;

    [SerializeField] private TMP_InputField initialPreyCount;
    [SerializeField] private TMP_InputField maxPreyCount;

    [SerializeField] private TMP_InputField initialFoodCount;
    [SerializeField] private TMP_InputField maxFoodCount;
    [SerializeField] private Slider foodPerSecond;

    void Start()
    {
        initialPreyCount.text = defaultInitialPrey;
        maxPreyCount.text = defaultMaxPrey;
        initialFoodCount.text = defaultInitialFood;
        maxFoodCount.text = defaultMaxFood;
        foodPerSecond.value = defaultFoodPerSecond;
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
