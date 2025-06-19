using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Character : MonoBehaviour
{
    public PlayerInput playerControls;

    private InputAction MoveAction;
    private InputAction JumpAction;

    private bool CurrentlyMoving;
    private Vector2 MoveVal;

    public float AccelerationSpeed;
    public float maxSpeed;
    public float JumpForce;

    public Rigidbody2D rb;


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
        var c = MoveVal.x;
        if(c < -0.1 || c > 0.1)
        {
            rb.AddForce(new Vector2(c * AccelerationSpeed, 0));
        }

        if (rb.velocity.magnitude > maxSpeed)
            {
                Vector2 limitedVel = rb.velocity * maxSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y);
            }

    }

    private void OnEnable()
    {
        playerControls.ActivateInput();
        MoveAction = playerControls.currentActionMap.FindAction("Move");
        JumpAction = playerControls.currentActionMap.FindAction("Jump");
        JumpAction.started += Jump;
        MoveAction.performed += move;
        MoveAction.canceled -= stop;
    }

    private void move(InputAction.CallbackContext context)
    {
        CurrentlyMoving = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        rb.velocity = new Vector2(rb.velocity.x ,0);
        rb.AddForce(new Vector2(0, JumpForce));
    }
    private void stop(InputAction.CallbackContext context)
    {
        /*StopCoroutine(movementcoroutineInstance);
        movementcoroutineInstance = null;*/
        CurrentlyMoving = false;
        MoveVal = new Vector2(0, rb.velocity.y);
    }

}
