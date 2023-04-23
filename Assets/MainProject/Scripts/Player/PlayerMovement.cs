using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundAcceleration = 1;
    [SerializeField] private float airAcceleration = 1;
    [SerializeField] private float waterAcceleration = 1;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float jumpDelay = 0.5f;
    private float lastGround = 0;
    [SerializeField] private float rotateForce = 1;

    [Header("Ground Checks")]
    [SerializeField] private float checkDistance;
    [SerializeField] private Vector2 size;
    [SerializeField] private LayerMask groundLayers;
    


    private Vector2 moveInput;
    private bool jumpInput;
    private bool useInput;
    public IInteractable CurrentInteractable { get; set; } = null;
    private bool stickInput;
    private Vector2 mousePosInput;

    [Header("Params")]
    private RaycastHit2D groundHit;
    private float gravityAngle;

    [Header("Fixed Joint")]
    [SerializeField] private FixedJoint2D fixedJoint2D;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.Get<float>() == 1;
    }

    public void OnUse(InputValue value)
    {
        useInput = value.Get<float>() == 1;
    }

    public void OnStick(InputValue value)
    {
        stickInput = value.Get<float>() == 1;
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
        UpdateUse();
    }

    private void UpdateMovement()
    {
        if (rb)
        {
            if (stickInput)
            {
                if (groundHit && !fixedJoint2D.enabled)
                {
                    if (groundHit.collider.TryGetComponent(out Rigidbody2D body))
                    {
                        rb.freezeRotation = false;
                        fixedJoint2D.connectedBody = body;
                    }
                    else
                        fixedJoint2D.connectedBody = null;

                    fixedJoint2D.enabled = true;
                }
            }
            else
                fixedJoint2D.enabled = false;

            if (groundHit)
            {
                if (groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    rb.AddForce(moveInput * waterAcceleration * Time.deltaTime, ForceMode2D.Force);

                    if (jumpInput && Time.time > lastGround + jumpDelay)
                    {
                        rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    float acceleration = groundHit ? groundAcceleration : airAcceleration;

                    rb.AddForce((groundHit ? transform.right : Vector3.right) * moveInput * acceleration * Time.deltaTime, ForceMode2D.Force);

                    if (jumpInput && Time.time > lastGround + jumpDelay)
                    {
                        rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
            }

            else
            {
                if (!groundHit)
                    rb.AddForce(moveInput * airAcceleration * Time.deltaTime, ForceMode2D.Force);
            }            
        }
    }

    private void UpdateTorque()
    {
        if (rb)
        {
            if (fixedJoint2D.enabled == false)
            {
                if (groundHit)
                {
                    if (groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                    {

                        rb.freezeRotation = false;
                        rb.AddTorque(Vector3.SignedAngle(transform.up, rb.velocity, transform.forward));
                    }
                    else
                    {
                        rb.freezeRotation = true;
                        rb.SetRotation(gravityAngle);
                    }                    
                }
                else
                {
                    rb.freezeRotation = false;
                    rb.AddTorque(gravityAngle * rotateForce * Time.deltaTime);
                }
            }
        }
    }

    private void UpdateParams()
    {
        groundHit = OnGround();
        gravityAngle = Vector3.SignedAngle(Vector3.up, groundHit.normal, transform.forward);

    }

    private void UpdateUse()
    {
        if (CurrentInteractable != null)
        {
            if(useInput)
            {
                CurrentInteractable.Interact();
                useInput = false;
            }
        }
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
