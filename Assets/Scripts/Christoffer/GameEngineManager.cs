using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class GameEngineManager : NetworkBehaviour
{
	List<NetworkObject> playerObjects = new();
	List<Rigidbody> playerRigidbodies = new();

	[SerializeField] Rigidbody testRB;

	public void HandlePlayerInput(ulong senderNetworkID,
								float moveInput,
								bool isJumping)
	{
		MovePlayer(moveInput, isJumping,  testRB);
		Debug.Log(senderNetworkID);
		if (!IsHost) return;
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].OwnerClientId == senderNetworkID)
			{
				MovePlayer(moveInput, isJumping, playerRigidbodies[i]);
			}
		}
	}

	void MovePlayer(float moveInput, bool isJumping, Rigidbody rigidbody)
	{
		Debug.Log(moveInput);
		Debug.Log(isJumping);
	}
}
