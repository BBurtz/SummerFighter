using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public Character character;
    public bool counter;

    public void callCounter()
    {
        SpiScript temp = (SpiScript)character;
        if (counter && temp != null)
        {
            temp.CounterAttack();
        }
    }
}
