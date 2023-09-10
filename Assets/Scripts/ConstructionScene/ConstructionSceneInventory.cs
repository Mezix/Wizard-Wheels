using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class ConstructionSceneInventory : MonoBehaviour
{
    public Dictionary<ConstructionSceneComponent, int> componentsInvIndexDict = new Dictionary<ConstructionSceneComponent, int>();
    public VerticalLayoutGroup _vLayoutGroup;
    private ConstructionSceneComponent constructionSceneComponentPrefab;
    private void Awake()
    {
        constructionSceneComponentPrefab = Resources.Load(GS.DataScenePrefabs("ConstructionSceneComponent"), typeof (ConstructionSceneComponent)) as ConstructionSceneComponent;
    }
    public void InitInventory(List<InventoryItemData> inventoryItems)
    {
        int index = 0;
        foreach(InventoryItemData data in inventoryItems)
        {
            if(index == MetalBarIndex || index == GemIndex || index == PlankIndex)
            {
                ConstructionSceneComponent tmp = Instantiate(constructionSceneComponentPrefab, _vLayoutGroup.transform);
                tmp._icon.sprite = Resources.Load(data.SpritePath, typeof (Sprite)) as Sprite;
                tmp._objectName = data.Name + ": ";
                tmp.SetAmount(data.Amount);

                componentsInvIndexDict.Add(tmp, index);
            }
            index++;
        }
    }
}
