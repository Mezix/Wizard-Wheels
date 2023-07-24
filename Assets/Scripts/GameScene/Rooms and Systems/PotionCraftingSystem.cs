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
        GameObject g = Instantiate((GameObject) Resources.Load(GS.Potions("PotionCraftingUI")));
        g.transform.SetParent(REF.UI._systems.transform, false);
        potionUI = g.GetComponent<PotionCraftingUI>();
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
