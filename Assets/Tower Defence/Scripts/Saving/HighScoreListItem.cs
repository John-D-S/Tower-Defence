using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class HighScoreListItem : MonoBehaviour
{
    [Header("-- HighScore Item Display Components --")]
    [SerializeField]
    private TextMeshProUGUI positionDisplay;
    [SerializeField]
    private TextMeshProUGUI nameDisplay;
    [SerializeField]
    private TextMeshProUGUI scoreDisplay;

    public void SetValues(int position, string name, int score)
    {
        positionDisplay.text = position.ToString();
        nameDisplay.text = name;
        scoreDisplay.text = score.ToString();
    }
}
