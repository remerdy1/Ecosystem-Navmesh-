using CustomAttributes;
using TMPro;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    [SerializeField] private TMP_Text preyCount;
    [SerializeField] private TMP_Text predatorCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text timeElapsed;

    private float elapsedTime;

    [SerializeField] private Simulation simulation;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100);


        preyCount.text = $"Prey Count: {simulation.GetCurrentPreyCount()}";
        predatorCount.text = $"Predator Count: {simulation.GetCurrentPredatorCount()}";
        foodCount.text = $"Food Count: {simulation.GetCurrentFoodCount()}";
        timeElapsed.text = $"Time Elapsed: {minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
