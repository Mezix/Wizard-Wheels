using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string _weaponName = "";
    public Sprite _UISprite;
    public int _damage;
    public float _projectileSpeed;
    public float _attacksPerSecond;
    public float _rotationSpeed;
    public float _lockOnRange;
}
