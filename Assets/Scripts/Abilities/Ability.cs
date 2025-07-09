using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;


public class Ability : ScriptableObject
{
    public static float MAX_INITIAL_USE_DELAY = 10;
    public string abilityName = "";

    public Texture icon;
    
    public virtual float[] InitialUseDelay
    {
        get => initialUseDelay;
        set => initialUseDelay = value;
    }

    [SerializeField]
    [PropertyRange(0, "maxInitialUseDelay")]
    private float[] initialUseDelay = { 0.3f };

    [SerializeField]
    public virtual float maxInitialUseDelay
    {
        get => MAX_INITIAL_USE_DELAY;
    }
    [field:SerializeField] public virtual float successDelay { get; set; }
    public float[] unsuccessDelay = { 0.3f };
    public virtual GameObject prefab { get; set; }
    public enum Dir
    {
        none = 0,
        right = 1,
        left = 2,
        up = 3,
        down = 4,
        pierce = 5
    }
    public Dir dir;
    public enum Mode
    {
        AttackBased = 0,
        BlockBased = 1,
        ComboBased = 2
    }
    public Mode modeBase;
    public enum Archetype
    {
        Singular = 0,
        Multi_Choice = 1,
        Multi_Followup = 2,
    }
    public Archetype archetype;

    public bool hasAdditionalFunctionality = false;

    [ShowIf("hasAdditionalFunctionality")]
    public MonoScript[] afTypes;
    [ShowIf("hasAdditionalFunctionality")]
    [SerializeReference] public AF[] afs;
    public Dictionary<string, AF> AF_Dictionary;

    [Button]
    public void InitializeAFValues()
    {
        afs = new AF[afTypes.Length];
        AF_Dictionary = new Dictionary<string, AF>();
        AF_Dictionary.Clear();
        for (int i = 0; i < afs.Length; i++)
        {
            Type t = afTypes[i].GetClass();
            afs[i] = (AF)Activator.CreateInstance(t);
            AF_Dictionary.Add(afs[i].afname, afs[i]);
        }
    }

}
