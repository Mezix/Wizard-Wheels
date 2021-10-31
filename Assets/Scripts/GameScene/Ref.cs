using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
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
    /// <summary>
    /// The Camera Script
    /// </summary>
    public static CameraScript Cam;
    /// <summary>
    /// The UI Script
    /// </summary>
    public static UIScript UI;
    /// <summary>
    /// timeManager
    /// </summary>
    public static TimeManager TM;
    /// <summary>
    /// The MouseCursorScript
    /// </summary>
    public static MouseCursor mouse;
    /// <summary>
    /// The CrosshairManager Script
    /// </summary>
    public static CrosshairManager c;

    public static UnitPathfinding Path;
}
