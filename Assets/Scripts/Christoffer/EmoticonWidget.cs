using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoticonWidget : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] AnimationClip open;
	[SerializeField] AnimationClip close;
	bool isOpening = true;

	public void StartToggleOpen()
	{
		if (isOpening)
		{
			animator.enabled = true;
			animator.Play(open.name);
			isOpening = false;
			StartCoroutine(CloseEmotesAfterSeconds(6));
		}
		else
		{
			StopAllCoroutines();
			animator.enabled = true;
			animator.Play(close.name);
			isOpening = true;
		}
	}

	IEnumerator CloseEmotesAfterSeconds(float seconds)
	{
		yield return new WaitForSecondsRealtime(seconds);
		if (!isOpening )
		{
			StartToggleOpen();
		}
	}
}
