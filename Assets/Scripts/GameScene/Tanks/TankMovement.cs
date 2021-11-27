using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody2D rb;
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
        if (GetComponentInChildren<TankRotation>().rotatableObjects.Count == 0) return;

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
        TankRoomConstellation trc = GetComponent<TankGeometry>()._tankRoomConstellation;
        TankRotation tr = GetComponent<TankRotation>();

        GameObject rotatableObjects = new GameObject("RotatableObjects");
        rotatableObjects.transform.parent = transform;
        rotatableObjects.transform.localPosition = Vector3.zero;

        for (int x = 0; x < trc.XTilesAmount; x++)
        {
            for (int y = 0; y < trc.YTilesAmount; y++)
            {
                if (trc.SavedPrefabRefMatrix.XArray[x].YStuff[y].TirePrefab)
                {
                    GameObject tire = trc.SavedPrefabRefMatrix.XArray[x].YStuff[y].TirePrefab;

                    if (trc.SavedPrefabRefMatrix.XArray[x].YStuff[y].TirePrefab.GetComponentInChildren<Tire>() != null)
                    {
                        //print(x.ToString() + ", " + y.ToString());
                        if (!trc.RoomPosMatrix[x, y]) continue;
                        GameObject tireObj = Instantiate(tire);
                        tireObj.transform.parent = trc.RoomPosMatrix[x, y].transform;
                        tireObj.transform.localPosition = Vector3.zero;
                        tireObj.transform.parent = rotatableObjects.transform;
                        Tires.Add(tireObj.GetComponentInChildren<Tire>());
                        tr.rotatableObjects.Add(tireObj.GetComponentInChildren<Tire>().gameObject);
                    }
                }
            }
        }

    }
    public void SetTireAnimationSpeed()
    {
        foreach (Tire t in Tires)
        {
            t.AnimatorSpeed(currentSpeed / maxSpeed);
        }
    }
}
