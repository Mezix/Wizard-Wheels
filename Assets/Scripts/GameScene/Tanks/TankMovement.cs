using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float currentSpeed;
    [HideInInspector]
    public Vector3 moveVector;
    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    public float baseAcceleration;
    public float baseDeceleration;
    public float baseMaxSpeed;

    public List<Tire> Tires = new List<Tire>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = 0;
    }
    public void InitSpeedStats()
    {
        if (!TryGetComponent(out TankController tc)) return;
        if (tc._tStats)
        {
            baseAcceleration = acceleration = tc._tStats._tankAccel;
            if (acceleration == 0) Debug.LogWarning("No Acceleration Stat!");

            baseDeceleration = deceleration = tc._tStats._tankDecel;
            if (deceleration == 0) Debug.LogWarning("No Deceleration Stat!");

            baseMaxSpeed = maxSpeed = tc._tStats._tankMaxSpeed;
            if (maxSpeed == 0) Debug.LogWarning("No max Speed");
        }
        else
        {
            baseAcceleration = acceleration = 0.005f;
            baseDeceleration = deceleration = 0.01f;
            baseMaxSpeed = maxSpeed = 5;
        }
        Ref.UI._engineUIScript.InitSliders();
    }
    public void Move()
    {
        if (GetComponentInChildren<TankRotation>().rotatableObjects.Count == 0) return;

        moveVector = GetComponentInChildren<TankRotation>().rotatableObjects[0].transform.up;

        float speedMultiplier = currentSpeed * Time.deltaTime;

        rb.velocity = moveVector * speedMultiplier;
        rb.MovePosition(transform.position + moveVector * speedMultiplier);
    }
    public void Accelerate()
    {
        if (GetComponent<PlayerTankMovement>()) Ref.UI._engineUIScript.StartStopEngineSound(true, 0.5f);
        if (currentSpeed + acceleration * Time.timeScale < maxSpeed) currentSpeed += acceleration * Time.timeScale;
        else currentSpeed = maxSpeed;
    }
    public void Decelerate()
    {
        if(GetComponent<PlayerTankMovement>()) Ref.UI._engineUIScript.StartStopEngineSound(false, 1);
        if (currentSpeed - acceleration * Time.timeScale > 0) currentSpeed -= deceleration * Time.timeScale;
        else currentSpeed = 0;
    }

    //  Change the animation of our tires
    public void InitTires()
    {
        TankGeometry tank = GetComponent<TankGeometry>();
        TankRotation tr = GetComponent<TankRotation>();

        GameObject rotatableObjects = new GameObject("RotatableObjects");
        rotatableObjects.transform.parent = transform;
        rotatableObjects.transform.localPosition = Vector3.zero;

        for (int x = 0; x < tank._tankRoomConstellation._XSize; x++)
        {
            for (int y = 0; y < tank._tankRoomConstellation._YSize; y++)
            {
                if (tank._tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].TirePrefab)
                {
                    GameObject tire = tank._tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].TirePrefab;

                    if (tank._tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].TirePrefab.GetComponentInChildren<Tire>() != null)
                    {
                        //print(x.ToString() + ", " + y.ToString());
                        if (!tank.RoomPosMatrix[x, y]) continue;
                        GameObject tireObj = Instantiate(tire);
                        tireObj.transform.parent = tank.RoomPosMatrix[x, y].transform;
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
