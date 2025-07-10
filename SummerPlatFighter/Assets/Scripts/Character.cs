using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using NaughtyAttributes;
using System.IO;

public class Character : MonoBehaviour
{
    public bool Setup;

    [ShowIf("Setup")] public PlayerInput playerControls;
    [ShowIf("Setup")] public Shield myShield;
    [ShowIf("Setup")] public Rigidbody2D rb;

    [Foldout("In Game Data")] public int Percent;
    [Foldout("In Game Data")] public int Stocks;

    private GameManager GM;

    [Foldout("In Game Data")] public bool FacingRight;
    [Foldout("In Game Data")] public bool inAction;
    [Foldout("In Game Data")] public bool fastFalling;
    [Foldout("In Game Data")] public bool shielding;
    private bool AirDodgeing;
    private bool holdingShield;

    private InputAction MoveAction;
    private InputAction JumpAction;
    private InputAction AttackAction;
    private InputAction StrongAction;
    private InputAction SpecialAction;
    private InputAction ShieldAction;
    private InputAction MeterAction;


    [Foldout("Balance")] public int MaxInAirJumps;
    private int currentJumps;
    [Foldout("In Game Data")] public int meter;
    [Foldout("In Game Data")] public bool UsingMeter;

    [Foldout("In Game Data")] public bool InHitstun;
    [Foldout("In Game Data")] public bool grounded;
    [Foldout("In Game Data")] public bool LedgeGrabbed;
    [Foldout("In Game Data")] public bool FreeFall;
    [Foldout("In Game Data")] public Vector2 MoveVal;

    [Foldout("Balance")] public float AccelerationSpeed;
    [Foldout("Balance")] public float maxSpeed;
    [Foldout("Balance")] public float AirDriftSpeed;
    [Foldout("Balance")] public float AirMaxSpeed;
    [Foldout("Balance")] public float groundedJumpForce;
    [Foldout("Balance")] public float AirJumpForce;
    [Foldout("Balance")] public float GravityForce;



    [Foldout("Balance")] public float airdodgeForce;

    [Foldout("Balance")] public Vector2 LedgeGrabPosition;

    private Vector2[] PastInputs = new Vector2[2];

    [Foldout("In Game Data")] public Coroutine AirdodgeCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        Percent = 0;
        GM = FindObjectOfType<GameManager>();
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

    public virtual void FixedUpdate()
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
            else if (!InHitstun && !shielding && !inAction && grounded)
            {
                if (!fastFalling)
                {
                    resetGravity();
                }

                if (grounded)
                {
                    currentJumps = MaxInAirJumps;
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

                Vector2 temp = new Vector2(rb.velocity.x, 0);
                if (temp.magnitude > maxSpeed)
                {
                    Vector2 limitedVelocity = temp.normalized * maxSpeed;
                    rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y);
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
            else if (!shielding && !InHitstun && !grounded)
            {

                if (!grounded && MoveVal.y < -.8)
                {
                    //fastfall
                    rb.gravityScale = 1.75f * GravityForce;
                    fastFalling = true;
                }

                var c = MoveVal.x;
                if (c < -0.1 || c > 0.1)
                {
                   rb.AddForce(new Vector2(c * AirDriftSpeed, 0));
                }
                Vector2 temp = new Vector2(rb.velocity.x, 0);
                if (temp.magnitude > AirMaxSpeed)
                { 
                    Vector2 limitedVelocity = temp.normalized * AirMaxSpeed;
                    rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y);
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
            NSpecial();
        }
        else if (MathF.Abs(MoveVal.x) > Mathf.Abs(MoveVal.y))
        {
            if(MoveVal.x > 0)
            {
                FacingRight = true;
            }
            else if (MoveVal.x < 0)
            {
                FacingRight = false;
            }
            FSpecial();
        }
        else if (MoveVal.y > 0)
        {
            USpecial();
        }
        else
        {
            DSpecial();
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (!LedgeGrabbed)
        {
            resetGravity();
        }
        if (MoveVal.magnitude <= 0.1 && !inAction)
        {
            if (grounded)
            {
                Jab();
            }
            else
            {
                Nair();
            }
        }
        else if(MathF.Abs(MoveVal.x) > Mathf.Abs(MoveVal.y) && !inAction)
        {
            if (grounded)
            {
                Ftilt();
            }
            else if ((FacingRight && MoveVal.x > 0) || (!FacingRight && MoveVal.x < 0))
            {
                Fair();
            }
            else
            {
                Bair();
            }
        }
        else if(MoveVal.y > 0 && !inAction)
        {
            if (grounded)
            {
                UTilt();
            }
            else
            {
                Uair();
            }
        }
        else if(!inAction)
        {
            if (grounded)
            {
                DTilt();
            }
            else
            {
                Dair();
            }
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
            JumpUp();
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

    public void resetGravity()
    {
        if (!LedgeGrabbed)
        {
            rb.gravityScale = GravityForce;
            fastFalling = false;
        }
    }

    public void TakeKnockback(float Base, float scaling, Vector2 angle, bool facingRight, int Damage, int Priority, int HitstunFrames)
    {
        resetGravity();
        Percent += Damage;
        GainMeter(Damage / 2);
        //hitstop
        InHitstun = true;
        StartCoroutine(HitStunTimer(HitstunFrames));
        Vector2 temp = angle + MoveVal;
        Vector2 KnockbackLaunch = temp.normalized * Base;
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

    public void Die()
    {
        Stocks--;
        //Play Death VFX
        if(Stocks == 0)
        {
            GM.GameOver();
        }
        rb.velocity = Vector2.zero;
        transform.position = GM.giveRespawnPoint().transform.position;
    }

    public void GainMeter(int Amount)
    {
        meter += Amount;
        if(meter > 300)
        {
            meter = 300;
        }
    }

    public virtual void Jab()
    {

    }

    public virtual void Ftilt()
    {

    }

    public virtual void DTilt()
    {

    }

    public virtual void UTilt()
    {

    }

    public virtual void DashAttack()
    {

    }

    public virtual void FStrongStart()
    {

    }

    public virtual void FStrongRelease()
    {

    }

    public virtual void DownStrongStart()
    {

    }

    public virtual void DownStrongRelease()
    {

    }

    public virtual void UpStrongStart()
    {

    }
    
    public virtual void UpStrongRelease()
    {

    }

    public virtual void Nair()
    {

    }

    public virtual void Fair()
    {

    }

    public virtual void Bair()
    {

    }

    public virtual void Dair()
    {

    }
    
    public virtual void Uair()
    {

    }

    public virtual void NSpecial()
    {

    }

    public virtual void FSpecial()
    {

    }

    public virtual void DSpecial()
    {

    }

    public virtual void USpecial()
    {

    }
}
