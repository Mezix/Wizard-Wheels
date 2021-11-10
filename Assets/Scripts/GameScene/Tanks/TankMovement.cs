using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float currentSpeed;
    public Vector3 moveVector;
    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    public List<Tire> Tires = new List<Tire>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = 0;

        acceleration = 0.005f;
        deceleration = 0.01f;
    }

    public void Move()
    {
        moveVector = GetComponentInChildren<TankRotation>().rotatableObjects[0].transform.up;
        rb.velocity = moveVector * currentSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveVector * currentSpeed * Time.deltaTime);
    }
    public void Accelerate()
    {
        if (currentSpeed + acceleration * Time.timeScale < maxSpeed) currentSpeed += acceleration * Time.timeScale;
        else currentSpeed = maxSpeed;
    }
    public void Decelerate()
    {
        if (currentSpeed - acceleration * Time.timeScale > 0) currentSpeed -= acceleration * Time.timeScale;
        else currentSpeed = 0;
    }

    //  Change the animation of our tires
    public void InitTires()
    {
        foreach (Tire t in GetComponentsInChildren<Tire>()) Tires.Add(t);
    }
    public void SetTireAnimationSpeed()
    {
        foreach (Tire t in Tires)
        {
            t.AnimatorSpeed(currentSpeed / maxSpeed);
        }
    }
}
