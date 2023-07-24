using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyEvent")]
public class EnemyEvent : ScriptableObject
{
    public List<EnemySpawn> EnemyWaves;
    [System.Serializable]
    public struct EnemySpawn
    {
        public float _delay;
        public TankStats _tankStats;
        public TankRoomConstellation _tankRoomConstellation;
    }
}
