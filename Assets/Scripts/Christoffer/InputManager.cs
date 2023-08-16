using LobbyRelaySample.ngo;
using System;
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

	public bool GameIsRunning;
	private bool chargingJump;
	public bool IsGrounded;

	[SerializeField]
	TMPro.TMP_Text m_nameOutput = default;
	Camera m_mainCamera;
	ulong m_localId;

	// If the local player cursor spawns before this cursor's owner, the owner's data won't be available yet. This is used to retrieve the data later.
	Action<ulong, Action<PlayerData>> m_retrieveName;

	private void Awake()
	{
		inputActions = new();
		actions = inputActions.PlayerActionMap;
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
	public override void OnNetworkSpawn()
	{
		m_retrieveName = NetworkedDataStore.Instance.GetPlayerData;
		m_mainCamera = GameObject.Find("InGameCamera").GetComponent<Camera>();
		InGameRunner.Instance.onGameBeginning += OnGameBegan;

		m_localId = NetworkManager.Singleton.LocalClientId;
	}
	public void OnGameBegan()
	{
		m_retrieveName.Invoke(OwnerClientId, SetName_ClientRpc);
		InGameRunner.Instance.onGameBeginning -= OnGameBegan;
	}

	[ClientRpc]
	private void SetName_ClientRpc(PlayerData data)
	{
		if (!IsOwner)
			m_nameOutput.text = data.name;
	}

	private void GroundCheck()
	{
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
				yield break;
			}
		}

		if (!chargingJump)
		{
			manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId, actions.Move.ReadValue<float>() * moveForce, 0, jumpDirection);
		}
	}
}
