using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [HideInInspector]
    public EnemyTankController _enemy;
    public GameObject _allImages;

    //  Camera

    public GameObject overviewImages;
    private Camera renderTexCamera;
    private RenderTexture renderTex;
    private Texture2D output2DTexture; // The texture you want to convert to a sprite
    private Sprite mySprite; // The sprite you're gonna save to
    public Image _renderImage; // The image on which the sprite is gonna be displayed
    public Button _overviewButton;
    public Button _trackTankButton;
    public Text _tankDistance;

    //  Arrow

    public GameObject _arrowImages;
    public Button _arrowButton;

    private void Awake()
    {
        OpenArrow();
    }
    private void Update()
    {
        if (_enemy)
        {
            SetIndicatorPos();
            RotateArrow();
            UpdateText();
            ApplyRenderTextureToImage();
        }
    }

    private void UpdateText()
    {
        _tankDistance.text = Mathf.RoundToInt(Vector3.Distance(Ref.PCon.transform.position, _enemy.transform.position)).ToString() + " M";
    }

    public void InitIndicator(Transform parent, EnemyTankController e)
    {
        GameObject camObj = Instantiate((GameObject)Resources.Load("RenderTexCamera"));
        _enemy = e;
        camObj.transform.SetParent(_enemy.transform, false);
        camObj.transform.localPosition = new Vector3(0,0,-10);

        renderTexCamera = camObj.GetComponent<Camera>();
        renderTex = Instantiate(Resources.Load("EnemyIndicatorTexture", typeof(RenderTexture)) as RenderTexture);
        renderTexCamera.targetTexture = renderTex;

        transform.SetParent(parent);

        _arrowButton.onClick.AddListener(() => OpenOverview());
        _overviewButton.onClick.AddListener(() => OpenArrow());
        _trackTankButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToObject(e.transform.root));
    }
    private void RotateArrow()
    {
        float maxScreenWidth = Screen.width / 2f;
        float maxScreenHeight = Screen.height / 2f;
        float zRot = HM.GetAngle2DBetween(new Vector3(maxScreenWidth, maxScreenHeight, 0), Camera.main.WorldToScreenPoint(_enemy.transform.position)) + 90;
        Vector3 angle = new Vector3(0, 0, zRot);
        HM.RotateLocalTransformToAngle(_arrowButton.transform, angle);
    }

    //  Arrow
    public void OpenOverview()
    {
        _arrowImages.SetActive(false);
        overviewImages.SetActive(true);
    }

    //  Camera

    public void OpenArrow()
    {
        _arrowImages.SetActive(true);
        overviewImages.SetActive(false);
    }
    private void SetIndicatorPos()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(_enemy.transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if(onScreen)
        {
            _allImages.SetActive(false);
            return;
        }
        else _allImages.SetActive(true);

        float maxScreenWidth = Screen.width / 2f;
        float maxScreenHeight = Screen.height / 2f;
        float featherAmount = 32f * overviewImages.transform.localScale.x;

        Vector2 indicatorLocation = Camera.main.WorldToScreenPoint(_enemy.transform.position) - new Vector3(maxScreenWidth, maxScreenHeight, 0);

        if (indicatorLocation.x >= maxScreenWidth)
        {
            indicatorLocation.x = maxScreenWidth - featherAmount;
        }
        else if (indicatorLocation.x <= -maxScreenWidth)
        {
            indicatorLocation.x = -maxScreenWidth + featherAmount;
        }
        if (indicatorLocation.y >= maxScreenHeight)
        {
            indicatorLocation.y = maxScreenHeight - featherAmount;
        }
        if (indicatorLocation.y <= -maxScreenHeight)
        {
            indicatorLocation.y = -maxScreenHeight + featherAmount;
        }
        GetComponent<RectTransform>().anchoredPosition = indicatorLocation;
    }

    public void ApplyRenderTextureToImage()
    {
        output2DTexture = toTexture2D(renderTex);
        mySprite = Sprite.Create(output2DTexture, new Rect(0.0f, 0.0f, output2DTexture.width, output2DTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        _renderImage.sprite = mySprite; // apply the new sprite to the image
    }
    public Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D dest = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        dest.Apply(false);
        Graphics.CopyTexture(rTex, dest);
        return dest;
    }
}
