using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    public int scrapValue = 0;
    public GameObject _targetingCircle;
    private void FixedUpdate()
    {
        _targetingCircle.SetActive(false);
    }
    public void InitScrap(int val)
    {
        scrapValue = val;
    }
    public void PickUpScrap(Transform scrapParent)
    {
        Ref.UI._upgradeScreen.AddNewScrap(scrapValue, true);

        transform.parent = scrapParent;
        transform.localPosition = Vector3.zero;
    }
    public void RemoveScrap()
    {
        Destroy(gameObject);
    }
    public void Highlight()
    {
        _targetingCircle.SetActive(true);
    }
}
