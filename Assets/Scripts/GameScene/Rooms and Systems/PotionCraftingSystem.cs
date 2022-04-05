using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCraftingSystem : ASystem
{
    private PotionCraftingUI potionUI;
    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        SpawnPotionCraftingUI();
    }
    private void SpawnPotionCraftingUI()
    {
        GameObject g = Instantiate((GameObject) Resources.Load("Potions/PotionCraftingUI"));
        g.transform.SetParent(Ref.UI._systems.transform, false);
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
