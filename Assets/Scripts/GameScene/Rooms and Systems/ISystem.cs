using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISystem : MonoBehaviour
{
    public Sprite SystemSprite;
    [HideInInspector]
    public GameObject SystemObj;
    [HideInInspector]
    public string SystemName;
    [HideInInspector]
    public RoomPosition RoomPosForInteraction;

    protected bool IsBeingInteractedWith;
    public abstract void InitSystemStats();
    public abstract void StartInteraction();
    public abstract void StopInteraction();
}
