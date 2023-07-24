using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REF : MonoBehaviour
{
    //  MAIN MENU

    public static MainMenuSceneManager mMenu;

    public static MainMenuUI mUI;

    public static MainMenuCamera mCam;


    //  GAME SCENE

    /// <summary>
    /// The GameObject attached to the Player Tank Controller Script
    /// </summary>
    public static GameObject PlayerGO;
    /// <summary>
    /// The Player Tank Controller script
    /// </summary>
    public static PlayerTankController PCon;
    /// <summary>
    /// Wether or not the player is dead
    /// </summary>
    public static bool PDead;

    public static CameraScript Cam;
    public static UIScript UI;
    public static DialogueManager Dialog;
    public static SpeedDisplay SD;
    public static TimeManager TM;
    public static MouseCursor mouse;
    public static CrosshairManager c;

    public static UnitPathfinding Path;

    public static EnemyManager EM;

    public static MapGeneration MapGen;

    public static MainMenuSceneTankPreview TankPreview;
    public static UpgradeScreen UpgrScreen;
    public static InventoryUI InvUI;
}
