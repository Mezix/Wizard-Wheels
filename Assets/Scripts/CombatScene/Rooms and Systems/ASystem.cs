using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASystem : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> _systemRoofSprites;

    public Sprite SystemSprite;
    [HideInInspector]
    public GameObject SystemObj;
    [HideInInspector]
    public string SystemName;
    [HideInInspector]
    public RoomPosition RoomPosForInteraction;

    //  Aiming
    [Serializable]
    public enum DirectionToSpawnIn
    {
        Right,
        Down,
        Left,
        Up
    }
    public DirectionToSpawnIn _direction = DirectionToSpawnIn.Up;
    public bool _canBeRotated;

    protected bool IsBeingInteractedWith;
    public virtual void InitSystemStats()
    {
        Debug.LogWarning("InitSystemStats() Not Implemented by " + name);
    }
    public virtual void StartInteraction()
    {
        IsBeingInteractedWith = true;
    }
    public virtual void StopInteraction()
    {
        IsBeingInteractedWith = false;
    }

    public virtual void Awake()
    {
        SystemObj = gameObject;
    }
    public virtual void SpawnInCorrectDirection()
    {
        if (!_canBeRotated) return;
        if (_direction.Equals(DirectionToSpawnIn.Up)) HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, 90));
        else if (_direction.Equals(DirectionToSpawnIn.Right)) HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, 0));
        else if (_direction.Equals(DirectionToSpawnIn.Down)) HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, -90));
        else if (_direction.Equals(DirectionToSpawnIn.Left)) HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, -180));
        else HM.RotateLocalTransformToAngle(transform, new Vector3(0, 0, 0));
    }
    public void SetOpacity(bool transparent)
    {
        if (_systemRoofSprites.Count == 0)
        {
            Debug.LogWarning("Sprites of "+ gameObject.name + " not initialized");
            return;
        }
        if (transparent)
        {
            foreach (SpriteRenderer sprite in _systemRoofSprites)
            {
                Color c = sprite.color;
                sprite.color = new Color(c.r, c.g, c.b, 0.5f);
            }
        }
        else
        {
            foreach (SpriteRenderer sprite in _systemRoofSprites)
            {
                Color c = sprite.color;
                sprite.color = new Color(c.r, c.g, c.b, 1);
            }
        }
    }
}
