using System;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public static ModeManager instance;

    public enum Modes
    { 
        Attack = 0,
        Counter = 1,
        Block = 2,
        Combo = 3,
        Evade = 4,
        Defend = 5,
    }

    public ModeFunctionalityReferenceSO[] ModeFunctionalities;

    public List<ModeTemplate> modes = new List<ModeTemplate>();
    private void Awake()
    {
        //print("initializing mode manager");
        instance = this;
    }

    public Type ReturnModeFunctionality(string mode)
    {
        for (int i = 0; i < ModeFunctionalities.Length; i++)
        {
            if (ModeFunctionalities[i].ModeName == mode)
                return ModeFunctionalities[i].GetModeType();
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
