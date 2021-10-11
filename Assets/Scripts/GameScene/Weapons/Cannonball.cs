﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour, IProjectile
{
    public float CurrentLifeTime { get; private set; } //Check IProjectile for explanations
    public float MaxLifetime { get; set; }
    public int Damage { get; set; }

    private Rigidbody2D rb;
    public float ProjectileSpeed { get; set; }
    private bool exploding;
    public bool HitPlayer { get; set; }
    private bool hasDoneDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ProjectileSpeed = 10f;
        MaxLifetime = 3;
    }
    private void OnEnable()
    {
        CurrentLifeTime = 0;
        exploding = false;
        hasDoneDamage = false;
    }
    private void FixedUpdate()
    {
        if(!exploding) MoveProjectile();
    }
    private void MoveProjectile()
    {
        transform.position += transform.right * ProjectileSpeed * Time.deltaTime;
    }
    private void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        CheckLifetime();
    }
    public void CheckLifetime()
    {
        if (CurrentLifeTime >= MaxLifetime)
        {
            DespawnBullet();
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!hasDoneDamage)
        {
            if (col.transform.root.tag == "Enemy" && !HitPlayer)
            {
                DamageEnemy(col.transform.root.GetComponentInChildren<IEnemy>());
                hasDoneDamage = true;
            }
            if (col.transform.root.tag == "Player" && HitPlayer)
            {
                DamagePlayer();
                hasDoneDamage = true;
            }
        }
    }
    private void DamageEnemy(IEnemy e)
    {
        e.TakeDamage(Damage);
        StartCoroutine(PlayExplosion());
    }
    private void DamagePlayer()
    {
        PlayerTankController.instance.TakeDamage(Damage);
        StartCoroutine(PlayExplosion());
    }
    private IEnumerator PlayExplosion()
    {
        exploding = true;
        GameObject explosion = Instantiate((GameObject) Resources.Load("SingleExplosion"));
        explosion.transform.position = transform.position;
        yield return new WaitForSeconds(0.43f);
        Destroy(explosion);
        DespawnBullet();
    }
    public void DespawnBullet()
    {
        ProjectilePool.Instance.AddToPool(gameObject);
    }
    public void SetBulletStatsAndTransform(int gunDamage, Vector3 pos, Quaternion rot)
    {
        Damage = gunDamage;
        transform.position = pos;
        transform.rotation = rot;
    }
}