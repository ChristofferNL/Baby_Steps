using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2;
    private Vector2 moveVector;

    // Update is called once per frame
    void Update()
    {
        moveVector = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) { moveVector += Vector2.left; }
        if (Input.GetKey(KeyCode.D)) { moveVector += Vector2.right; }
        if (Input.GetKey(KeyCode.W)) { moveVector += Vector2.up; }
        if (Input.GetKey(KeyCode.S)) { moveVector += Vector2.down; }
        moveVector.Normalize();
        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
    }
}
