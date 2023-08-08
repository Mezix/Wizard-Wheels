using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class Loader {
    public enum Scene { CombatScene, MenuScene, EventScene, ConstructionScene, RouteTransitionScene }
    public static void Load(Scene scene)
    {
        if (scene.ToString() == "GameScene")
        {
        }
        if (scene.ToString() == "MenuScene")
        {
        }
        if (scene.ToString() == "EventScene")
        {
        }
        if (scene.ToString() == "RouteTransitionScene")
        {
        }
        if (scene.ToString() == "ConstructionScene")
        {
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(scene.ToString());
    }

    public static Scene EventNodeToScene(PlayerData.NodeEventType nodeToTransform)
    {
        if (nodeToTransform.Equals(PlayerData.NodeEventType.Combat)) return Scene.CombatScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Construction)) return Scene.ConstructionScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Shop)) return Scene.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Dialogue)) return Scene.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.FreeLoot)) return Scene.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.NewWizard)) return Scene.EventScene;

        else return Scene.CombatScene;
    }
}
