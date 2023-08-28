using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EmotesManager : NetworkBehaviour
{
	public enum EmoteVariants
	{
		LAUGH,
		CRYING_LAUGH,
		ANGRY
	}

	[SerializeField] List<GameObject> leftPlayerEmoteObjects = new();
	[SerializeField] List<GameObject> rightPlayerEmoteObjects = new();

	public void SendLaughEmote()
	{
		SendEmote_ServerRpc(NetworkManager.Singleton.LocalClientId, (int)EmoteVariants.LAUGH);
	}

	public void SendCryingLaughEmote()
	{
		SendEmote_ServerRpc(NetworkManager.Singleton.LocalClientId, (int)EmoteVariants.CRYING_LAUGH);
	}

	public void SendAngryEmote()
	{
		SendEmote_ServerRpc(NetworkManager.Singleton.LocalClientId, (int)EmoteVariants.ANGRY);
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
			leftPlayerEmoteObjects[emoteNumber].SetActive(true);
		}
		else
		{
			rightPlayerEmoteObjects[emoteNumber].SetActive(true);
		}
	}
}
