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
        else if (ability.archetype == Ability.Archetype.Multi_Choice)
        {

            Debug.Log($"[{cf.gameObject.name}] ATTACK: MLTI-CHOICE");

            //Setup
            string choice = GetMultiChoiceAttack(ability); if (choice == "none") return;
            MAT_ChoiceGroup mltiTrigger = trigger.GetComponent<MAT_ChoiceGroup>();

            //Trigger 
            mltiTrigger.Use(ability, ability.InitialUseDelay[0], out ModeTriggerGroup _chosenChildTrigger, choice);

            //Animation
            IAbilityAnims anims = _chosenChildTrigger.ability as IAbilityAnims;
            cf.Controls.animController.Play(anims.type, anims.Enum, CharacterAnimationController.UPPERBODY, false, false);
        }
        else if (ability.archetype == Ability.Archetype.Multi_Followup)
        {
            Debug.Log($"[{cf.gameObject.name}] ATTACK: Multi-Followup");

            AbilityMulti multi_Mode = (AbilityMulti)ability;

            //Functionality
            trigger.GetComponent<MAT_FollowupGroup>().Use(multi_Mode.abilities[0].InitialUseDelay[0]);


            //Animation
            AM.FollowUpPackage FollowUpPackage = new AM.FollowUpPackage(
                trigger,
                Mode,
                cf.GetAnimEnums(multi_Mode),
                typeof(AM.AtkAnims),
                typeof(AM.AtkAnims.Anims),
                CharacterAnimationController.UPPERBODY,
                false,
                false,
                0.2f,
                multi_Mode.InitialUseDelay
                );
            cf.StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));


        }




        string GetMultiChoiceAttack(Ability _ability)
        {
            string choice = "";
            AbilityMultiChoice ability = (_ability as AbilityMultiChoice);


            switch (ability.archetype)
            {
                case Ability.Archetype.Multi_Choice:

                    choice = cf.Controls.getMoveDirection?.Invoke();

                    for (int i = 0; i < ability.choices.Length; i++)
                        if (ability.choices[i] == choice)
                            return choice;

                    choice = "none";
                    Mode.functionality.Finish();
                    break;
            }

            return choice;
        }
    }


}
