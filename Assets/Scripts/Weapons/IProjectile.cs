using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile //the interface for all projectiles fired from ranged Weapons
{
    float CurrentLifeTime { get; } //the timer that ticks up in accordance to how long the projectile has been alive so far
    float MaxLifetime { get; set; } //the lifetime our projectile has before it despawns
    int Damage { get; set; } //damage of each bullet, determined by the weapon that fires it
    void CheckLifetime(); //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    void DespawnBullet(); //a function to remove the bullet from the game/add it to the bullet pool
    void SetBulletStatsAndTransform(int weaponDamage, Vector3 pos, Quaternion rot);
}
