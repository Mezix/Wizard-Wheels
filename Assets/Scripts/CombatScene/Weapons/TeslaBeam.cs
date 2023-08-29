using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBeam : MonoBehaviour
{
    public List<Room> roomsToHit = new List<Room>();
    private SpriteRenderer _teslaSpriteRenderer;
    private BoxCollider2D _teslaBeamCollider;
    private void Awake()
    {
        _teslaSpriteRenderer = GetComponent<SpriteRenderer>();
        _teslaBeamCollider = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            if(other.GetComponentInChildren<Room>())
            {
                Room r = other.GetComponentInChildren<Room>();
                if(!r._tGeo.GetComponent<PlayerTankController>())
                {
                    if (!roomsToHit.Contains(r)) roomsToHit.Add(r);
                }
            }
        }
    }

    public void SetTeslaBeamSize(float range)
    {
        _teslaSpriteRenderer.size = new Vector2(0.5f, range);
        _teslaSpriteRenderer.transform.localPosition = new Vector3(0, range / 2 - 0.085f, 0);
        _teslaBeamCollider.size = new Vector2(0.15f, range);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other)
        {
            if (other.GetComponentInChildren<Room>())
            {
                Room r = other.GetComponentInChildren<Room>();
                if (roomsToHit.Contains(r)) roomsToHit.Remove(r);
            }
        }
    }
}
