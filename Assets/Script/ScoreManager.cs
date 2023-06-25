using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

using unityroom.Api;


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
    public void StopCount()
    {
        isStart = false;
        text.text = "";
    }

    /// <summary>
    /// ランキングにスコアを送信する.
    /// </summary>
    public void SendScore()
    {
        UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);
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
