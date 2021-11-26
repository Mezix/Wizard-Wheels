using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField]
    private Button trackCameraButton;
    [SerializeField]
    private GameObject hpBG;
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
        trackCameraButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToEnemy(transform));
        canvas.sortingLayerName = "VehicleUI";
    }
    public void ScaleTankHealth(float maxHp)
    {
        RectTransform r = hpBG.GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(maxHp * (20 + hpBar.GetComponent<HorizontalLayoutGroup>().spacing) + 75, r.sizeDelta.y);
    }
}
