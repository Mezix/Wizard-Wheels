using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileProjectile : AProjectile
{
    public bool hasLaunched;
    public Animator _magicMissileAnimator;
    public float launchTime = 0.5f;
    public override void OnEnable()
    {
        base.OnEnable();
        _magicMissileAnimator.SetTrigger("Launch");
        _magicMissileAnimator.speed = 1 / launchTime;
        hasLaunched = false;
        MaxLifetime = 6.5f;
    }
    public override void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        if (CurrentLifeTime > launchTime) hasLaunched = true;
        CheckLifetime();
    }
    public override void FixedUpdate()
    {
        if (!despawnAnimationPlaying & hasLaunched) MoveProjectile();
    }
    public override void MoveProjectile()
    {
        rb.MovePosition(transform.position + transform.right * (Mathf.Max(3, ProjectileSpeed * CurrentLifeTime/MaxLifetime)) * Time.deltaTime);
    }
}
