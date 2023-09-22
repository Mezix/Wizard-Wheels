using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public PoolableType _poolableType;

    public enum PoolableType
    {
        Cannonshell,
        Howitzer,
        MagicMissile,
        MagicExplosion,
        RegularExplosion,
        DeathExplosion
    }
}
