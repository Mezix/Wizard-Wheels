using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField]
    private Button trackCameraButton;
    [SerializeField]
    private Button matchSpeedButton;
    private Canvas canvas;
    public GameObject hpBar;
    public Text tankNameText;
    void Awake()
    {
        InitUI();
    }

    private void InitUI()
    {
        canvas = GetComponent<Canvas>();
        trackCameraButton.onClick = new Button.ButtonClickedEvent();
        trackCameraButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToObject(transform.root));
        canvas.sortingLayerName = "VehicleUI";
    }
}
