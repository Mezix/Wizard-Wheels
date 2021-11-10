using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AProjectile : MonoBehaviour //the interface for all projectiles fired from ranged Weapons
{
    public float CurrentLifeTime { get; protected set; } //Check IProjectile for explanations
    public float MaxLifetime { get; set; }
    public int Damage { get; set; }
    public float ProjectileSpeed { get; set; }
    public bool HitPlayer { get; set; }

    protected Rigidbody2D rb;
    [SerializeField]
    protected SpriteRenderer _projectileSprite;
    protected bool despawnAnimationPlaying;
    protected bool hasDoneDamage;

    //  Shadow
    [SerializeField]
    protected GameObject _shadow;
    [SerializeField]
    protected Transform _shadowRotation;
    [SerializeField]
    protected float maxShadowHeight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ProjectileSpeed = 10f;
        MaxLifetime = 3;
    }
    private void OnEnable()
    {
        CurrentLifeTime = 0;
        despawnAnimationPlaying = false;
        hasDoneDamage = false;
        _shadow.transform.localPosition = new Vector2(0, -maxShadowHeight);
        _shadow.SetActive(true);
        _projectileSprite.gameObject.SetActive(true);
    }
    private void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        CheckLifetime();
    }
    private void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
        UpdateShadowPosition();
    }
    protected void MoveProjectile()
    {
        rb.MovePosition(transform.position + transform.right * ProjectileSpeed * Time.deltaTime);
        UpdateShadowPosition();
    }
    protected void UpdateShadowPosition()
    {
        HM.RotateLocalTransformToAngle(_shadowRotation, -transform.rotation.eulerAngles);
        _shadow.transform.localPosition = new Vector2(0, -maxShadowHeight * (1f - CurrentLifeTime / MaxLifetime));
    }
    protected void CheckLifetime() //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    {
        if (CurrentLifeTime >= MaxLifetime && !despawnAnimationPlaying)
        {
            StartCoroutine(DespawnAnimation());
        }
    }
    protected void DespawnBullet()
    {
        ProjectilePool.Instance.AddToPool(gameObject);
    }
    public void SetBulletStatsAndTransform(int gunDamage, Vector3 pos, Quaternion rot)
    {
        Damage = gunDamage;
        transform.position = pos;
        transform.rotation = rot;
    }
    public virtual void DamageEnemy(IEnemy e)
    {
        e.TakeDamage(Damage);
        StartCoroutine(DespawnAnimation());
    }
    public virtual void DamagePlayer()
    {
        PlayerTankController.instance.TakeDamage(Damage);
        StartCoroutine(DespawnAnimation());
    }
    public virtual IEnumerator DespawnAnimation()
    {
        despawnAnimationPlaying = true;
        _shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        yield return new WaitForSeconds(0f);
        DespawnBullet();
    }
}
