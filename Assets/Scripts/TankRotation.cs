using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotation : MonoBehaviour
{
    private float rotationspeed = 50f;
    public bool rotateBack;

    void Start()
    {
        rotateBack = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (rotateBack)
        {
            RotateBack();
        }
        HandleRotationInput();
    }
    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleRotateBack();
        else
        {
            if (Input.GetKey(KeyCode.A)) RotateLeft();
            if (Input.GetKey(KeyCode.D)) RotateRight();
        }
    }
    public void ToggleRotateBack()
    {
        rotateBack = !rotateBack;
    }

    //  ROTATION

    private void RotateBack()
    {
        float vec = transform.rotation.eulerAngles.z - 180;
        if (vec > 0)
        {
            RotateLeft();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
        if (vec < 0)
        {
            RotateRight();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
    }
    private void RotateLeft()
    {
        transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
    }
    private void RotateRight()
    {
        transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
    }
}
