using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberClaw : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<ScrapPile>())
        {
            other.GetComponent<ScrapPile>().PickUpScrap();
        }
    }
}
