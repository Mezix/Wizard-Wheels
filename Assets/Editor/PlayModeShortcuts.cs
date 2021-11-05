using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class PlayModeShortcuts : MonoBehaviour
{
    [InitializeOnLoad]
    public static class EnterPlayModeBindings
    {
        static EnterPlayModeBindings()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
            EditorApplication.quitting += Quitting;
        }

        static void ModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredPlayMode)
                ShortcutManager.instance.activeProfileId = "RuntimeShortcuts";
            else if (playModeState == PlayModeStateChange.EnteredEditMode)
                ShortcutManager.instance.activeProfileId = "Default";
        }

        static void Quitting()
        {
            ShortcutManager.instance.activeProfileId = "Default";
        }
    }
}
