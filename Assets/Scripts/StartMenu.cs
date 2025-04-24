using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    string defaultInitialFood = "100";
    int defaultFoodPerSecond = 14;
    string defaultMaxPrey = "150";
    string defaultInitialPrey = "50";
    string defaultInitialPredator = "10";
    string defaultMaxPredator = "50";
    string defaultMaxFood = "500";

    [SerializeField] Simulation simulation;

    [SerializeField] private TMP_InputField initialPreyCount;
    [SerializeField] private TMP_InputField maxPreyCount;
    [SerializeField] private TMP_InputField initialPredatorCount;
    [SerializeField] private TMP_InputField maxPredatorCount;
    [SerializeField] private TMP_InputField initialFoodCount;
    [SerializeField] private TMP_InputField maxFoodCount;
    [SerializeField] private TMP_Text sliderText;
    [SerializeField] private Slider foodPerSecond;

    void Start()
    {
        initialPreyCount.text = defaultInitialPrey;
        maxPreyCount.text = defaultMaxPrey;
        initialFoodCount.text = defaultInitialFood;
        maxFoodCount.text = defaultMaxFood;
        initialPredatorCount.text = defaultInitialPredator;
        maxPredatorCount.text = defaultMaxPredator;
        sliderText.text = $"Food Spawn Rate Per Second: {foodPerSecond.value}";
        foodPerSecond.value = defaultFoodPerSecond;
    }

    public void UpdateSliderText()
    {
        sliderText.text = $"Food Spawn Rate Per Second: {foodPerSecond.value}";
    }

    public void StartSimulation()
    {
        if (string.IsNullOrEmpty(initialFoodCount.text)) initialFoodCount.text = defaultInitialFood;
        if (string.IsNullOrEmpty(maxPreyCount.text)) maxPreyCount.text = defaultMaxPrey;
        if (string.IsNullOrEmpty(initialPreyCount.text)) initialPreyCount.text = defaultInitialPrey;
        if (string.IsNullOrEmpty(maxPredatorCount.text)) maxPredatorCount.text = defaultMaxPredator;
        if (string.IsNullOrEmpty(initialPredatorCount.text)) initialPredatorCount.text = defaultInitialPredator;
        if (string.IsNullOrEmpty(maxFoodCount.text)) maxFoodCount.text = defaultMaxFood;

        simulation.SetStats(
            int.Parse(initialPreyCount.text), int.Parse(maxPreyCount.text),
            int.Parse(initialFoodCount.text), int.Parse(maxFoodCount.text),
            int.Parse(initialPredatorCount.text), int.Parse(maxPredatorCount.text),
            (int)foodPerSecond.value
        );
        simulation.InitializeSimulation();
    }
}
