using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraScript : MonoBehaviour
{
    public Transform objToTrack;
    [HideInInspector]
    private Vector2 cameraOffset;
    private Vector3 mouseStartDragPos;
    private bool isMovingToPos;

    //private PixelPerfectCamera pixelCam;
    public OcclusionCulling2D _occlusionCulling2D;
    public float zoomAmount;
    public float desiredZoom;
    public float minZoom;
    public float maxZoom;

    private void Awake()
    {
        REF.Cam = this;
        _occlusionCulling2D = GetComponentInChildren<OcclusionCulling2D>();
        //pixelCam = Camera.main.GetComponent<PixelPerfectCamera>();
        isMovingToPos = false;
    }
    private void Start()
    {
        zoomAmount = 0.75f;
        maxZoom = 1f;
        desiredZoom = minZoom = 12;
        REF.CombatUI.ZoomSlider.minValue = maxZoom;
        REF.CombatUI.ZoomSlider.maxValue = minZoom;
        SetZoom(minZoom);
        Events.instance.PlayerIsDying += StopTracking;
        Events.instance.PlayerTankDestroyed += StopTracking;
        Events.instance.EnemyTankDying += CheckForEnemy;
        SetTrackedVehicleToPlayer();
    }
    
    private void CheckForEnemy(EnemyTankController enemy)
    {
        if(!enemy)
        {
            SetTrackedVehicleToPlayer();
        }
        else if (enemy.transform.Equals(objToTrack.gameObject)) SetTrackedVehicleToPlayer();
    }

    void Update()
    {
        HandleZoomInput();
        
        //ZoomToDesiredZoom();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Mouse2)) mouseStartDragPos = Input.mousePosition; //initially set from where we are draggin
            if(Input.GetKey(KeyCode.Mouse2)) ChangeCameraOffset(); //dragging motion
        }
        if (objToTrack) MoveCameraToLocalPos();
    
    }

    //  Cam Movement

    public void SetTrackedVehicleToPlayer()
    {
        if (!REF.PCon) return;

        Events.instance.DoubleClickAttempted(REF.PCon.gameObject);

        REF.EM.UntrackAllEnemyTanks();
        REF.CombatUI.TrackingTank(true);
        objToTrack = REF.PlayerGO.transform;
        transform.SetParent(objToTrack);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    public void SetTrackedVehicleToObject(Transform t)
    {
        if (t.GetComponentInChildren<PlayerTankController>()) REF.CombatUI.TrackingTank(true);
        else REF.CombatUI.TrackingTank(false);

        REF.EM.UntrackAllEnemyTanks();
        if (t.GetComponentInChildren<EnemyTankController>()) t.GetComponentInChildren<EnemyTankController>().enemyUI.TrackTank(true);

        objToTrack = t;
        transform.SetParent(objToTrack);
        HM.RotateTransformToAngle(transform, Vector3.zero);
        cameraOffset = Vector3.zero;
        isMovingToPos = true;
    }
    private void MoveCameraToLocalPos()
    {
        HM.RotateTransformToAngle(transform, Vector3.zero);
        if (isMovingToPos)
        {
            Vector3 newPos = Vector2.Lerp(transform.localPosition, Vector3.zero, 0.1f);
            newPos.z = -10;
            if (Vector3.Distance(Vector3.zero, newPos) < 1f)
            {
                newPos = Vector3.zero;
                isMovingToPos = false;
            }
            transform.localPosition = newPos;
        }
        else
        {
            transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, -10);
        }
    }
    private void StopTracking()
    {
        objToTrack = null;
        isMovingToPos = false;
        transform.parent = null;
    }
    private void ChangeCameraOffset()
    {
        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, 0) + 
                                   Camera.main.ScreenToWorldPoint(mouseStartDragPos) - 
                                   Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(!cameraOffset.Equals(Vector3.zero)) REF.CombatUI.TrackingTank(false);
        mouseStartDragPos = Input.mousePosition;
        isMovingToPos = false;
    }

    //  Zoom

    private void HandleZoomInput()
    {
        if (Input.mouseScrollDelta.y != 0 && !REF.CombatUI._settingsScript._settingsOn)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ZoomOut();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ZoomIn();
            }
        }
    }
    private void ZoomIn()
    {
        if (desiredZoom + zoomAmount > minZoom) desiredZoom = minZoom;
        else desiredZoom += zoomAmount;
        SetZoom(desiredZoom);
    }
    private void ZoomOut()
    {
        if (desiredZoom - zoomAmount < maxZoom) desiredZoom = maxZoom;
        else desiredZoom -= zoomAmount;
        SetZoom(desiredZoom);
    }
    public void SetZoom(float zoomLevel)
    {
        Camera.main.orthographicSize = zoomLevel;
        _occlusionCulling2D.UpdateCameraWidth();

        REF.CombatUI.ZoomSlider.value = (zoomLevel / minZoom) * minZoom;
        //pixelCam.assetsPPU = zoomLevel;
    }
    public void SetDesiredZoom(float zoom)
    {
        if (desiredZoom > minZoom) desiredZoom = minZoom;
        else if (desiredZoom < maxZoom) desiredZoom = maxZoom;
        else desiredZoom = zoom;
    }

    //  Camera Shake

    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    private IEnumerator Shake(float duration, float magnitude)
    {
        Camera cam = GetComponentInChildren<Camera>();
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            cam.transform.localPosition = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        cam.transform.localPosition = originalPos;
    }
}
