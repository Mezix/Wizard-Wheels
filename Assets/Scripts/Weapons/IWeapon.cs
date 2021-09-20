using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon //the interface used by the player to attack, since we have melee and ranged weapons we want an even more generic weapon interface
{
    //  the generic attack function used by the player:
    //
    //  the other interfaces that inherit from IWeapon, IGun and IMelee, dont need this function explicitly, 
    //  it is passed on to all actual implementations and does not need to be implemented 
    //  by the interfaces themselves

    bool weaponSelected { get; set; }
    void Aim();
    void PointTurretAtTarget();
    void Attack();

    float Damage { get; set; } //all weapons need damage, so this is defined
    float AttacksPerSecond { get; set; }  //a constant that determines our firerate, mostly used for display purposes for the player
    float TimeBetweenAttacks { get; } //derived from RoundsPerSecond, used to determine when the next bullet is allowed to be fired
    float TimeElapsedBetweenLastAttack { get; } //a timer that continously ticks up so we can check with TimeBetweenShots to see if we can fire again
}
