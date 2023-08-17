using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] float moveForce;
    [SerializeField] float jumpForceToAdd;
    [SerializeField] float jumpForceBase;
    [SerializeField] float timeBetweenAdd;
    [SerializeField] float jumpForceTimesToAdd;


    private bool jumpLeft;
    private bool jumpRight;
    private bool chargingJump;
    public bool isGrounded;

    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask groundCheckLayerMask;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //groundcheck
        if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundCheckLayerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }



        if (Input.GetKey(KeyCode.LeftArrow))
        {
            jumpLeft = true;
            jumpRight = false;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            jumpLeft = false;
            jumpRight = true;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            jumpLeft = false;
            jumpRight = false;
            chargingJump = true;
            StartCoroutine(ChargeJump());
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && isGrounded)
        {
            chargingJump = false;
        }



        IEnumerator ChargeJump()
        {
            float jumpForce = jumpForceBase;
            int counter = 0;
            while (chargingJump && isGrounded)
            {
                if (counter <= jumpForceTimesToAdd)
                {
                    counter++;
                    jumpForce += jumpForceToAdd;
                }
                yield return new WaitForSeconds(timeBetweenAdd);
            }

            if (isGrounded)
            {
                if (jumpLeft)
                {
                    playerRb.AddForce(new Vector2(-1, 2) * jumpForce, ForceMode2D.Impulse);
                }
                else if (jumpRight)
                {
                    playerRb.AddForce(new Vector2(1, 2) * jumpForce, ForceMode2D.Impulse);
                }
                else
                {
                    playerRb.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                }
            }


            chargingJump = false;
        }
    }
}
