using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : MonoBehaviour, ISystem
{
    public Sprite SystemSprite { get; set; }
    public GameObject SystemObj { get; set; }
    public string SystemName { get; set; }
    private void Awake()
    {
        SystemObj = gameObject;
    }
    public void InitSystem()
    {
        SystemSprite = Resources.Load("Art\\EngineSprite", typeof(Sprite)) as Sprite;
    }
    public void StartInteraction()
    {
        print("Engine go brrr hahahaha :D");
    }

    public void StopInteraction()
    {
        print("Engine stop go brrr");
    }
}
