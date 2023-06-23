using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip getItemClip;
    [SerializeField] private AudioClip jump;
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void ItemGet()
    {
        m_AudioSource.PlayOneShot(getItemClip);
    }
    public void Jump()
    {
        m_AudioSource.PlayOneShot(jump);
    }
}
