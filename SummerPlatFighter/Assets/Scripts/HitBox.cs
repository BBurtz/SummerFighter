using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float BaseKnockback;
    public float KnockbackScaling;
    public Vector2 KnockbackAngle;
    public int Damage;
    public int ShieldDamage;
    public int Priority;
    public bool active;
    public List<HitBox> HitBoxes;
    public int hitstunFrames;

    public Character Owner;
    private void Start()
    {
        active = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Counter" && active && Owner != collision.gameObject.GetComponent<Shield>().owner)
        {
            collision.gameObject.GetComponent<HurtBox>().callCounter();
            active = false;
            foreach (HitBox hitBox in HitBoxes)
            {
                hitBox.active = false;
            }
        }
        else if (collision.gameObject.tag == "Shield" && active && Owner != collision.gameObject.GetComponent<Shield>().owner)
        {
            collision.gameObject.GetComponent<Shield>().takeDamage(ShieldDamage);
            active = false;
            foreach (HitBox hitBox in HitBoxes)
            {
                hitBox.active = false;
            }
        }
        else if (collision.gameObject.tag == "HurtBox" && active && Owner != collision.gameObject.GetComponent<HurtBox>().character)
        {
            collision.gameObject.GetComponent<HurtBox>().character.TakeKnockback(BaseKnockback, KnockbackScaling, KnockbackAngle, true, Damage, Priority, hitstunFrames);
            active = false;
            foreach(HitBox hitBox in HitBoxes)
            {
                hitBox.active = false;
            }
            //Owner.GainMeter(Damage);
        }
    }
}
