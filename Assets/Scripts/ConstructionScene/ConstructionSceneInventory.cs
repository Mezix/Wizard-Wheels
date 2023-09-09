using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class ConstructionSceneInventory : MonoBehaviour
{
    public List<ConstructionSceneComponent> components;
    public VerticalLayoutGroup _vLayoutGroup;
    private ConstructionSceneComponent _prefab;
    private void Awake()
    {
        _prefab = Resources.Load(GS.DataScenePrefabs("ConstructionSceneComponent"), typeof (ConstructionSceneComponent)) as ConstructionSceneComponent;
    }

    public void InitComponents(List<InventoryItemData> inventoryItems)
    {

    }
}
