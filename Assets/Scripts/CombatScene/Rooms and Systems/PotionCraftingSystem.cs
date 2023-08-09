using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCraftingSystem : ASystem
{
    private PotionCraftingUI potionUI;
    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        SpawnPotionCraftingUI();
    }
    private void SpawnPotionCraftingUI()
    {
        if (!REF.CombatUI) return;
        potionUI = Instantiate(Resources.Load(GS.Potions("PotionCraftingUI"), typeof(PotionCraftingUI)) as PotionCraftingUI);
        potionUI.transform.SetParent(REF.CombatUI._systems.transform, false);
    }
    public override void InitSystemStats()
    {

    }
    public override void StartInteraction()
    {
        IsBeingInteractedWith = true;
    }
    public override void StopInteraction()
    {
        IsBeingInteractedWith = false;
    }
}
