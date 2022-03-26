using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberClaw : MonoBehaviour
{
    public Animator _clawAnimator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<ScrapPile>())
        {
            _clawAnimator.SetBool("CloseClaw", true);
            //other.GetComponent<ScrapPile>().PickUpScrap(_arm);
        }
    }
}
