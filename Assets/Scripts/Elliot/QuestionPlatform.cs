using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPlatform : MonoBehaviour
{
    [SerializeField] int amountOfPlayersColliding = 0;
    public QuestionManager questionManager;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7) { amountOfPlayersColliding++; }

		if (amountOfPlayersColliding >= 2) { questionManager.OpenNextQuestion_ServerRpc(); }
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7) { amountOfPlayersColliding--; }
	}
}