using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

public class TankMovement : MonoBehaviour
{
    public Rigidbody2D _tRB;
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
        _tRB = GetComponent<Rigidbody2D>();
        currentSpeed = 0;
    }
    public void InitSpeedStats()
    {
        if (!TryGetComponent(out TankController tc)) return;
        if (tc._vehicleInfo != null)
        {
            baseAcceleration = acceleration = tc._vehicleInfo.TankAccel;
            if (acceleration == 0) Debug.LogWarning("No Acceleration Stat!");

            baseDeceleration = deceleration = tc._vehicleInfo.TankDecel;
            if (deceleration == 0) Debug.LogWarning("No Deceleration Stat!");

            baseMaxSpeed = maxSpeed = tc._vehicleInfo.TankMaxSpeed;
            if (maxSpeed == 0) Debug.LogWarning("No max Speed");
        }
        else
        {
            baseAcceleration = acceleration = 0.005f;
            baseDeceleration = deceleration = 0.01f;
            baseMaxSpeed = maxSpeed = 5;
        }
        REF.CombatUI._engineUIScript.InitSliders();
    }
    public void Move()
    {
        moveVector = transform.up;

        float speedMultiplier = currentSpeed * Time.deltaTime;

        _tRB.velocity = moveVector * currentSpeed; //purely for display!
        _tRB.MovePosition(transform.position + moveVector * speedMultiplier);
    }
    public void Accelerate()
    {
        if (GetComponent<PlayerTankMovement>()) REF.CombatUI._engineUIScript.StartStopEngineSound(true, 0.5f);
        if (currentSpeed + acceleration * Time.timeScale < maxSpeed) currentSpeed += acceleration * Time.timeScale;
        else currentSpeed = maxSpeed;
    }
    public void Decelerate()
    {
        if (GetComponent<PlayerTankMovement>()) REF.CombatUI._engineUIScript.StartStopEngineSound(false, 1);
        if (currentSpeed - acceleration * Time.timeScale > 0) currentSpeed -= deceleration * Time.timeScale;
        else currentSpeed = 0;
    }

    //  Change the animation of our tires
    public void InitTires()
    {
        TankGeometry tank = GetComponent<TankGeometry>();
        for (int x = 0; x < tank._vehicleData.SavedXSize; x++)
        {
            for (int y = 0; y < tank._vehicleData.SavedYSize; y++)
            {
                if (tank.RoomPosMatrix[x, y] == null
                    || tank._vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].Equals(new RoomInfo())
                    || tank._vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath == null
                    || tank._vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath == "") continue;

                GameObject tireObj = Instantiate(Resources.Load(tank._vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath, typeof(GameObject))) as GameObject;
                tireObj.transform.parent = tank.RoomPosMatrix[x, y].transform;
                tireObj.transform.localPosition = Vector3.zero;
                Tire tire = tireObj.GetComponentInChildren<Tire>();
                Tires.Add(tire);
            }
        }

    }
    public void SetTireAnimationSpeed()
    {
        foreach (Tire t in Tires)
        {
            t.AnimatorSpeed(currentSpeed);
        }
    }
}
