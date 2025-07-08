using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileScript : MonoBehaviour
{
    public float ProjectileLife;

    private void Start()
    {
        StartCoroutine(ProjectileTimer());
    }

    public IEnumerator ProjectileTimer()
    {
        yield return new WaitForSecondsRealtime(ProjectileLife);
        Destroy(gameObject);
    }
}
