using Sirenix.OdinInspector;
using System;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
using static EntityController;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability, IAbilityAnims, IAbilityDirectional
{
    public Type type { get => typeof(AM.AtkAnims); }
    public int Enum { get => (int)Attack; }
    public IAbilityDirectional.Direction Dir { get => dir; }

    [BoxGroup("Attack Ability")] [SerializeField] private IAbilityDirectional.Direction dir;
    [BoxGroup("Attack Ability")] public float damage;
    [BoxGroup("Attack Ability")] public float flinchAmount = 1f;
    [BoxGroup("Attack Ability")] public AM.AtkAnims.Anims Attack;
    [BoxGroup("Attack Ability")] [SerializeField] private GameObject triggerCollider;
    [BoxGroup("Attack Ability")] [SerializeField] private GameObject attackedEffect;

    public override GameObject ColliderPrefab 
    {
        get => triggerCollider;
        set => triggerCollider = value;
    }

    public override void Use(ICombatMode mode, CombatFunctionality cf, RuntimeModeData Mode)
    {
        Ability ability = Mode.ability;
        ModeTriggerGroup trigger = cf.AbilityTriggerEnableUse(Mode);

        Mode.functionality.Starting();
        Mode.SetAbility(ability);
        Mode.trigger = trigger;
        bool hasAf = ability.hasAdditionalFunctionality;

        if (hasAf && ability.hasInitializedAfs == false) ability.InitializeAFValues();

        if(ability.archetype == Ability.Archetype.Singular)
        {
            AbilityAttack ability_Mode = (AbilityAttack)ability;

            Debug.Log($"[{cf.gameObject.name}] ATTACK: Singular");

            //Setup

            //Mutations


            //Trigger
            trigger.Use(ability.InitialUseDelay[0]);

            //Animation
            IAbilityAnims anims = trigger.ability as IAbilityAnims;
            cf.Controls.animController.Play(anims.type, anims.Enum, CharacterAnimationController.UPPERBODY, false, false, ability.InitialUseDelay[0]);
        }
    }


}
