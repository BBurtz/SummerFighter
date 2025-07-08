using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int ShieldHealth;
    public bool Shielded;
    public int ShieldHealthRegenRate;
    public int ShieldHealthdegenRate;

    public SpriteRenderer ShieldRenderer;
    public CircleCollider2D ShieldCollider;

    public Coroutine CurrentCoroutineInstance;

    private void Start()
    {
        ShieldRenderer.enabled = false;
        ShieldCollider.enabled = false;
        CurrentCoroutineInstance = StartCoroutine(ShieldRegen());
    }

    private void Update()
    {
        if (ShieldHealth <= 0)
        {
            //shieldBreak (still don't know how we want to go about that)
        }
    }
    public void ShieldUp()
    {
        if (CurrentCoroutineInstance != null)
        {
            StopCoroutine(CurrentCoroutineInstance);
        }
        Shielded = true;
        ShieldRenderer.enabled = true;
        ShieldCollider.enabled = true;
        CurrentCoroutineInstance = StartCoroutine(ShieldDegen());
    }

    public void ShieldDown()
    {
        if (CurrentCoroutineInstance != null)
        {
            StopCoroutine(CurrentCoroutineInstance);
        }
        Shielded = false;
        ShieldRenderer.enabled = false;
        ShieldCollider.enabled = false;
        CurrentCoroutineInstance = StartCoroutine(ShieldRegen());
    }

    public IEnumerator ShieldRegen()
    {
        while (!Shielded)
        {
            ShieldHealth += ShieldHealthRegenRate;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ShieldDegen()
    {
        while (Shielded)
        {
            ShieldHealth -= ShieldHealthRegenRate;
            yield return new WaitForEndOfFrame();
        }
    }

    public void takeDamage(int damage)
    {
        if (Shielded)
        {
            ShieldHealth -= damage;
        }
    }
}
