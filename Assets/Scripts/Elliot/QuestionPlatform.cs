using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPlatform : MonoBehaviour
{
    [SerializeField] int amountOfPlayersColliding = 0;
    public QuestionManager questionManager;

	bool hasSpawnedQuestion;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6) { amountOfPlayersColliding++; }

		if (amountOfPlayersColliding >= 2 && !hasSpawnedQuestion) 
		{ 
			questionManager.OpenNextQuestion_ServerRpc(); 
			hasSpawnedQuestion = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6) { amountOfPlayersColliding--; }
	}
}