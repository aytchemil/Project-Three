using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string abilityName;

    public Texture icon;
    
    public virtual float[] InitialUseDelay
    {
        get => initialUseDelay;
        set => initialUseDelay = value;
    }

    [SerializeField]
    private float[] initialUseDelay = { 0.3f };
    [field:SerializeField] public virtual float successDelay { get; set; }
    public float[] unsuccessDelay = { 0.3f };
    public float movementAmount;
    public virtual GameObject prefab { get; set; }

    public enum Archetype
    {
        Singular = 0,
        Multi_Choice = 1,
        Multi_Followup = 2,
    }
    public Archetype archetype;
    public enum Trait
    {
        None = 0,
        MovementForward = 4,
        MovementLeftOrRight = 5,
    }
    public Trait trait;

    [SerializeField]
    private string animation_name;

    public virtual string AnimName
    {
        get => animation_name;
        set => animation_name = value;
    }
}
