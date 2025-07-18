using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using static EntityController;
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


public abstract class Ability : ScriptableObject
{
    public static float MAX_INITIAL_USE_DELAY = 10;
    bool isSinglar { get => (archetype == Archetype.Singular); }


    [BoxGroup("Ability")] public string abilityName = "";
    [BoxGroup("Ability")] public Texture icon;
    [BoxGroup("Ability")][ShowIf("isSinglar")][SerializeReference] public List<AbilityEffect> effects;
    [BoxGroup("Ability")] public bool isStance = false;

    [BoxGroup("Delays")][SerializeField][PropertyRange(0, "maxInitialUseDelay")] private float[] initialUseDelay = { 0.3f };
    [BoxGroup("Delays")] public float[] successDelay = { 0f };
    [BoxGroup("Delays")] public float[] unsuccessDelay = { 0.3f };

    [BoxGroup("Enums")] public Mode modeBase;
    [BoxGroup("Enums")] public Archetype archetype;
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
    [SerializeField] public virtual float maxInitialUseDelay
    {
        get => MAX_INITIAL_USE_DELAY;
    }

    public virtual GameObject ColliderPrefab { get; set; }

    [BoxGroup("Additional Functionality (AF)")] public bool hasAdditionalFunctionality = false;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    public TypeVariable[] afTypes;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    [SerializeReference] public AF[] afs;
    public Dictionary<string, AF> AF_Dictionary;
    [BoxGroup("Additional Functionality (AF)")] [ShowIf("hasAdditionalFunctionality")]
    public bool hasInitializedAfs = false;


    void OnEnable()
    {
        if (string.IsNullOrEmpty(abilityName)) abilityName = name;
        if (effects == null) effects = new List<AbilityEffect>();
    }

    [Button]
    public void InitializeAFValues()
    {
        if (afs == null || afs.Length != afTypes.Length)
            afs = new AF[afTypes.Length];

        AF_Dictionary = new Dictionary<string, AF>();
        AF_Dictionary.Clear();

        for (int i = 0; i < afs.Length; i++)
        {
            if (afs[i] == null)
            {
                Type t = afTypes[i].Value;
                afs[i] = (AF)Activator.CreateInstance(t);
            }
            AF_Dictionary.Add(afs[i].afname, afs[i]);
        }

        hasInitializedAfs = true;
    }


    public abstract void Use(ICombatMode mode, CombatFunctionality cf, RuntimeModeData Mode);


}





