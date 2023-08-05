using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class Loader {
    public enum Scene { CombatScene, MenuScene, EventScene, ConstructionScene }

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
        if (scene.ToString() == "ConstructionScene")
        {
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(scene.ToString());
    }
}
