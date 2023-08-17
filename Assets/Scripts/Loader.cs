using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class Loader {
    public enum SceneType { CombatScene, MenuScene, EventScene, ConstructionScene, RouteTransitionScene, VictoryScene }

    public static SceneType SceneToSceneType(Scene scene)
    {
        if (scene.name == "CombatScene")
        {
            return SceneType.CombatScene;
        }
        else if (scene.name == "MenuScene")
        {
            return SceneType.MenuScene;
        }
        else if (scene.name == "EventScene")
        {
            return SceneType.EventScene;
        }
        else if (scene.name == "RouteTransitionScene")
        {
            return SceneType.RouteTransitionScene;
        }
        else if (scene.name == "ConstructionScene")
        {
            return SceneType.ConstructionScene;
        }
        return SceneType.CombatScene;
    }

    public static void Load(SceneType scene)
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        AudioManager.Singleton.PlayMusic(scene);
        SceneManager.LoadScene(scene.ToString());
    }

    public static SceneType EventNodeToScene(PlayerData.NodeEventType nodeToTransform)
    {
        if (nodeToTransform.Equals(PlayerData.NodeEventType.Combat)) return SceneType.CombatScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Construction)) return SceneType.ConstructionScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Shop)) return SceneType.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.Dialogue)) return SceneType.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.FreeLoot)) return SceneType.EventScene;
        else if (nodeToTransform.Equals(PlayerData.NodeEventType.FreeWizard)) return SceneType.EventScene;

        else return SceneType.CombatScene;
    }
}
