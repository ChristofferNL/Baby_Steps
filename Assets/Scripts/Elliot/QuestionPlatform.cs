using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionPlatform : MonoBehaviour
{
    [SerializeField] int amountOfPlayersColliding = 0;
    public QuestionManager questionManager;

	public bool hasSpawnedQuestion;
	public Vector3 flagPivotPoint;
	public Transform flagTransform;

	[SerializeField] float waitTimeBetweenExpand= 0.02f;
	[SerializeField] float scaleSpeed= 0.3f;
	[SerializeField] float expandXGoal= 100;
	[SerializeField] float flagFoldSpeed = 40;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6) { amountOfPlayersColliding++; }

		if (amountOfPlayersColliding >= 2 && !hasSpawnedQuestion) 
		{ 
			questionManager.OpenNextQuestion_ServerRpc();
			StartCoroutine(ExpandPlatform());
			StartCoroutine(FoldDownFlag());
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

	IEnumerator FoldDownFlag()
    {
		Material flagMat = flagTransform.gameObject.GetComponent<SpriteRenderer>().material;
		flagMat.SetVector("_PivotPoint", flagPivotPoint);
		while (flagTransform.eulerAngles.z < 90)
        {
			flagTransform.RotateAround(flagPivotPoint, Vector3.forward, flagFoldSpeed * Time.unscaledDeltaTime);
			yield return 0;
        }
		flagTransform.gameObject.SetActive(false);
		flagMat.SetVector("_PivotPoint", new Vector3(0,-1000,0));
	}
}