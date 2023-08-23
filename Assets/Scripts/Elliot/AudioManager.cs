using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    [ClientRpc]
    public void PlaySound_ClientRpc(int soundId)
    {
        audioSource.PlayOneShot(audioClips[soundId]);
    }
}
