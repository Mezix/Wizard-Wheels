using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour, IProjectile
{
    public float CurrentLifeTime { get; private set; } //Check IProjectile for explanations
    public float MaxLifetime { get; set; }
    public float Damage { get; set; }

    private Rigidbody2D rb;
    private float projectileSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileSpeed = 10f;
        MaxLifetime = 3;
    }
    private void OnEnable()
    {
        CurrentLifeTime = 0;
    }
    private void FixedUpdate()
    {
        transform.position += transform.right * projectileSpeed * Time.deltaTime;
    }
    private void Update()
    {
        CheckLifetime();
        CurrentLifeTime += Time.deltaTime;
    }
    public void CheckLifetime()
    {
        if (CurrentLifeTime >= MaxLifetime)
        {
            DespawnBullet();
        }
    }
    public void DespawnBullet()
    {
        ProjectilePool.Instance.AddToPool(gameObject);
    }
    public void SetBulletStatsAndTransform(float gunDamage, Vector3 pos, Quaternion rot)
    {
        Damage = gunDamage;
        transform.position = pos;
        transform.rotation = rot;
    }
}
