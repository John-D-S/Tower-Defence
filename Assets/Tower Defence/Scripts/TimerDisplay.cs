using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static StaticObjectHolder;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerDisplay : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int time = Mathf.RoundToInt(Time.time);
        timerText.text = time.ToString();
        theScoreSystem.Score = time;
    }
}
