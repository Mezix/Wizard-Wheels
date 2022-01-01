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
    [SerializeField]
    private Image matchSpeedImg;
    private bool matching;
    private Canvas canvas;
    public GameObject hpBar;
    public Text tankNameText;
    void Awake()
    {
        matching = false;
        canvas = GetComponent<Canvas>();
        //InitUI(e);
    }

    public void InitUI(string name, EnemyTankMovement e)
    {
        tankNameText.text = name;
        trackCameraButton.onClick = new Button.ButtonClickedEvent();
        trackCameraButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToObject(transform.root));
        matchSpeedButton.onClick = new Button.ButtonClickedEvent();
        matchSpeedButton.onClick.AddListener(() => MatchSpeed(e, matching));
        canvas.sortingLayerName = "VehicleUI";
    }

    public void MatchSpeed(EnemyTankMovement e, bool b)
    {
        if (b) matchSpeedImg.sprite = Resources.Load("Art\\UI\\Match_Speed_On", typeof(Sprite)) as Sprite;
        else matchSpeedImg.sprite = Resources.Load("Art\\UI\\Match_Speed_Off", typeof(Sprite)) as Sprite;
        matching = !b;
        Ref.PCon.TMov.MatchSpeed(e);
    }
}
