using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    [SerializeField] private float worldObjectScrollTime;
    [SerializeField] private float absObjectScrollTime;

    private float worldObjectScrollSpeed;
    private float absObjectScrollSpeed;

    [SerializeField] private GameObject worldObject;
    [SerializeField] private GameObject worldObject_StartPoiny;
    [SerializeField] private GameObject worldObject_EndPoint;
    [SerializeField] private GameObject absObject;
    [SerializeField] private GameObject absObject_StartPoint;
    [SerializeField] private GameObject absObject_EndPoint;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject outro;
    private bool isScroll;

    /// <summary>
    /// スクロールの割合を出すためのタイマー.
    /// </summary>
    private float Timer = default;

    /// <summary>
    /// スクロールの割合(0-1).
    /// </summary>
    public float Ratio => Mathf.InverseLerp(0.0f, absObjectScrollTime , Timer);

    private void Start()
    {
        worldObjectScrollSpeed = Mathf.Abs(worldObject_StartPoiny.transform.position.y - worldObject_EndPoint.transform.position.y) / worldObjectScrollTime;
        absObjectScrollSpeed = Mathf.Abs(absObject_StartPoint.transform.position.y - absObject_EndPoint.transform.position.y) / absObjectScrollTime;
    }

    public void ScrollStart()
    {
        isScroll = true;
        scoreManager.StartCount();
        Invoke(nameof(this.OutroStart),absObjectScrollTime);
    }

    private void Update()
    {
        if (isScroll)
        {
            worldObject.transform.Translate(0, worldObjectScrollSpeed * Time.deltaTime, 0);
            absObject.transform.Translate(0, absObjectScrollSpeed * Time.deltaTime, 0);
            Timer += Time.deltaTime;
        }
    }
    public void OutroStart()
    {
        outro.SetActive(true);
        isScroll = false;
    }
}
