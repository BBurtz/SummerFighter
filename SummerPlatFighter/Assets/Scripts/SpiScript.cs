using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiScript : Character
{

    [Foldout("Spawning Refs")] public GameObject CounterHurtBox;
    [Foldout("Spawning Refs")] public GameObject CounterHurtBoxSpawnSpot;
    

    [ShowIf("Setup")] public Animator Animator;

    [Foldout("In Game Data")] public bool PoweredUpSpecial;
    [Foldout("In Game Data")] public bool UpSpecialMovement;

    [Foldout("Balance")] public float UpSpecialMovementSpeed;
    [Foldout("Balance")] public float UpSpecialMovementDrift;
    [Foldout("Balance")] public float UpSpecialForce;
    [Foldout("Balance")] public Vector2 sideSpecialForceAndDirection;


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (UpSpecialMovement)
        {
            var c = MoveVal.x;
            if (c < -0.1 || c > 0.1)
            {
                rb.AddForce(new Vector2(c * AccelerationSpeed, 0));
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            Vector2 temp = new Vector2(rb.velocity.x, 0);
            if (temp.magnitude > UpSpecialMovementSpeed)
            {
                Vector2 limitedVelocity = temp.normalized * UpSpecialMovementSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Jab()
    {
        
    }

    public override void Ftilt()
    {

    }

    public override void DTilt()
    {

    }

    public override void UTilt()
    {

    }

    public override void DashAttack()
    {

    }

    public override void FStrongStart()
    {

    }

    public override void FStrongRelease()
    {

    }

    public override void DownStrongStart()
    {

    }

    public override void DownStrongRelease()
    {

    }

    public override void UpStrongStart()
    {

    }

    public override void UpStrongRelease()
    {

    }

    public override void Nair()
    {

    }

    public override void Fair()
    {

    }

    public override void Bair()
    {

    }

    public override void Dair()
    {

    }

    public override void Uair()
    {

    }

    public override void NSpecial()
    {
        if (UsingMeter)
        {
            meter -= 100;
            NspecialEffect();
        }
        else
        {
            PoweredUpSpecial = false;
        }
        NspecialEffect();
    }

    public override void FSpecial()
    {
        if (UsingMeter)
        {
            meter -= 100;
            PoweredUpSpecial = true;
        }
        else
        {
            PoweredUpSpecial = false;
        }

        FSpecialEffect();
    }

    public override void DSpecial()
    {
        if (UsingMeter)
        {
            meter -= 100;
            DSpeicalEffect();
        }
        else
        {
            PoweredUpSpecial = false;
        }
        DSpeicalEffect();
    }

    public override void USpecial()
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        if (UsingMeter)
        {
            meter -= 100;
            PoweredUpSpecial = true;
        }
        else
        {
            PoweredUpSpecial = false;
        }
        USpeicalEffect();
        StartCoroutine(EndUpSpecialTimer());
        inAction = true;
    }

    public void NspecialEffect()
    {
        //Start Nspecial animation
    }

    public void FSpecialEffect()
    {
        if (FacingRight)
        {
            rb.AddForce(sideSpecialForceAndDirection);
        }
        else
        {
            rb.AddForce(new Vector2(sideSpecialForceAndDirection.x * -1, sideSpecialForceAndDirection.y));
        }
    }

    public void EndSideSpecial()
    {
        FreeFall = true; 
        inAction = false;
        //Freefall animaiton
    }

    public void DSpeicalEffect()
    {
        GameObject Temp = Instantiate(CounterHurtBox, CounterHurtBoxSpawnSpot.transform);
        Temp.GetComponent<HurtBox>().character = this;
    }

    public void USpeicalEffect()
    {
        rb.AddForce(Vector2.up.normalized * UpSpecialForce);
    }

    public void UnlockSideToSideMovement()
    {
        UpSpecialMovement = true;
    }

    public void EndUpSpecial()
    {
        FreeFall = true;
        inAction = false;
        //Freefall animaiton
        rb.velocity = Vector2.zero;
        resetGravity();
    }
    public void CounterAttack()
    {
        //Counter Animation
    }

    public IEnumerator EndUpSpecialTimer()
    {
        yield return new WaitForSecondsRealtime(.25f);
        UnlockSideToSideMovement();
        yield return new WaitForSecondsRealtime(.5f);
        EndUpSpecial();
    }
}
