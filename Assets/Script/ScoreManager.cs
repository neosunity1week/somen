using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    private bool isStart = false;
    [NonSerialized] public int score;
    [SerializeField] private TextMeshProUGUI text;

    public void AddScore(int amount)
    {
        score += amount;
        DisplayUpdate();
    }
    private void DisplayUpdate()
    {
        text.text = $"score:{score}";
    }
    public void StartCount()
    {
        isStart = true;
    }

    private float elipsedTime;
    private void Update()
    {
        if(!isStart) return;
        elipsedTime += Time.deltaTime;
        if(elipsedTime > 1.0f)
        {
            score += 10;
            elipsedTime = 0f;
            DisplayUpdate();
        }
    }
}
