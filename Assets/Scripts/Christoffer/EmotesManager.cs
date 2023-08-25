using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EmotesManager : NetworkBehaviour
{
	public enum EmoteVariants
	{
		CLAP,
		THINKING,
		LAUGHING,
		SLEEPY
	}

	[SerializeField] List<Sprite> leftPlayerEmotes = new();
	[SerializeField] List<Sprite> rightPlayerEmotes = new();
	[SerializeField] Image leftPlayerEmoteImage;
	[SerializeField] Image rightPlayerEmoteImage;

	public void SendEmote(int emoteNumber)
	{
		SendEmote_ServerRpc(NetworkManager.Singleton.LocalClientId, emoteNumber);
	}

    [ServerRpc(RequireOwnership = false)]
	void SendEmote_ServerRpc(ulong senderID, int emoteNumber)
	{
		ShowEmote_ClientRpc(senderID, emoteNumber);
	}

	[ClientRpc]
	void ShowEmote_ClientRpc(ulong senderID, int emoteNumber)
	{
		if (senderID == 0)
		{
			rightPlayerEmoteImage.gameObject.SetActive(false);
			leftPlayerEmoteImage.gameObject.SetActive(true);
			leftPlayerEmoteImage.sprite = leftPlayerEmotes[emoteNumber];
			leftPlayerEmoteImage.GetComponent<Animator>().Play("EmoteSpawn");
			DespawnEmoteObject(leftPlayerEmoteImage.gameObject);
		}
		else
		{
            leftPlayerEmoteImage.gameObject.SetActive(false);
            rightPlayerEmoteImage.gameObject.SetActive(true);
			rightPlayerEmoteImage.sprite = rightPlayerEmotes[emoteNumber];
			rightPlayerEmoteImage.GetComponent<Animator>().Play("EmoteSpawn");
            DespawnEmoteObject(rightPlayerEmoteImage.gameObject);
        }
	}

	IEnumerator DespawnEmoteObject(GameObject theObject)
	{
		yield return new WaitForSecondsRealtime(1);
		theObject.SetActive(false);
	}
}
