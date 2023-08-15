using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
	[SerializeField] GameEngineManager manager;

	PlayerInputs inputActions;
	PlayerInputs.PlayerActionMapActions actions;

	public bool GameIsRunning;

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
		GetInputs();
	}

	private void GetInputs()
	{
		manager.HandlePlayerInput(OwnerClientId,
			actions.Move.ReadValue<float>(),
			actions.Jump.IsPressed());
	}
}
