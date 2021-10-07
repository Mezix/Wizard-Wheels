using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    public Animator tireAnimator;
    public SpriteRenderer tireSprite;

    private void Awake()
    {
        AnimatorSpeed(0);
        HM.RotateTransformToAngle(transform, Vector3.zero);
    }
    private void Update()
    {
        
    }
    public void AnimatorSpeed(float speed)
    {
        speed = Mathf.Max(0, speed);
        tireAnimator.speed = speed;
    }
}
