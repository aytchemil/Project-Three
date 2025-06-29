using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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
    public CombatAdditionalFunctionalities.Function af;


    private bool isMovement => 
        ((af == CombatAdditionalFunctionalities.Function.MovementForward) ||
        (af == CombatAdditionalFunctionalities.Function.MovementLeftOrRight)) &&
        hasAdditionalFunctionality
        ;
    [ShowIf("isMovement")]
    public float movementAmount;

    [SerializeField]
    private string animation_name;

    public virtual string AnimName
    {
        get => animation_name;
        set => animation_name = value;
    }
}
