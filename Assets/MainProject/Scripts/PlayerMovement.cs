using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float jumpForce = 1;

    [Header("Ground Checks")]
    [SerializeField] private Vector2 posOffset;
    [SerializeField] private Vector2 size;
    [SerializeField] private LayerMask groundLayers;
    


    private float moveInput;
    private bool jumpInput;
    private bool useInput;
    private Vector2 mousePosInput;

    [Header("Params")]
    private bool onGround;


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.Get<float>() == 1 ? true : false;
    }

    public void OnUse(InputValue value)
    {
        useInput = value.Get<float>() == 1 ? true : false;
    }

    public void OnMousePos(InputValue value)
    {
        mousePosInput = value.Get<Vector2>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (rb)
        {
            rb.AddForce(transform.right * moveInput * acceleration * Time.deltaTime);
            
            if (jumpInput && onGround)
            {
                rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
            }
        }

        onGround = OnGround();

    }

    private bool OnGround()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)posOffset, size, 0, Vector2.down, 0.01f, groundLayers);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)posOffset, size);
    }
}
