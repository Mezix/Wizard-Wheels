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
        public VehicleStats _tankStats;
        public VehicleConstellation _tankRoomConstellation;
    }
}
