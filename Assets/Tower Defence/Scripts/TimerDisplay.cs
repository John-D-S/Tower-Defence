using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static StaticObjectHolder;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerDisplay : MonoBehaviour
{
    //the text that displays the timer
    private TextMeshProUGUI timerText;

    private void Start()
    {
        //initialize the timerText component
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //get the time, round it and give it to the score system to save when the game ends
        int time = Mathf.RoundToInt(Time.time);
        timerText.text = time.ToString();
        theScoreSystem.Score = time;
    }
}
