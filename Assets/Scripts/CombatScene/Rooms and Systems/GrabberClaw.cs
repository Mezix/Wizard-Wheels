using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberClaw : MonoBehaviour
{
    public Animator _clawAnimator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<LootCrate>())
        {
            _clawAnimator.SetBool("CloseClaw", true);
        }
    }
}
