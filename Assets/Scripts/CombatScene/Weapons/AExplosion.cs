using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AExplosion : MonoBehaviour
{
    public Animator _animator;
    public float animLength;
    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
    public void InitExplosion(Vector3 position, float scale = 1)
    {
        transform.SetParent(null);
        transform.position = position;
        transform.localScale = Vector3.one * scale;
        StartCoroutine(Explode());
    }
    private IEnumerator Explode()
    {
        _animator.SetTrigger("Explode");
        yield return new WaitForSeconds(animLength);
        ObjectPool.Instance.AddToPool(GetComponent<PoolableObject>());
    }
}