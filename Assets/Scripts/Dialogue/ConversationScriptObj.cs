﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public DialogueCharacterScriptObj characterLeft;
    public DialogueCharacterScriptObj characterRight;

    [TextArea(2, 5)]
    public string textLeft;
    [TextArea(2, 5)]
    public string textRight;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
public class ConversationScriptObj : ScriptableObject
{
    public Line[] lines;
}
