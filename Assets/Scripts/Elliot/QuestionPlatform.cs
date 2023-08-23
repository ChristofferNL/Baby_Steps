using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPlatform : MonoBehaviour
{
    [SerializeField] int amountOfPlayersColliding = 0;
    public QuestionManager questionManager;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7) { amountOfPlayersColliding++; }

        if(amountOfPlayersColliding >= 2) { questionManager.OpenNextQuestion_ServerRpc(); }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 7) { amountOfPlayersColliding--; }
    }
}