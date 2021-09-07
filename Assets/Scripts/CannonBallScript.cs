using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float projectileSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileSpeed = 10f;
    }
    private void FixedUpdate()
    {
        transform.position += transform.right * projectileSpeed * Time.deltaTime;
    }
}
