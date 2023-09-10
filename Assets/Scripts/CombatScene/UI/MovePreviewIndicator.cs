using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePreviewIndicator : MonoBehaviour
{
    public GameObject _allObjects;
    public Color _canMoveColor;
    public Color _cantMoveColor;
    public SpriteRenderer movePreviewSpriteRenderer;
    public SpriteRenderer cantMoveIndicator;

    public void CanMoveToRoom(bool canMove)
    {
        cantMoveIndicator.gameObject.SetActive(!canMove);

        if(canMove) movePreviewSpriteRenderer.color = _canMoveColor;
        else movePreviewSpriteRenderer.color = _cantMoveColor;
    }
}
