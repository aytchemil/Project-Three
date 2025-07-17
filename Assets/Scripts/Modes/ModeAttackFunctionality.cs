
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ModeAttackFunctionality : MonoBehaviour, ICombatMode
{
    private CombatFunctionality cf;

    public string MODE { get => "Attack"; }


    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    private void OnEnable()
    {
        cf.Controls.MyAttackWasBlocked += AttackBlocked;
    }

    private void OnDisable()
    {
        cf.Controls.MyAttackWasBlocked -= AttackBlocked;
    }

    public void UseModeFunctionality() => Attack();
    
    /// <summary>
    /// Attack Functionality
    /// </summary>
    void Attack()
    {
        print($"[{gameObject.name}] ATTACK STARTED...");


        //Setup
        EntityController.RuntimeModeData attack = cf.Controls.Mode(MODE);
        Ability ability = attack.ability;
        ModeTriggerGroup trigger = cf.AbilityTriggerEnableUse(MODE);
        bool hasAf = ability.hasAdditionalFunctionality;

        //Flags
        StartAttacking();

        //Initial Mutations
        // + SETS the curr ability
        // + SETS the curr Trigger
        attack.SetAbility(ability);
        attack.trigger = trigger;

        if(hasAf && ability.hasInitializedAfs == false) ability.InitializeAFValues();

        if(ability.archetype == Ability.Archetype.Singular)
        {
            AbilityAttack ability_attack = (AbilityAttack)ability;

            print($"[{gameObject.name}] ATTACK: Singular");

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

            print($"[{gameObject.name}] ATTACK: MLTI-CHOICE");

            //Setup
            string choice = GetMultiChoiceAttack(ability); if (choice == "none") return;
            MAT_ChoiceGroup mltiTrigger = trigger.GetComponent<MAT_ChoiceGroup>();

            //Trigger 
            mltiTrigger.Use(ability, ability.InitialUseDelay[0], out ModeTriggerGroup _chosenChildTrigger, choice);

            //Animation
            IAbilityAnims anims = _chosenChildTrigger.ability as IAbilityAnims;
            cf.Controls.animController.Play(anims.type, anims.Enum, CharacterAnimationController.UPPERBODY, false, false);
        }
        else if(ability.archetype == Ability.Archetype.Multi_Followup)
        {
            print($"[{gameObject.name}] ATTACK: Multi-Followup");

            AbilityMulti multi_attack = (AbilityMulti)ability;

            //Functionality
            trigger.GetComponent<MAT_FollowupGroup>().Use(multi_attack.abilities[0].InitialUseDelay[0]);


            //Animation
            AM.FollowUpPackage FollowUpPackage = new AM.FollowUpPackage(
                trigger,
                attack,
                cf.GetAnimEnums(multi_attack),
                typeof(AM.AtkAnims),
                typeof(AM.AtkAnims.Anims),
                CharacterAnimationController.UPPERBODY,
                false,
                false,
                0.2f,
                multi_attack.InitialUseDelay
                );
            StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));


        }

    }

    #region Flags

    /// <summary>
    /// Sets Control's alreadyAttacking flag to TRUE
    /// </summary>
    public void StartAttacking()
    {
        cf.Controls.Mode(MODE).isUsing = true;
    }

    /// <summary>
    /// Sets Control's alreadyAttacking flag to FALSE
    /// </summary>
    public void FinishAttacking()
    {
        cf.Controls.Mode(MODE).isUsing = false;
        cf.Controls.didReattack = false;
    }
    #endregion



    #region Archetypes


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
                FinishAttacking();
                break;
        }

        return choice;
    }





    /// <summary>
    /// LISTENER for attack being blocked (Controls.MyAttackWasBlocked)
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="ability"></param>
    public void AttackBlocked(string myLookdir, Ability ability)
    {
        print("didreattack: on ModeAttackFunc, checking if the ability used is an attack ability");
        if (ability.modeBase != Ability.Mode.AttackBased) return;
        print($"+YES:[{gameObject.name}] didreattack : Attack was blocked");

        if (ability.archetype == Ability.Archetype.Multi_Followup)
        {
            print("+didreattack: AbilityMulti blocked");
            MAT_FollowupGroup trigger = cf.Controls.Mode(MODE).trigger.GetComponent<MAT_FollowupGroup>();
            if(trigger.IncrementTriggerProgress() == true)
            {
                print("didreattack: final trigger prog");
                trigger.DisableThisTrigger();
            }
        }

        if (ability.archetype == Ability.Archetype.Multi_Choice)
        {
            print("+didreattack: Ability_MultiChoice blocked");
            MAT_ChoiceGroup trigger = cf.Controls.Mode(MODE).trigger.GetComponent<MAT_ChoiceGroup>();

            trigger.DisableThisTrigger();
        }
    }

}

#endregion