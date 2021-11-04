using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float _movespeed = 0.1f;
    public float velocity = 0f;
    public Vector3 _movementVector;
    public float acceleration = 0.0025f;
    public float deceleration = 0.005f;
    public float maxVelocity = 3f;

    public List<Tire> Tires = new List<Tire>();
    public void Move()
    {
        _movementVector = GetComponentInChildren<TankRotation>().rotatableObjects[0].transform.up;
        transform.position += _movementVector * velocity * Time.deltaTime;
    }
    public void Accelerate()
    {
        if (velocity < maxVelocity) velocity += acceleration * Time.timeScale;
        else velocity = maxVelocity;
    }
    public void Decelerate()
    {
        if (velocity > 0) velocity -= deceleration * Time.timeScale;
        else velocity = 0;
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
            t.AnimatorSpeed(velocity / maxVelocity);
        }
    }
}
