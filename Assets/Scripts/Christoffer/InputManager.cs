using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : NetworkBehaviour
{
	public enum JumpDirection
	{
		NONE,
		LEFT,
		RIGHT,
	}
	JumpDirection jumpDirection;

    const float JUMP_STRAIGHT_UP_BUFFER = 2000f;

    [SerializeField] GameEngineManager manager;
	[SerializeField] Rigidbody2D playerRb;
	[SerializeField] Rigidbody2D playerRbTwo;
	[SerializeField] float moveForce;
	[SerializeField] float jumpForceToAdd;
	[SerializeField] float jumpForceBase;
	[SerializeField] float timeBetweenAdd;
	[SerializeField] float jumpForceTimesToAdd;
	[SerializeField] float groundCheckDistance;
	[SerializeField] float groundCheckRadius = 0.4f;
	[SerializeField] LayerMask pOneGroundCheckLayerMask;
	[SerializeField] LayerMask pTwoGroundCheckLayerMask;

	Rigidbody2D assignedPlayerRb;

	PlayerInputs inputActions;
	PlayerInputs.PlayerActionMapActions actions;

	public bool GameIsRunning = false;
	private bool chargingJump;
	public bool IsGrounded;
	public bool doGroundCheck = true;
	public bool canChargeWhilePulled = false;
	public bool canWalk = false;

	public Vector3 touchStartPos = Vector3.zero;
	public bool isGettingTouch;
	public int movementTouchId;

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
		if (NetworkManager.Singleton.LocalClientId == 0)
		{
            if (Physics2D.CircleCast(assignedPlayerRb.transform.position, groundCheckRadius, Vector3.down, 0.8f, pOneGroundCheckLayerMask))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }
		else
		{
            if (Physics2D.CircleCast(assignedPlayerRb.transform.position, groundCheckRadius, Vector3.down, 0.8f, pTwoGroundCheckLayerMask))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

    }

#if UNITY_ANDRIOD
	public void StartGettingTouchInput()
    {
		if (!GameIsRunning) return;
		if (Touchscreen.current == null)
		{
            for (int i = 0; i < Touchscreen.current.touches.Count; i++)
            {
                //Debug.LogError("touch pos:"+ i + " : " + Touchscreen.current.touches[i].position.ReadValue());
                //figures out the id of the touch which caused the startmovment function to get called and saves that id to use as a reference when getting input later
                if (Touchscreen.current.touches[i].position.ReadValue() != Vector2.zero && !isGettingTouch && IsGrounded)
                {
                    movementTouchId = i;
                    isGettingTouch = true;
                    touchStartPos = Touchscreen.current.touches[movementTouchId].startPosition.ReadValue();
                    StartChargeJump();
                }
            }
		}
	}
#else
    public void StartGettingTouchInput()
	{
        if (!GameIsRunning) return;
		if (!isGettingTouch)
		{
            isGettingTouch = true;
            touchStartPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
            if (IsGrounded) StartChargeJump();
        }
    }
#endif


	public void StopGettingTouchInput()
	{
        if (!GameIsRunning) return;
        isGettingTouch = false;
		//movementTouchId = 9999;
		StopChargeJump();
    }

    private void StartChargeJump()
    {


        chargingJump = true;
        StartCoroutine(ChargeJump());
    }

    private void StopChargeJump()
    {
        chargingJump = false;
    }

    private void GetInputs()
	{
        if (!isGettingTouch)
		{
			//switch (actions.Move.ReadValue<float>())
			//{
			//	case < 0:
			//		jumpDirection = JumpDirection.LEFT;
			//		break;
			//	case > 0:
			//		jumpDirection = JumpDirection.RIGHT;
			//		break;
			//	default:
			//		jumpDirection = JumpDirection.NONE;
			//		break;
			//}

			//if (actions.Jump.IsPressed() && IsGrounded && !chargingJump)
			//{
			//	StartChargeJump();
			//}
			//else if (actions.Jump.WasReleasedThisFrame() && IsGrounded || actions.Jump.WasReleasedThisFrame() && canChargeWhilePulled)
			//{
			//	StopChargeJump();
			//}

			//if (!chargingJump && canWalk)
			//{
			//	manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId,
			//										actions.Move.ReadValue<float>() * moveForce,
			//										0,
			//										jumpDirection,
			//										IsGrounded,
			//										false);
			//}
			//else if (!chargingJump && !canWalk)
			//{
			//	manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId,
			//										0,
			//										0,
			//										jumpDirection,
			//										IsGrounded,
			//										false);
			//}
		}
		else
		{
            float xDistance = 0;
#if UNITY_ANDROID
			xDistance = Touchscreen.current.touches[movementTouchId].position.ReadValue().x - touchStartPos.x;
#else
            xDistance = Camera.main.WorldToScreenPoint(Input.mousePosition).x - touchStartPos.x;
#endif

            //if (Touchscreen.current.touches.Count > 0 || Touchscreen.current == null)
            //{
                
            //}
            //else
            //{

                
            //}


            switch (xDistance)
            {
                case < -JUMP_STRAIGHT_UP_BUFFER:
                    jumpDirection = JumpDirection.LEFT;
                    break;
                case > JUMP_STRAIGHT_UP_BUFFER:
                    jumpDirection = JumpDirection.RIGHT;
                    break;
                default:
                    jumpDirection = JumpDirection.NONE;
                    break;
            }
        }


		
	}

    IEnumerator ChargeJump()
    {
        float jumpForce = jumpForceBase;
        int counter = 0;
        while (chargingJump && IsGrounded || chargingJump && canChargeWhilePulled)
        {
            manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId,
                                                actions.Move.ReadValue<float>() * moveForce,
                                                0,
                                                jumpDirection,
                                                IsGrounded,
                                                true);
            if (counter <= jumpForceTimesToAdd)
            {
                counter++;
                jumpForce += jumpForceToAdd;
            }

            yield return new WaitForSeconds(timeBetweenAdd);
        }

        if (IsGrounded || canChargeWhilePulled)
        {
            manager.HandlePlayerInput_ServerRpc(NetworkManager.Singleton.LocalClientId,
                                                actions.Move.ReadValue<float>() * moveForce,
                                                jumpForce,
                                                jumpDirection,
                                                IsGrounded,
                                                false);
			PlayJumpSoundClientRpc();
            StartCoroutine(PauseGroundCheck(groundCheckPauseTime, canChargeWhilePulled));
            yield break;
        }
    }

    IEnumerator PauseGroundCheck(float pauseTime, bool chargeWhilePulled)
	{
		IsGrounded = false;
		doGroundCheck = false;
		canChargeWhilePulled = false;
        isGettingTouch = false;
        yield return new WaitForSeconds(pauseTime);
		doGroundCheck = true;
        canChargeWhilePulled = chargeWhilePulled;

    }

	[ClientRpc]
	void PlayJumpSoundClientRpc()
	{
		AudioManager.instance.PlaySound(0);
	}
}