using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : MonoBehaviour, ISystem
{
    public void StartInteraction()
    {
        print("Engine go brrr hahahaha :D");
    }

    public void StopInteraction()
    {
        print("Engine stop go brrr");
    }
}
