using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GS // Stands for "Get Strings"
{
    //  PREFABS

    public static string Prefabs(string filename = "")
    {
        return "Prefabs/" + filename;
    }
    public static string UIPrefabs(string filename = "")
    {
        return Prefabs() + "UI/" + filename;
    }
    public static string DataScenePrefabs(string filename = "")
    {
        return Prefabs() + "Data Manipulation Scene/" + filename;
    }
    public static string WeaponPrefabs(string filename = "")
    {
        return Prefabs() + "Weapons/" + filename;
    }
    public static string Wizards(string filename = "")
    {
        return Prefabs() + "Wizards/" + filename;
    }
    public static string Effects(string filename = "")
    {
        return Prefabs() + "Effects/" + filename;
    }
    public static string RoomPrefabs(string filename = "")
    {
        return Prefabs() + "Rooms/" + filename;
    }
    public static string WallPrefabs(string filename = "")
    {
        return RoomPrefabs() + "Walls/" + filename;
    }

    public static string Potions(string filename = "")
    {
        return Prefabs() + "Potions/" + filename;
    }

    public static string Enemy(string filename = "")
    {
        return Prefabs() + "Enemy/" + filename;
    }

    //  GRAPHICS

    public static string Cursors(string filename = "")
    {
        return UIGraphics() + "Cursors/" + filename;
    }
    public static string UIGraphics(string filename = "")
    {
        return Graphics() + "UI/" + filename;
    }
    public static string InventoryGraphics(string filename = "")
    {
        return UIGraphics() + "Inventory/" + filename;
    }
    public static string WeaponGraphics(string filename = "")
    {
        return Graphics() + "Weapons/" + filename;
    }
    public static string WizardGraphics(string filename = "")
    {
        return Graphics() + "Weapons/" + filename;
    }
    public static string TilemapArtwork(string filename = "")
    {
        return Graphics() + "Tilemap Artwork/" + filename;
    }
    public static string Graphics(string filename = "")
    {
        return "Graphics/" + filename;
    }

    //  DIALOGUE

    public static string Dialogue(string filename = "")
    {
        return "Dialogue/" + filename;
    }
    public static string Conversations(string filename = "")
    {
        return Dialogue() + "Conversations/" + filename;
    }
    public static string DialogueCharacters(string filename = "")
    {
        return Dialogue() + "Characters/" + filename;
    }

    // Tiles

    public static string Tiles(string filename = "")
    {
        return "Tiles/" + filename;
    }
    public static string BGTiles(string filename = "")
    {
        return Tiles() + "BG Tiles/" + filename;
    }


    //  SCRIPTABLE OBJECTS

    public static string ScriptableObjects(string filename = "")
    {
        return "ScriptableObjects/" + filename;
    }
    public static string InventoryItems(string filename = "")
    {
        return ScriptableObjects() + "InventoryItems/" + filename;
    }

}
