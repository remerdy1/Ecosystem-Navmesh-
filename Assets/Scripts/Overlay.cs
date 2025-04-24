using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    [SerializeField] private TMP_Text preyCount;
    [SerializeField] private TMP_Text predatorCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text timeElapsed;
    [SerializeField] private Queue<string> textQueue = new Queue<string>();
    [SerializeField] private TMP_Text[] dialogueTMPText = new TMP_Text[6];

    [SerializeField] private Simulation simulation;

    void Update()
    {
        int minutes = Mathf.FloorToInt(simulation.elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(simulation.elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((simulation.elapsedTime * 100f) % 100);


        preyCount.text = $"Prey Count: {simulation.GetCurrentPreyCount()}";
        predatorCount.text = $"Predator Count: {simulation.GetCurrentPredatorCount()}";
        foodCount.text = $"Food Count: {simulation.GetCurrentFoodCount()}";
        timeElapsed.text = $"Time Elapsed: {minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void AddTextToDialogue(string text)
    {
        if (textQueue.Count >= dialogueTMPText.Length) textQueue.Dequeue();

        textQueue.Enqueue(text);
        int i = 0;

        foreach (string t in textQueue)
        {
            dialogueTMPText[i].text = t;
            i++;
        }
    }
}
