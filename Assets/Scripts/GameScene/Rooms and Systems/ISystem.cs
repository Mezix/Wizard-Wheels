using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISystem
{
    Sprite SystemSprite { get; set; }
    GameObject SystemObj { get; set; }
    string SystemName { get; set; }
    RoomPosition RoomPosForInteraction { get; set; }

    bool IsBeingInteractedWith { get; set; }
    void InitSystem();
    void StartInteraction();
    void StopInteraction();
}
