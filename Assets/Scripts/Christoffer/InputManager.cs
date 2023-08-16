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
	[SerializeField] float moveForce;
	[SerializeField] float jumpForceToAdd;
	[SerializeField] float jumpForceBase;
	[SerializeField] float timeBetweenAdd;
	[SerializeField] float jumpForceTimesToAdd;
	[SerializeField] float groundCheckDistance;
	[SerializeField] LayerMask groundCheckLayerMask;

	PlayerInputs inputActions;
	PlayerInputs.PlayerActionMapActions actions;

	private bool GameIsRunning = false;
	private bool chargingJump;
	public bool IsGrounded;
	public bool doGroundCheck = true;

	[SerializeField] float groundCheckPauseTime = 0.5f;

	private void Awake()
	{
		inputActions = new();
		actions = inputActions.PlayerActionMap;
	}

    private void Start()
    {
		StartCoroutine(StartGameCountdown());
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

	IEnumerator StartGameCountdown()
	{
		yield return new WaitForSeconds(6f);
		GameIsRunning = true;
	}

	private void GroundCheck()
	{
		if (!doGroundCheck) return;
		if (Physics2D.Raycast(playerRb.transform.position, Vector2.down, groundCheckDistance, groundCheckLayerMask))
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

		if (actions.Jump.IsPressed() && IsGrounded)
		{
			chargingJump = true;
			StartCoroutine(ChargeJump());
		}
		else if (actions.Jump.WasReleasedThisFrame() && IsGrounded)
		{
			chargingJump = false;
		}

		IEnumerator ChargeJump()
		{
			float jumpForce = jumpForceBase;
			int counter = 0;
			while (chargingJump && IsGrounded)
			{
				if (counter <= jumpForceTimesToAdd)
				{
					counter++;
					jumpForce += jumpForceToAdd;
				}
				yield return new WaitForSeconds(timeBetweenAdd);
			}

			if (IsGrounded)
			{
				manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId, actions.Move.ReadValue<float>() * moveForce, jumpForce, jumpDirection);
				StartCoroutine(PauseGroundCheck(groundCheckPauseTime));
				yield break;
			}
		}

		if (!chargingJump)
		{
			manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId, actions.Move.ReadValue<float>() * moveForce, 0, jumpDirection);
		}
	}

	IEnumerator PauseGroundCheck(float pauseTime)
	{
		IsGrounded = false;
		doGroundCheck = false;
		yield return new WaitForSeconds(pauseTime);
		doGroundCheck = true;
	}
}
