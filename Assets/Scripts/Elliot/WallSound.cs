using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WallSound : MonoBehaviour
{
    //plays a sound when a player collides with the walls
    [SerializeField] float speedForSound = 2f;
    [SerializeField] int soundId = 6;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            float playerSpeed = collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            if (playerSpeed > speedForSound)
            {
                PlayWallServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    void PlayWallServerRpc()
    {
        PlayWallSoundClientRpc();
    }

    [ClientRpc]
    void PlayWallSoundClientRpc()
    {
        AudioManager.instance.PlaySound(soundId);
    }
}
