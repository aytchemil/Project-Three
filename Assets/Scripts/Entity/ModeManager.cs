using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class ModeManager
{
    public static ModeManager inst;
    public enum Modes
    { 
        Attack = 0,
        Counter = 1,
        Block = 2,
        Combo = 3,
        Evade = 4,
        Defend = 5,
    }

    public List<ModeData> modes = new List<ModeData>();

    public ModeManager(List<ModeData> modes)
    {
        this.modes = modes;
        inst = this;
    }

    private static GameObject debugTextObj;
    private static TextMesh debugTextMesh;

    private static void DebugStepMarker(int step)
    {
        // Create it only once
        if (debugTextObj == null)
        {
            debugTextObj = new GameObject("DebugStepText");
            debugTextMesh = debugTextObj.AddComponent<TextMesh>();

            // Transform
            debugTextObj.transform.position = new Vector3(25f, 22f, -25f);
            debugTextObj.transform.rotation = Quaternion.identity;
            debugTextObj.transform.localScale = new Vector3(6f, 6f, 6f);

            // Style
            debugTextMesh.characterSize = 0.5f;
            debugTextMesh.fontStyle = FontStyle.Bold;
            debugTextMesh.anchor = TextAnchor.MiddleCenter;
            debugTextMesh.alignment = TextAlignment.Center;
            debugTextMesh.color = Color.cyan;
        }

        // Just update the text
        debugTextMesh.text = $"STEP {step}";
    }

    public static int FindFirstIndex<T>(List<T> list, T value, int startIndex = 0)
    {
        if (list == null || startIndex < 0 || startIndex >= list.Count)
            return -1;

        for (int i = startIndex; i < list.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], value))
                return i;
        }
        return -1;
    }
}
