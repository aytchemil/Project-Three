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

    [SerializeField]
    public virtual float maxInitialUseDelay
    {
        get => MAX_INITIAL_USE_DELAY;
    }

    [BoxGroup("Ability")] public string abilityName = "";
    [BoxGroup("Ability")] public Sprite icon;
    [BoxGroup("Ability")] public bool hasAdditionalFunctionality = false;
    [BoxGroup("Ability")] [ShowIf("canHaveAfs")][ShowIf("hasAdditionalFunctionality")][SerializeReference] public List<AbilityEffect> effects;

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


    public virtual GameObject ColliderPrefab { get; set; }



    void OnEnable()
    {
        if (string.IsNullOrEmpty(abilityName)) abilityName = name;
        if (effects == null) effects = new List<AbilityEffect>();
    }


    public abstract void Use(ICombatMode mode, CombatFunctionality cf, RuntimeModeData Mode);


}





