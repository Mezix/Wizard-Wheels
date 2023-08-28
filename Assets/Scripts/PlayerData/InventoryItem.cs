using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string Name;
    public Sprite Image;
    public int Price;
}