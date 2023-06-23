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
    private bool isScroll;

    private void Start()
    {
        worldObjectScrollSpeed = Mathf.Abs(worldObject_StartPoiny.transform.position.y - worldObject_EndPoint.transform.position.y) / worldObjectScrollTime;
        absObjectScrollSpeed = Mathf.Abs(absObject_StartPoint.transform.position.y - absObject_EndPoint.transform.position.y) / absObjectScrollTime;
    }

    public void ScrollStart()
    {
        isScroll = true;
        scoreManager.StartCount();
    }

    private void Update()
    {
        if (isScroll)
        {
            worldObject.transform.Translate(0, worldObjectScrollSpeed * Time.deltaTime, 0);
            absObject.transform.Translate(0, absObjectScrollSpeed * Time.deltaTime, 0);
        }
    }
}
