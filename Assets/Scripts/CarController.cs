using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float turnSpeed = 200f; 
    
    public WallManager wallManager;
    private float moveInput, turnInput;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(wallManager.WasWallHit()){
            rb.velocity = Vector2.zero;
            return;
        }
        GetInputs();
        HandleMovement();
        if(isMoving()) HandleTurning();
    }
    private void GetInputs(){
        moveInput = Input.GetAxis("Vertical"); 
        turnInput = moveInput >= 0 ? Input.GetAxis("Horizontal") : - Input.GetAxis("Horizontal"); 
    }

    private void HandleMovement()
    {
        Vector2 forward = transform.up * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forward);
    }

    private void HandleTurning(){
        float turn = -turnInput * turnSpeed * Time.deltaTime;
        rb.rotation += turn;
    }

    private bool isMoving() => moveInput != 0;

}
