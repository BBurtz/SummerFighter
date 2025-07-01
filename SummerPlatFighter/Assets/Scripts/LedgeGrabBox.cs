using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabBox : MonoBehaviour
{
    public Character Character;
    private Ledges CurrentLedge;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ledge" && Character.rb.velocity.y <= 0 && !Character.inAction)
        {
            CurrentLedge = collision.gameObject.GetComponent<Ledges>();
            CurrentLedge.CurrentlyGrabbed = true;
            Character.grabLedge(collision.gameObject);
            //start invinciblity if possible
        }
    }

    public void letGoOfLedge()
    {
        CurrentLedge.CurrentlyGrabbed = false;
        CurrentLedge = null;
    }
}
