using System;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIController : MonoBehaviour
{
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Text text;

    public void UpdateHungerSlider(float currentValue)
    {
        hungerSlider.value = currentValue / 100;
    }

    public void UpdateThirstSlider(float currentValue)
    {
        thirstSlider.value = currentValue / 100;
    }

    public void UpdateText(string newText)
    {
        text.text = newText;
    }
}
