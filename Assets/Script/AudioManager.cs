using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip getItemClip;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip damage;

    [SerializeField] private float volume;

    private GameObject bgm;
    private GameObject environment;
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        bgm = transform.Find("BGM").gameObject;
        environment = transform.Find("Environment").gameObject;
    }

    public void GameStart()
    {
        bgm.SetActive(true);
        environment.SetActive(true);
    }
    public void StopGame()
    {
        bgm.SetActive(false);
        environment.SetActive(false);
    }

    public void ItemGet() => m_AudioSource.PlayOneShot(getItemClip,volume);

    public void Jump() => m_AudioSource.PlayOneShot(jump, volume);

    public void Damage() => m_AudioSource.PlayOneShot(damage, volume);
}
