﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HM
{
    public static void RotateTransformToAngle(Transform t, Vector3 vec)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = vec;
        t.rotation = q;
    }
    public static void RotateLocalTransformToAngle(Transform t, Vector3 vec)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = vec;
        t.localRotation = q;
    }

    public static float GetAngle2DBetween(Vector3 from, Vector3 to)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(from.y - to.y, from.x - to.x);
    }
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    public static RaycastHit2D RaycastAtPosition(Vector3 pos, int layerMask = 0)
    {
        //make sure we arent on the same plane as our object we are trying to hit
        pos.z = 1000;
        return Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);
    }
    public static RaycastHit2D RaycastToMouseCursor()
    {
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
    }
    /// <summary>
    /// Find the LayerMask by using LayerMask.GetMask("layermask name here")
    /// </summary>
    /// <param name="layerMask"> </param>
    /// <returns></returns>
    public static RaycastHit2D RaycastToMouseCursor(int layerMask)
    {
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
    }
    /// <summary>
    /// DeepCopy of a list
    /// </summary>
    /// <param name="ListToPullFrom"></param>
    /// <param name="ListToCopyInto"></param>
    public static void CopyListOfRoomPositions(List<RoomPosition> ListToPullFrom, List<RoomPosition> ListToCopyInto)
    {
        ListToCopyInto.Clear();
        foreach (RoomPosition element in ListToPullFrom)
        {
            ListToCopyInto.Add(element);
        }
    }

    public static Vector2 GetLocalVector2DPosition(Transform t)
    {
        return new Vector2(t.localPosition. x, t.localPosition.y);
    }
    public static Vector2 GetWorldVector2DPosition(Transform t)
    {
        return new Vector2(t.position.x, t.position.y);
    }

    public static float ParseStringToFloat(string text)
    {
        if(float.TryParse(text, out float number)) return number;
        return -1; // default return
    }
    public static int ParseStringToInt(string text)
    {
        if(int.TryParse(text, out int number)) return number;
        return -1;
    }

    public static string SecondsToTimeDisplay(float timeInSeconds)
    {
        var ts = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:00}:{1:00}", ts.TotalHours, ts.TotalMinutes);
    }
}
