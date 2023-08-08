using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TankScrapCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ScrapPile scrap))
        {
            scrap.PickUpScrap(transform);
            scrap.RemoveScrap();
        }
    }
}
