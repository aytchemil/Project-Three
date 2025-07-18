using System;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager
{
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
    }

    public Type ReturnModeFunctionality(string mode)
    {
        for (int i = 0; i < modes.Count; i++)
        {
            if (modes[i].name == mode)
                return modes[i].GetModeType();
        }

        Debug.LogError($"[ModeManager] No mode found with the name of [{mode}], please check input");
        return null;
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
