using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameEngineManager : NetworkBehaviour
{
	[SerializeField] Rigidbody2D testRB;
	[SerializeField] Rigidbody2D testRBTwo;

	[ServerRpc (RequireOwnership = false)]
	public void HandlePlayerInput_ServerRpc(ulong clientID,
								float moveInput,
								float jumpForce,
								InputManager.JumpDirection jumpDirection)
	{
		/*MovePlayer(moveInput, jumpForce, jumpDirection, rigidbody);*/ // for testing

		//if (!IsOwner) return;
		if (clientID == 0)
		{
            MovePlayer(moveInput, jumpForce, jumpDirection, testRB);
		}
		else
		{
            MovePlayer(moveInput, jumpForce, jumpDirection, testRBTwo);
        }


	}

	void MovePlayer(float moveInput, float jumpForce, InputManager.JumpDirection jumpDirection, Rigidbody2D rigidbody)
	{
		if (moveInput != 0)
		{
			rigidbody.AddForce(new Vector2(moveInput, 0), ForceMode2D.Force);
		}

		if (jumpForce > 0)
		{
			switch (jumpDirection)
			{
				case InputManager.JumpDirection.NONE:
					rigidbody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
					break;
				case InputManager.JumpDirection.LEFT:
					rigidbody.AddForce(new Vector2(-1, 2) * jumpForce, ForceMode2D.Impulse);
					break;
				case InputManager.JumpDirection.RIGHT:
					rigidbody.AddForce(new Vector2(1, 2) * jumpForce, ForceMode2D.Impulse);
					break;
				default:
					break;
			}
		}
	}
}
