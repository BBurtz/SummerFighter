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
    public Shield myShield;

    public int Percent;

    public bool FacingRight;
    public bool inAction;
    public bool fastFalling;
    public bool shielding;
    private bool AirDodgeing;
    private bool holdingShield;

    private InputAction MoveAction;
    private InputAction JumpAction;
    private InputAction AttackAction;
    private InputAction StrongAction;
    private InputAction SpecialAction;
    private InputAction ShieldAction;

    public int MaxInAirJumps;
    private int currentJumps;

    private bool InHitstun;
    public bool grounded;
    public bool LedgeGrabbed;
    private Vector2 MoveVal;

    public float AccelerationSpeed;
    public float maxSpeed;
    public float groundedJumpForce;
    public float AirJumpForce;
    public float GravityForce;

    public Rigidbody2D rb;

    public float airdodgeForce;

    public Vector2 LedgeGrabPosition;

    private Vector2[] PastInputs = new Vector2[2];

    public Coroutine AirdodgeCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        Percent = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(holdingShield && grounded && !shielding)
        {
            shielding = true;
            myShield.ShieldUp();
        }
        if(grounded && AirdodgeCoroutine != null)
        {
            StopCoroutine(AirdodgeCoroutine);
            rb.gravityScale = GravityForce;
            AirDodgeing = false;
            actionDone();
        }
    }

    private void FixedUpdate()
    {
        PastInputs[1] = PastInputs[0];
        PastInputs[0] = MoveVal;
        MoveVal = MoveAction.ReadValue<Vector2>();
        if (!AirDodgeing)
        {
            if (LedgeGrabbed && !InHitstun && !shielding && !inAction)
            {
                rb.gravityScale = 0;
                if (MoveVal.x > 0.1 || MoveVal.x < -0.1)
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
            else if (!InHitstun && !shielding && !inAction || !grounded)
            {
                if (!fastFalling)
                {
                    resetGravity();
                }

                if (grounded)
                {
                    currentJumps = MaxInAirJumps;
                }

                if (!grounded && MoveVal.y < -.8)
                {
                    //fastfall
                    rb.gravityScale = 1.75f * GravityForce;
                    fastFalling = true;
                }

                var c = MoveVal.x;
                if (c < -0.1 || c > 0.1)
                {
                    bool noDashDance = true;
                    if (c > 0.5 && grounded)
                    {
                        if (PastInputs[0].x < -.5 || PastInputs[1].x < -.5)
                        {
                            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                            noDashDance = false;
                        }
                    }
                    else if(c < -0.5 && grounded)
                    {
                        if(PastInputs[0].x > .5 || PastInputs[1].x > .5)
                        {
                            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                            noDashDance = false;
                        }
                    }

                    if (noDashDance)
                    {
                        rb.AddForce(new Vector2(c * AccelerationSpeed, 0));
                    }
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
            else if (shielding && !InHitstun && !inAction && grounded)
            {
                var c = MoveVal.x;
                if (c < -0.1 || c > 0.1)
                {
                    if (FacingRight)
                    {
                        if (c > 0.1)
                        {
                            //front roll
                        }
                        else if (c < -0.1)
                        {
                            //back roll
                        }
                    }
                    else
                    {
                        if (c > 0.1)
                        {
                            //back Roll
                        }
                        else if (c < -0.1)
                        {
                            //front roll
                        }
                    }
                }
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
        ShieldAction = playerControls.currentActionMap.FindAction("Shield");
        JumpAction.started += Jump;
        ShieldAction.performed += Shield;
        ShieldAction.canceled += DropShield;
        StrongAction.performed += StrongStart;
        StrongAction.canceled += StrongLaunch;
        AttackAction.started += Attack;
        SpecialAction.started += Special;
    }

    private void DropShield(InputAction.CallbackContext context)
    {
        holdingShield = false;
        if (grounded)
        {
            shielding = false;
            myShield.ShieldDown();
        }
    }

    private void Shield(InputAction.CallbackContext context)
    {
        holdingShield = true;
        if (grounded)
        {
            shielding = true;
            myShield.ShieldUp();
        }
        else
        {
            StartCoroutine(airDodge());
        }
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

    public void HorizontalBReverseCheck()
    {
        if (FacingRight)
        {
            if (MoveVal.x < -.5 || PastInputs[0].x < -.5 || PastInputs[1].x < -.5)
            {
                FacingRight = false;
                if (!grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                }
            }
        }
        else
        {
            if (MoveVal.x > .5 || PastInputs[0].x > .5 || PastInputs[1].x> .5)
            {
                FacingRight = true;
                if (!grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                }
            }
        }
    }
    
    public void VirticleBReverseCheck()
    {
        if (FacingRight)
        {
            if (MoveVal.x < -.3 || PastInputs[0].x < -.3 || PastInputs[1].x < -.3)
            {
                FacingRight = false;
                if (!grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                }
            }
        }
        else
        {
            if (MoveVal.x > .3 || PastInputs[0].x > .3 || PastInputs[1].x> .3)
            {
                FacingRight = true;
                if (!grounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
                }
            }
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

    private void Jump(InputAction.CallbackContext context)
    {
        if (shielding)
        {
            myShield.ShieldDown();
        }

        if (grounded)
        {
            if (!LedgeGrabbed)
            {
                resetGravity();
            }
            //jump squat animation
        }
        else if (LedgeGrabbed)
        {
            //ledge Jump animation
        }
        else if (currentJumps > 0)
        {
            JumpUp();
            if (!LedgeGrabbed)
            {
                resetGravity();
            }
        }
    }

    private void JumpUp()
    {
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

    public void TakeKnockback(float Base, float scaling, Vector2 angle, bool facingRight, int Damage, int Priority, int HitstunFrames)
    {
        Percent += Damage;
        //hitstop
        InHitstun = true;
        StartCoroutine(HitStunTimer(HitstunFrames));
        Vector2 KnockbackLaunch = angle.normalized * Base;
        if (!facingRight)
        {
            KnockbackLaunch.x *= -1;
        }
        KnockbackLaunch = KnockbackLaunch + (angle.normalized * scaling * Percent);
        rb.velocity = Vector2.zero;
        rb.AddForce(KnockbackLaunch, ForceMode2D.Impulse);
    }

    public IEnumerator HitStunTimer(int HitstunFrames)
    {
        int counter = 0;
        while (counter < HitstunFrames)
        {
            yield return new WaitForSecondsRealtime(1/60);
            counter++;
        }
        InHitstun = false;
    }

    public void actionDone()
    {
        inAction = false;
    }

    public IEnumerator airDodge()
    {
        AirDodgeing=true;
        inAction = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        int counter = 0;
        while (counter < 2)
        {
            counter++;
            yield return new WaitForSecondsRealtime(1 / 60);
        }
        rb.AddForce(MoveVal.normalized * airdodgeForce);
        counter = 0;
        //become invincible
        while(counter < 19)
        {
            counter++;
            yield return new WaitForSecondsRealtime(1 / 60);
        }
        counter = 0;
        rb.velocity = Vector2.zero;
        while(counter < 12)
        {
            counter++;
            yield return new WaitForSecondsRealtime(1 / 60);
        }
        rb.gravityScale = GravityForce;
        actionDone();
        AirDodgeing = false;
    }
}
