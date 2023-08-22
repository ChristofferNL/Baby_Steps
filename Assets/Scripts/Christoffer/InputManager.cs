using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
	public enum JumpDirection
	{
		NONE,
		LEFT,
		RIGHT,
	}
	JumpDirection jumpDirection;

	[SerializeField] GameEngineManager manager;
	[SerializeField] Rigidbody2D playerRb;
	[SerializeField] Rigidbody2D playerRbTwo;
	[SerializeField] float moveForce;
	[SerializeField] float jumpForceToAdd;
	[SerializeField] float jumpForceBase;
	[SerializeField] float timeBetweenAdd;
	[SerializeField] float jumpForceTimesToAdd;
	[SerializeField] float groundCheckDistance;
	[SerializeField] float groundCheckRadius = 0.6f;
	[SerializeField] LayerMask groundCheckLayerMask;

	Rigidbody2D assignedPlayerRb;

	PlayerInputs inputActions;
	PlayerInputs.PlayerActionMapActions actions;

	private bool GameIsRunning = false;
	private bool chargingJump;
	public bool IsGrounded;
	public bool doGroundCheck = true;
	public bool canChargeWhilePulled = false;
	public bool canWalk = false;

	[SerializeField] float groundCheckPauseTime = 0.5f;

	private void Awake()
	{
		inputActions = new();
		actions = inputActions.PlayerActionMap;
	}

	public override void OnNetworkSpawn()
	{
		if (NetworkManager.Singleton.LocalClientId == 0)
		{
			assignedPlayerRb = playerRb;
		}
		else
		{
			assignedPlayerRb = playerRbTwo;
		}
	}

	private void OnEnable()
	{
		actions.Enable();
	}

	private void OnDisable()
	{
		actions.Disable();
	}

	private void Update()
	{
		if (!GameIsRunning) return;
		GroundCheck();
		GetInputs();
	}

	public void EnableControls()
	{
		GameIsRunning = true;
	}

	private void GroundCheck()
	{
		if (!doGroundCheck) return;		
		if (Physics2D.CircleCast(assignedPlayerRb.transform.position, groundCheckRadius, Vector3.down, 0.8f, groundCheckLayerMask))
		{
			IsGrounded = true;
		}
		else
		{
			IsGrounded = false;
		}
	}

	private void GetInputs()
	{
		switch (actions.Move.ReadValue<float>())
		{
			case < 0:
				jumpDirection = JumpDirection.LEFT;
				break;
			case > 0:
				jumpDirection = JumpDirection.RIGHT;
				break;
			default:
				jumpDirection = JumpDirection.NONE;
				break;
		}

		if (actions.Jump.IsPressed() && IsGrounded && !chargingJump)
		{
			chargingJump = true;
			StartCoroutine(ChargeJump());
		}
		else if (actions.Jump.WasReleasedThisFrame() && IsGrounded || actions.Jump.WasReleasedThisFrame() && canChargeWhilePulled)
		{
			chargingJump = false;
		}

		IEnumerator ChargeJump()
		{
			float jumpForce = jumpForceBase;
			int counter = 0;
			while (chargingJump && IsGrounded || chargingJump && canChargeWhilePulled)
			{
                if (counter <= jumpForceTimesToAdd)
				{
					counter++;
					jumpForce += jumpForceToAdd;
                }

                yield return new WaitForSeconds(timeBetweenAdd);
			}

			if (IsGrounded || canChargeWhilePulled)
			{
				manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId, actions.Move.ReadValue<float>() * moveForce, jumpForce, jumpDirection);
				StartCoroutine(PauseGroundCheck(groundCheckPauseTime, canChargeWhilePulled));
				yield break;
			}
		}

		if (!chargingJump && canWalk)
		{
			manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId, actions.Move.ReadValue<float>() * moveForce, 0, jumpDirection);
		}
	}

	IEnumerator PauseGroundCheck(float pauseTime, bool chargeWhilePulled)
	{
		IsGrounded = false;
		doGroundCheck = false;
		canChargeWhilePulled = false;
        yield return new WaitForSeconds(pauseTime);
		doGroundCheck = true;
        canChargeWhilePulled = chargeWhilePulled;

    }
}
