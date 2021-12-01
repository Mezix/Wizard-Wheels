using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : MonoBehaviour, ISystem
{
    public Sprite SystemSprite { get; set; }
    public GameObject SystemObj { get; set; }
    public string SystemName { get; set; }
    public RoomPosition RoomPosForInteraction { get; set; }
    public bool IsBeingInteractedWith { get; set; }
    private void Awake()
    {
        SystemObj = gameObject;
    }
    public void InitSystemStats()
    {
        SystemSprite = Resources.Load("Art\\EngineSprite", typeof(Sprite)) as Sprite;
    }
    public void StartInteraction()
    {
        IsBeingInteractedWith = true;
        //print("Engine go brrr hahahaha :D");
    }

    public void StopInteraction()
    {
        IsBeingInteractedWith = false;
        print("Engine stop go brrr");
    }
}
