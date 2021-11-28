using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleExplosion : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Explode());
    }
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.435f);
        Destroy(gameObject);
    }
}