using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreGetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        int result = ScoreManager.GetScore();
        text.text = result.ToString();
    }
}
