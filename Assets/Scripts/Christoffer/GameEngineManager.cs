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

	[Header("Player Animations")]
	[SerializeField] SpriteRenderer playerOneSpriteRenderer;
	[SerializeField] Sprite idlePlayerOne;
	[SerializeField] Sprite jumpPlayerOne;
	[SerializeField] Sprite airPlayerOne;
	[SerializeField] SpriteRenderer playerTwoSpriteRenderer;
    [SerializeField] Sprite idlePlayerTwo;
    [SerializeField] Sprite jumpPlayerTwo;
    [SerializeField] Sprite airPlayerTwo;

    [ServerRpc (RequireOwnership = false)]
	public void HandlePlayerInput_ServerRpc(ulong clientID,
								float moveInput,
								float jumpForce,
								InputManager.JumpDirection jumpDirection,
								bool isGrounded,
								bool isChargingJump)
	{

		AnimatePlayer(clientID, jumpDirection, isChargingJump, isGrounded);

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
					rigidbody.AddForce(new Vector2(0, 2.5f) * jumpForce, ForceMode2D.Impulse);
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

	void AnimatePlayer(ulong clientID, InputManager.JumpDirection jumpDirection, bool isChargingJump, bool isGrounded)
	{
		if (clientID == 0)
		{
			playerOneSpriteRenderer.flipX = jumpDirection == InputManager.JumpDirection.RIGHT ? false : 
											jumpDirection == InputManager.JumpDirection.LEFT ? true : 
											playerOneSpriteRenderer.flipX;

			if (!isChargingJump && isGrounded)
			{
				playerOneSpriteRenderer.sprite = idlePlayerOne;
				return;
			}

			playerOneSpriteRenderer.sprite = isChargingJump ? jumpPlayerOne : airPlayerOne;
		}
		else
		{
            playerTwoSpriteRenderer.flipX = jumpDirection == InputManager.JumpDirection.RIGHT ? false :
											jumpDirection == InputManager.JumpDirection.LEFT ? true :
											playerTwoSpriteRenderer.flipX;

            if (!isChargingJump && isGrounded)
            {
                playerTwoSpriteRenderer.sprite = idlePlayerTwo;
                return;
            }

            playerTwoSpriteRenderer.sprite = isChargingJump ? jumpPlayerTwo : airPlayerTwo;
        }
	}
}
