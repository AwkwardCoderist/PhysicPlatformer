using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform sprite;
    [SerializeField] private float groundAcceleration = 1;
    [SerializeField] private float airAcceleration = 1;
    [SerializeField] private float waterAcceleration = 1;
    [SerializeField] private float waterJumpForce = 0.5f;
    [SerializeField] private float grabAcceleration = 1;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float jumpSideForce = 0.1f;
    [SerializeField] private float jumpDelay = 0.5f;
    private float lastGround = 0;
    [SerializeField] private float rotateForce = 1;
    [SerializeField] private float landRotateSmoothness = 75;

    [Header("Ground Checks")]
    [SerializeField] private float checkDistance;
    [SerializeField] private Vector2 size;
    [SerializeField] private LayerMask groundLayers;

    [Header("Drops")]
    [SerializeField] private Transform leftDrop;
    [SerializeField] private Transform leftDropRoot;
    [SerializeField] private Transform rightDrop;
    [SerializeField] private Transform rightDropRoot;
    [SerializeField] private float dropsOffset = 15;
    [SerializeField] private float dropsSmoothness = 15;
    [SerializeField] private float dropsForce = 1;
    [SerializeField] private AnimationCurve leftRootCurve;
    [SerializeField] private AnimationCurve rightRootCurve;
    [SerializeField] private float rootLoopTime = 5;
    private float rootRotateTime = 0;

    [Header("Animations Params")]
    [SerializeField] private float pushDistance = 0.5f;
    [SerializeField] private LayerMask pushLayers;

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
        UpdateAnimator();
        UpdateDrops();
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

            if (fixedJoint2D.enabled)
            {
                if(fixedJoint2D.connectedBody)
                    fixedJoint2D.connectedBody.AddForceAtPosition(moveInput * grabAcceleration,fixedJoint2D.anchor, ForceMode2D.Force);
            }
            else
            {
                if (groundHit)
                {
                    if (groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                    {
                        rb.AddForce(moveInput * waterAcceleration, ForceMode2D.Force);

                        if (jumpInput && Time.time > lastGround + jumpDelay)
                        {
                            rb.AddForce(transform.up * waterJumpForce, ForceMode2D.Impulse);
                        }
                    }
                    else
                    {
                        float acceleration = groundHit ? groundAcceleration : airAcceleration;

                        rb.AddForce((groundHit ? transform.right : Vector3.right) * moveInput * acceleration, ForceMode2D.Force);

                        if (jumpInput && Time.time > lastGround + jumpDelay)
                        {
                            rb.AddForce((transform.up * jumpForce) + (transform.right * moveInput.x * jumpSideForce), ForceMode2D.Impulse);
                        }
                    }
                }

                else
                {
                    if (!groundHit)
                        rb.AddForce(moveInput * airAcceleration, ForceMode2D.Force);
                }
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
                        rb.AddTorque(Vector3.SignedAngle(transform.up, -Physics2D.gravity, transform.forward)*0.1f);
                    }
                    else
                    {
                        rb.freezeRotation = true;
                        rb.SetRotation(Quaternion.RotateTowards(Quaternion.Euler(0,0,rb.rotation), Quaternion.Euler(0, 0, gravityAngle), Time.deltaTime * landRotateSmoothness));
                    }                    
                }
                else
                {
                    rb.freezeRotation = false;
                    rb.AddTorque(gravityAngle * rotateForce);
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

    private void UpdateAnimator()
    {
        if (fixedJoint2D.enabled)
        {
            anim.Play("Player_grab");
        }
        else if (groundHit)
        {
            if (groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                sprite.localScale = new Vector3(
                    Mathf.Abs(sprite.transform.localScale.x) * Mathf.Sign(moveInput.x),
                    anim.transform.localScale.y,
                    anim.transform.localScale.z);

                anim.Play("Player_swim");
            }
            else if (moveInput.x != 0)
            {
                sprite.localScale = new Vector3 (
                    Mathf.Abs(sprite.transform.localScale.x) * Mathf.Sign(moveInput.x),
                    anim.transform.localScale.y,
                    anim.transform.localScale.z);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * sprite.localScale.x, pushDistance, pushLayers);

                if (hit)
                    anim.Play("Player_runPush");
                else
                    anim.Play("Player_run");


            }
            else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_idle"))
            {
                anim.Play("Player_idle");
            }
        }
        else
        {
            anim.Play("Player_fall");
        }
    }

    private void UpdateDrops()
    {
        leftDropRoot.Rotate(0, 0, leftRootCurve.Evaluate(rootRotateTime / rootLoopTime) * dropsForce);
        rightDropRoot.Rotate(0, 0, rightRootCurve.Evaluate(rootRotateTime / rootLoopTime) * dropsForce);

        rootRotateTime += Time.deltaTime;
            
        if(rootRotateTime > rootLoopTime)
        {
            rootRotateTime = 0;
        }

        leftDrop.rotation = Quaternion.RotateTowards(leftDrop.rotation, Quaternion.LookRotation(transform.forward, -Physics2D.gravity) * Quaternion.Euler(0, 0, -dropsOffset), dropsSmoothness * Time.deltaTime);
        rightDrop.rotation = Quaternion.RotateTowards(rightDrop.rotation, Quaternion.LookRotation(transform.forward, -Physics2D.gravity) * Quaternion.Euler(0, 0, dropsOffset), dropsSmoothness * Time.deltaTime);

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

        Gizmos.DrawLine(transform.position, transform.position + transform.right * sprite.localScale.x * pushDistance);
    }
}
