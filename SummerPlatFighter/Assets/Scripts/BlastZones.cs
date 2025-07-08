using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZones : MonoBehaviour
{
    public bool IsTopBlastzone;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("hit Blastzone");
        if(collision.tag == "HurtBox")
        {
            Debug.Log("Hit characters hurtbox");
            if(IsTopBlastzone && collision.gameObject.GetComponent<HurtBox>().character.InHitstun)
            {
                collision.gameObject.GetComponent<HurtBox>().character.Die();
            }
            else
            {
                collision.gameObject.GetComponent<HurtBox>().character.Die();
            }
        }
    }
}
