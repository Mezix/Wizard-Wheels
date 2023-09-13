using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalExplosion : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Explode());
    }
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}