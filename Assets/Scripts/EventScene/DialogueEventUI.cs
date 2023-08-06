using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Transform _sceneTransform;
    public int sceneDirection; // 1 ==> Right, -1 == Left

    public Image _skyImage;

    public Image _cloudsImage;
    private Image cloudsParallaxImage;
    private float cloudSpeed;

    public Image _groundImage;
    private Image groundParallaxImage;
    private float groundSpeed;

    public Image _interiorImage;
    private bool vehicleMoving;
    private float timeSinceLastBump;

    public Button _nextEventButton;
    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }
    public void Init(PlayerData.EventType eventTypeToLoad)
    {
        _nextEventButton.onClick.AddListener(() => LoadNextEvent(eventTypeToLoad));

        cloudsParallaxImage = Instantiate(_cloudsImage, _sceneTransform, false);
        groundParallaxImage = Instantiate(_groundImage, _sceneTransform, false);

        _groundImage.transform.SetAsLastSibling();
        _interiorImage.transform.SetAsLastSibling();

        sceneDirection = 1;
        vehicleMoving = true;

        groundSpeed = 10;
        cloudSpeed = groundSpeed / 2f;

        groundParallaxImage.transform.localPosition = sceneDirection * Vector3.right * _groundImage.preferredWidth;
        cloudsParallaxImage.transform.localPosition = sceneDirection * Vector3.right * _cloudsImage.preferredWidth;

        StartCoroutine(StartVehicleBumps());
        StartCoroutine(ParallaxEffect());
    }
    private void LoadNextEvent(PlayerData.EventType eventToLoad)
    {
        if(eventToLoad.Equals(PlayerData.EventType.Combat)) Loader.Load(Loader.Scene.CombatScene);
        else if(eventToLoad.Equals(PlayerData.EventType.Construction)) Loader.Load(Loader.Scene.ConstructionScene);

        //  EventScene
        else if (eventToLoad.Equals(PlayerData.EventType.Dialogue)) Loader.Load(Loader.Scene.EventScene);
        else if(eventToLoad.Equals(PlayerData.EventType.FreeLoot)) Loader.Load(Loader.Scene.EventScene);
        else if(eventToLoad.Equals(PlayerData.EventType.NewWizard)) Loader.Load(Loader.Scene.EventScene);
        else if(eventToLoad.Equals(PlayerData.EventType.Shop)) Loader.Load(Loader.Scene.EventScene);
    }

    private IEnumerator ParallaxEffect()
    {
        while(vehicleMoving)
        {
            _cloudsImage.transform.localPosition        -= sceneDirection * Vector3.right * cloudSpeed * Time.deltaTime;
            cloudsParallaxImage.transform.localPosition -= sceneDirection * Vector3.right * cloudSpeed * Time.deltaTime;

            _groundImage.transform.localPosition        -= sceneDirection * Vector3.right * groundSpeed * Time.deltaTime;
            groundParallaxImage.transform.localPosition -= sceneDirection * Vector3.right * groundSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator StartVehicleBumps()
    {
        while(vehicleMoving)
        {
            timeSinceLastBump += Time.deltaTime;
            if (timeSinceLastBump > 3f)
            {
                _cloudsImage.transform.localPosition        += Vector3.up * 1;
                cloudsParallaxImage.transform.localPosition += Vector3.up * 1;
                _groundImage.transform.localPosition        += Vector3.up * 1;
                groundParallaxImage.transform.localPosition += Vector3.up * 1;

                yield return new WaitForSeconds(0.5f);

                _cloudsImage.transform.localPosition        -= Vector3.up * 1;
                cloudsParallaxImage.transform.localPosition -= Vector3.up * 1;
                _groundImage.transform.localPosition        -= Vector3.up * 1;
                groundParallaxImage.transform.localPosition -= Vector3.up * 1;

                timeSinceLastBump = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
