using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    public int scrapValue = 0;
    public GameObject _targetingCircle;
    public bool _collecting;
    public bool _collected;
    public Collider2D _scrapCollider;
    private void Awake()
    {
        _collecting = false;
        _scrapCollider.enabled = true;
    }
    private void FixedUpdate()
    {
        _targetingCircle.SetActive(false);
    }
    public void InitScrap(int val)
    {
        scrapValue = val;
        _collected = false;
    }
    public void PickUpScrap(Transform scrapParent)
    {
        if (_collected) return;
        _collected = true;
        REF.CombatUI._upgradeScreen.AddNewScrap(scrapValue, true);
        transform.parent = scrapParent;
        transform.localPosition = Vector3.zero;
        _scrapCollider.enabled = false;
    }
    public void RemoveScrap()
    {
        Destroy(gameObject);
    }
    public void Highlight()
    {
        if(!_collecting) _targetingCircle.SetActive(true);
    }
}
