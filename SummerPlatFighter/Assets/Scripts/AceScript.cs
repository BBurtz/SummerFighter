using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Threading;

public class AceScript : Character
{

    [Foldout("Projectile Refs")] public GameObject WallProjectile;
    [Foldout("Projectile Refs")] public GameObject BairCard;
    [Foldout("Projectile Refs")] public GameObject JabCard;

    [ShowIf("Setup")] public Animator Animator;

    [Foldout("In Game Data")] public bool Shuffled;
    [Foldout("In Game Data")] public bool PoweredUpSpecial;

    [Foldout("Balance")] public float ShuffledLength;
    [Foldout("Balance")] public float TeleportDistance;
    [Foldout("Balance")] public float PoweredUpTeleportDistance;
    [Foldout("Balance")] public float SpeedupTimer;
    [Foldout("Balance")] public float SpedUpGravity;
    [Foldout("Balance")] public float SpedUpAccelerationSpeed;
    [Foldout("Balance")] public float SpedUpMaxSpeed;
    [Foldout("Balance")] public float SpedUpAirDriftSpeed;
    [Foldout("Balance")] public float SpedUpAirMaxSpeed;
    [Foldout("Balance")] public float WallPushSpeed;
    [Foldout("Balance")] public float PoweredUpWallPushSpeed;

    private float BasemaxSpeed;
    private float BaseAirDriftSpeed;
    private float BaseAirMaxSpeed;
    private float BaseAccelerationSpeed;
    private float BaseGravityForce;
    // Start is called before the first frame update
    void Start()
    {
        BasemaxSpeed = maxSpeed;
        BaseAccelerationSpeed = AccelerationSpeed;
        BaseAirDriftSpeed = AirDriftSpeed;
        BaseAirMaxSpeed = AirMaxSpeed;
        BaseGravityForce = GravityForce;
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
        if(UsingMeter || Shuffled)
        {
            if (UsingMeter)
            {
                meter -= 100;
            }
            else
            {
                Shuffled = false;
            }
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
        if (UsingMeter || Shuffled)
        {
            if (UsingMeter)
            {
                meter -= 100;
            }
            else
            {
                Shuffled = false;
            }
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
        if (UsingMeter || Shuffled)
        {
            if(UsingMeter)
            {
                meter -= 100;
            }
            else
            {
                Shuffled = false;
            }
            PoweredUpSpecial = true;
        }
        else
        {
            PoweredUpSpecial = false;
        }
        USpeicalEffect();
    }

    public void NspecialEffect()
    {
        Shuffled = true;
        StartCoroutine(ShuffleTimer());
    }

    public void FSpecialEffect()
    {
        Vector2 temp = transform.position;
        if (FacingRight)
        {
            temp.x += .5f;
        }
        else
        {
            temp.x -= .5f;
        }
        GameObject Wall = Instantiate(WallProjectile, temp, Quaternion.identity);
        Wall.GetComponent<HitBox>().Owner = this;
        if (PoweredUpSpecial)
        {
            if(FacingRight)
            {
                Wall.GetComponent<Rigidbody2D>().AddForce(new Vector2(PoweredUpWallPushSpeed, 0));
            }
            else
            {
                Wall.GetComponent<Rigidbody2D>().AddForce(new Vector2(PoweredUpWallPushSpeed * -1, 0));
            }
        }
        else
        {
            if (FacingRight)
            {
                Wall.GetComponent<Rigidbody2D>().AddForce(new Vector2(WallPushSpeed, 0));
            }
            else
            {
                Wall.GetComponent<Rigidbody2D>().AddForce(new Vector2(WallPushSpeed * -1, 0));
            }
        }
    }

    public void DSpeicalEffect()
    {
        maxSpeed = SpedUpMaxSpeed;
        AccelerationSpeed = SpedUpAccelerationSpeed;
        AirDriftSpeed = SpedUpAirDriftSpeed;
        AirMaxSpeed = SpedUpAirMaxSpeed;
        GravityForce = SpedUpGravity;
        resetGravity();
        StartCoroutine(BuffTimer());
    }

    public void USpeicalEffect()
    {
        Vector2 TeleportLocation;
        if (PoweredUpSpecial)
        {
            TeleportLocation = MoveVal * PoweredUpTeleportDistance;
        }
        else
        {
            TeleportLocation = MoveVal * TeleportDistance;
            FreeFall = true;
        }

        transform.position = transform.position + new Vector3(TeleportLocation.x, TeleportLocation.y, 0);
        PoweredUpSpecial = false;
    }

    public IEnumerator ShuffleTimer()
    {
        yield return new WaitForSecondsRealtime(ShuffledLength);
        Shuffled = false;
    }

    public IEnumerator BuffTimer()
    {
        yield return new WaitForSecondsRealtime(SpeedupTimer);
        maxSpeed = BasemaxSpeed;
        AccelerationSpeed = BaseAccelerationSpeed;
        AirDriftSpeed = BaseAirDriftSpeed;
        AirMaxSpeed = BaseAirMaxSpeed;
        GravityForce = BaseGravityForce;
        resetGravity();
    }
}
