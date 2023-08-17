using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField]
    private Button trackCameraButton;
    [SerializeField]
    private Image trackEnemyTankImage;
    [SerializeField]
    private Button matchSpeedButton;
    [SerializeField]
    private Image matchSpeedImg;
    private bool matching;
    public Canvas canvas;
    public GameObject hpBar;
    public Text tankNameText;
    public Transform _allObjects;
    void Awake()
    {
        matching = true;
        canvas = GetComponent<Canvas>();
    }
    public void InitUI(string name, EnemyTankMovement e)
    {
        tankNameText.text = name;
        trackCameraButton.onClick = new Button.ButtonClickedEvent();
        trackCameraButton.onClick.AddListener(() => REF.Cam.SetTrackedVehicleToObject(transform.root));
        matchSpeedButton.onClick = new Button.ButtonClickedEvent();
        matchSpeedButton.onClick.AddListener(() => MatchSpeed(e, matching));
        canvas.sortingLayerName = "VehicleUI";
    }
    public void MatchSpeed(EnemyTankMovement e, bool b)
    {
        REF.CombatUI._engineUIScript._matchSpeedImage.gameObject.SetActive(b);
        REF.PCon.TMov.MatchSpeed(e, b);
        if (b) matchSpeedImg.sprite = Resources.Load(GS.UIGraphics("Match_Speed_On"), typeof(Sprite)) as Sprite;
        else matchSpeedImg.sprite = Resources.Load(GS.UIGraphics("Match_Speed_Off"), typeof(Sprite)) as Sprite;
        matching = !b;
    }
    public void TrackTank(bool b)
    {
        if (b) trackEnemyTankImage.sprite = Resources.Load(GS.UIGraphics("TrackTankTrue"), typeof(Sprite)) as Sprite;
        else trackEnemyTankImage.sprite = Resources.Load(GS.UIGraphics("TrackTankFalse"), typeof(Sprite)) as Sprite;
    }

    public void KeepLevel(EnemyTankMovement e)
    {
        HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, -e.transform.rotation.eulerAngles.z));
    }
}
