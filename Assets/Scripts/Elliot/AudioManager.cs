using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    [ClientRpc]
    public void PlaySound(int soundId)
    {
        audioSource.PlayOneShot(audioClips[soundId]);
    }

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }    
}
