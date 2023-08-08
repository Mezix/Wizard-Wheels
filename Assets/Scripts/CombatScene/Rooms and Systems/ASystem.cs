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

    protected bool IsBeingInteractedWith;
    public abstract void InitSystemStats();
    public abstract void StartInteraction();
    public abstract void StopInteraction();

    public virtual void Awake()
    {
        SystemObj = gameObject;
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
