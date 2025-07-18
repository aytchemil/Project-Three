using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mode", menuName = "ScriptableObjects/Mode")]
public class ModeData : ScriptableObject
{
    public string name;
    public AbilitySet abilitySet;
    public bool isStance;
    [ShowIf("isStance")]
    public bool abilityIndividualSelection;
    public enum Modes
    {
        Attack = 0,
        Block = 1,
        Combo = 2,
        Counter = 3
    }

    [SerializeField] public Modes mode;
    public Texture UIIndicator;
    public string modeTextDesc;
    private string modeTypeName; // Store the type name as a string for runtime

}

public static class ModesRegistery
{
    public static List<Type> modes;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RegisterModes()
    {
        modes = new List<Type>();
        modes.Add(typeof(AttackMode));
        modes.Add(typeof(BlockMode));
        modes.Add(typeof(ComboMode));
        modes.Add(typeof(CounterMode));

    }

}

