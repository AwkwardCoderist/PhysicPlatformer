using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundAcceleration = 1;
    [SerializeField] private float airAcceleration = 1;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float jumpDelay = 0.5f;
    private float lastGround = 0;
    [SerializeField] private float rotateForce = 1;

    [Header("Ground Checks")]
    [SerializeField] private float checkDistance;
    [SerializeField] private Vector2 size;
    [SerializeField] private LayerMask groundLayers;
    


    private float moveInput;
    private bool jumpInput;
    private bool useInput;
    private Vector2 mousePosInput;

    [Header("Params")]
    private RaycastHit2D groundHit;
    private float gravityAngle;


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

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateTorque();
        UpdateParams();
    }

    private void UpdateMovement()
    {
        if (rb)
        {

            float acceleration = groundHit ? groundAcceleration : airAcceleration;

            rb.AddForce(transform.right * moveInput * acceleration * Time.deltaTime, ForceMode2D.Force);

            if (jumpInput && groundHit && Time.time > lastGround + jumpDelay)
            {
                rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
            }
        }

    }

    private void UpdateTorque()
    {
        if (rb)
        {
            if (groundHit)
            {
                rb.freezeRotation = true;
                rb.SetRotation(gravityAngle);
            }
            else
            {
                rb.freezeRotation = false;
                rb.AddTorque(gravityAngle * rotateForce * Time.deltaTime);
            }

        }
    }

    private void UpdateParams()
    {
        groundHit = OnGround();
        gravityAngle = Vector3.SignedAngle(Vector3.up, groundHit.normal, transform.forward);

    }

    private RaycastHit2D OnGround()
    {
        return Physics2D.BoxCast(transform.position, size, 0, Physics2D.gravity.normalized, checkDistance, groundLayers);
    }

    private void OnDrawGizmosSelected()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, size, 0, Physics2D.gravity, checkDistance, groundLayers);

        if (hit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, 0.1f);
            Gizmos.color = Color.white;
        }
        Gizmos.DrawWireCube(transform.position, size);
        Gizmos.DrawLine(rb.worldCenterOfMass, rb.worldCenterOfMass + Physics2D.gravity);
    }
}
