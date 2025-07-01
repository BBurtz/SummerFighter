using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Character : MonoBehaviour
{
    public PlayerInput playerControls;

    public bool FacingRight;
    public bool inAction;
    public bool fastFalling;

    private InputAction MoveAction;
    private InputAction JumpAction;
    private InputAction AttackAction;
    private InputAction StrongAction;
    private InputAction SpecialAction;

    public int MaxInAirJumps;
    private int currentJumps;

    private bool CurrentlyMoving;
    public bool grounded;
    public bool LedgeGrabbed;
    private Vector2 MoveVal;

    public float AccelerationSpeed;
    public float maxSpeed;
    public float groundedJumpForce;
    public float AirJumpForce;
    public float GravityForce;

    public Rigidbody2D rb;

    public Vector2 LedgeGrabPosition;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MoveVal = MoveAction.ReadValue<Vector2>();
        if (LedgeGrabbed)
        {
            rb.gravityScale = 0;
            if(MoveVal.x > 0.1 || MoveVal.x < -0.1)
            {
                if (FacingRight)
                {
                    if (MoveVal.x > .1)
                    {
                        Debug.Log("normal Getup");
                    }
                    else if (MoveVal.x < -.1)
                    {
                        Debug.Log("drop");
                    }
                }
                else
                {
                    if (MoveVal.x < -.1)
                    {
                        Debug.Log("normal Getup");
                    }
                    else if (MoveVal.x > .1)
                    {
                        Debug.Log("drop");
                    }
                }
            }
        }
        else
        {
            if (!fastFalling)
            {
                resetGravity();
            }

            if (grounded)
            {
                currentJumps = MaxInAirJumps;
            }

            if(!grounded && MoveVal.y < -.8)
            {
                //fastfall
                rb.gravityScale = 1.75f * GravityForce;
                fastFalling = true;
            }

            var c = MoveVal.x;
            if (c < -0.1 || c > 0.1)
            {
                rb.AddForce(new Vector2(c * AccelerationSpeed, 0));
            }
            else if (grounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector2 limitedVel = rb.velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y);
            }
        }

    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        JumpAction = playerControls.currentActionMap.FindAction("Jump");
        StrongAction = playerControls.currentActionMap.FindAction("Strong");
        AttackAction = playerControls.currentActionMap.FindAction("Attack");
        SpecialAction = playerControls.currentActionMap.FindAction("Special");
        JumpAction.started += Jump;
        MoveAction.performed += move;
        MoveAction.canceled -= stop;
        StrongAction.performed += StrongStart;
        StrongAction.canceled += StrongLaunch;
        AttackAction.started += Attack;
        SpecialAction.started += Special;
    }

    private void Special(InputAction.CallbackContext context)
    {
        if (!LedgeGrabbed)
        {
            resetGravity();
        }

        if (MoveVal.magnitude <= 0.1)
        {
            //NSpecial
        }
        else if (MathF.Abs(MoveVal.x) > Mathf.Abs(MoveVal.y))
        {
            //Side Special
        }
        else if (MoveVal.y > 0)
        {
            //Up Special
        }
        else
        {
            //Down Special
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (!LedgeGrabbed)
        {
            resetGravity();
        }
        if (MoveVal.magnitude <= 0.1)
        {
            //Jab/nair
        }
        else if(MathF.Abs(MoveVal.x) > Mathf.Abs(MoveVal.y))
        {
            //Ftilt/bair/fair
        }
        else if(MoveVal.y > 0)
        {
            //upTilt/upAir
        }
        else
        {
            //DownTilt/Dair
        }
    }

    private void StrongLaunch(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void StrongStart(InputAction.CallbackContext context)
    {
        if (!LedgeGrabbed)
        {
            resetGravity();
        }

        if (MoveVal.magnitude <= 0.1)
        {
            //FStrong/nair
        }
        else if (MathF.Abs(MoveVal.x) > Mathf.Abs(MoveVal.y))
        {
            //FStrong/bair/fair
        }
        else if (MoveVal.y > 0)
        {
            //UpStrong/upAir
        }
        else
        {
            //DownStrong/Dair
        }
    }

    private void move(InputAction.CallbackContext context)
    {
        CurrentlyMoving = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!LedgeGrabbed)
        {
            resetGravity();
        }

        if (grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, groundedJumpForce));
        }
        else if (currentJumps > 0)
        {
            currentJumps--;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, AirJumpForce));
        }

    }
    private void stop(InputAction.CallbackContext context)
    {
        /*StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;*/
        CurrentlyMoving = false;
        MoveVal = new Vector2(0, rb.velocity.y);
    }

    public void grabLedge(GameObject Ledge)
    {
        LedgeGrabbed = true;
        if (Ledge.GetComponent<Ledges>().LeftLedge)
        {
            FacingRight = true;
            Vector2 LedgePosition = Ledge.transform.position;
            LedgePosition.y -= LedgeGrabPosition.y;
            LedgePosition.x -= LedgeGrabPosition.x;
            transform.position = LedgePosition;
        }
        else
        {
            FacingRight = false;
            Vector2 LedgePosition = Ledge.transform.position;
            LedgePosition.y -= LedgeGrabPosition.y;
            LedgePosition.x += LedgeGrabPosition.x;
            transform.position = LedgePosition;
        }

        //inviciblility
    }

    private void resetGravity()
    {
        rb.gravityScale = GravityForce;
        fastFalling = false;
    }
}
