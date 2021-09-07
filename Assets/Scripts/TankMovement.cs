using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float _movespeed = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * _movespeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * _movespeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * _movespeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * _movespeed;
        }
    }
}
