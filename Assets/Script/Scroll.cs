using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    [SerializeField] private float worldObjectScrollSpeed;
    [SerializeField] private float absObjectScrollSpeed;
    [SerializeField] private GameObject worldObject;
    [SerializeField] private GameObject absObject;

    private bool isScroll;

    public void ScrollStart()
    {
        isScroll = true;
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
