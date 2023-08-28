using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPlatform : MonoBehaviour
{
    [SerializeField] int amountOfPlayersColliding = 0;
    public QuestionManager questionManager;

	public bool hasSpawnedQuestion;

	[SerializeField] float waitTimeBetweenExpand= 0.02f;
	[SerializeField] float scaleSpeed= 0.3f;
	[SerializeField] float expandXGoal= 100;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6) { amountOfPlayersColliding++; }

		if (amountOfPlayersColliding >= 2 && !hasSpawnedQuestion) 
		{ 
			questionManager.OpenNextQuestion_ServerRpc();
			StartCoroutine(ExpandPlatform());
			hasSpawnedQuestion = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6) { amountOfPlayersColliding--; }
	}

	IEnumerator ExpandPlatform()
	{
		while (transform.parent.localScale.x < expandXGoal)
		{
			yield return new WaitForSecondsRealtime(waitTimeBetweenExpand);
			transform.parent.localScale = new Vector3(transform.parent.localScale.x + scaleSpeed, transform.parent.localScale.y, transform.parent.localScale.z);
		}
	}
}