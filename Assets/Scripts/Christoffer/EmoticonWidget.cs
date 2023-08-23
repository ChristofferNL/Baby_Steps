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
		}
		else
		{
			animator.enabled = true;
			animator.Play(close.name);
			isOpening = true;
		}
	}

}
