using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : ISystem
{
    private void Awake()
    {
        SystemObj = gameObject;
    }
    public override void InitSystemStats()
    {
    }
    public override void StartInteraction()
    {
        IsBeingInteractedWith = true;
        //print("Engine go brrr hahahaha :D");
    }

    public override void StopInteraction()
    {
        IsBeingInteractedWith = false;
        print("Engine stop go brrr");
    }

    
}
