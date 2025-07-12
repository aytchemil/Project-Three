using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
public interface IAbilityAnims
{
    public abstract System.Type type { get; }
    public abstract int Enum { get; }
}
public interface IAbilityDirectional
{
    public enum Direction
    {
        none = 0,
        right = 1,
        left = 2,
        up = 3,
        down = 4,
        pierce = 5
    }
    public Direction Dir { get; }
}


public class Ability : ScriptableObject
{
    public static float MAX_INITIAL_USE_DELAY = 10;

    [BoxGroup("Ability")] public string abilityName = "";
    [BoxGroup("Ability")] public Texture icon;

    [BoxGroup("Delays")] [SerializeField]  [PropertyRange(0, "maxInitialUseDelay")]
    private float[] initialUseDelay = { 0.3f };
    [BoxGroup("Delays")] public float[] successDelay = { 0f };
    [BoxGroup("Delays")] public float[] unsuccessDelay = { 0.3f };
    public virtual GameObject prefab { get; set; }

    [BoxGroup("Enums")]
    public Mode modeBase;
    [BoxGroup("Enums")]
    public Archetype archetype;
    public enum Archetype
    {
        Singular = 0,
        Multi_Choice = 1,
        Multi_Followup = 2,
    }
    public enum Mode
    {
        AttackBased = 0,
        BlockBased = 1,
        ComboBased = 2
    }
    public virtual float[] InitialUseDelay
    {
        get => initialUseDelay;
        set => initialUseDelay = value;
    }
    [SerializeField]
    public virtual float maxInitialUseDelay
    {
        get => MAX_INITIAL_USE_DELAY;
    }

    [BoxGroup("Additional Functionality (AF)")] public bool hasAdditionalFunctionality = false;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    public MonoScript[] afTypes;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    [SerializeReference] public AF[] afs;
    public Dictionary<string, AF> AF_Dictionary;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    public bool hasInitializedAfs = false;


    [Button]
    public void InitializeAFValues()
    {
        if (afs == null || afs.Length != afTypes.Length)
            afs = new AF[afTypes.Length];

        AF_Dictionary = new Dictionary<string, AF>();
        AF_Dictionary.Clear();

        for (int i = 0; i < afs.Length; i++)
        {
            if(afs[i] == null)
            {
                Type t = afTypes[i].GetClass();
                afs[i] = (AF)Activator.CreateInstance(t);
            }
            AF_Dictionary.Add(afs[i].afname, afs[i]);
        }

        hasInitializedAfs = true;
    }

}
