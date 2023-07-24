using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GS // Stands for "Get Strings"
{
    //  Prefabs
    public static string Prefabs(string filename = "")
    {
        return "Prefabs/" + filename;
    }
    public static string UIPrefabs(string filename = "")
    {
        return Prefabs() + "UI/" + filename;
    }
    public static string Effects(string filename = "")
    {
        return Prefabs() + "Effects/" + filename;
    }

    public static string Potions(string filename = "")
    {
        return Prefabs() + "Potions/" + filename;
    }

    public static string Enemy(string filename = "")
    {
        return Prefabs() + "Enemy/" + filename;
    }

    //  Scriptable Objects
    public static string ScriptableObjects(string filename = "")
    {
        return "ScriptableObjects/" + filename;
    }
    public static string InventoryItems(string filename = "")
    {
        return ScriptableObjects() + "InventoryItems/" + filename;
    }

    //  Graphics

    public static string Props(string filename = "")
    {
        return Graphics() + "Props/" + filename;
    }
    public static string Graphics(string filename = "")
    {
        return "Graphics/" + filename;
    }

    //  Dialogue
    public static string Dialogue(string filename = "")
    {
        return "Dialogue/" + filename;
    }
    public static string Conversations(string filename = "")
    {
        return Dialogue() + "Conversations/" + filename;
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
}
