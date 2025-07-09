using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;



public class ModeAttackFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE_NAME { get => "Attack"; }


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

    public override void UseModeFunctionality() => Attack();
    
    /// <summary>
    /// Attack Functionality
    /// </summary>
    void Attack()
    {
        print($"[{gameObject.name}] ATTACK STARTED...");


        //Setup
        CombatEntityController.CombatEntityModeData attack = cf.Controls.Mode("Attack");
        Ability ability = attack.ability;
        ModeTriggerGroup usingTrigger = cf.AbilityTriggerEnableUse("Attack");
        bool hasAf = ability.hasAdditionalFunctionality;

        //Flags
        StartAttacking();

        //Initial Mutations
        // + SETS the curr ability
        // + SETS the curr Trigger
        attack.SetAbility(ability);
        attack.trigger = usingTrigger;

        if(hasAf)
            ability.InitializeAFValues();

        //Mutations
        switch (ability.archetype)
        {
            case AbilityAttack.Archetype.Singular:
                AbilityAttack ability_attack = (AbilityAttack)ability;

                print($"[{gameObject.name}] ATTACK: Singular");

                //Setup

                //Mutations

                //Additional Functionality 
                if (hasAf)
                    AF_Attack(ability);

                //Trigger
                usingTrigger.StartabilityTrigger(ability, ability.InitialUseDelay[0]);

                //Animation
                cf.Controls.animController.Play(typeof(AM.AtkAnims), (int)ability_attack.Attacks, CharacterAnimationController.UPPERBODY, false, false);

                break;
            case AbilityAttack.Archetype.Multi_Choice:

                print($"[{gameObject.name}] ATTACK: MLTI-CHOICE");

                //Setup
                string choice = GetMultiChoiceAttack(ability); if (choice == "none") break;
                MAT_ChoiceGroup parentTrigger = usingTrigger.GetComponent<MAT_ChoiceGroup>();

                //Additional Functionality
                if (hasAf)
                {
                    AF_Attack(ability);
                    AF_Choice(ability, choice);
                }


                //Trigger 
                StartCoroutine(parentTrigger.MultiChoiceAttack(ability, ability.InitialUseDelay[0], choice));

                //Animation
                //AnimateAblity(ability.abilities[0].AnimName.ToString(), ability.abilities[0].InitialUseDelay[0], cf.Controls.animController);

                break;
            case AbilityAttack.Archetype.Multi_Followup:
                print($"[{gameObject.name}] ATTACK: Multi-Followup");

                AbilityMulti multi_attack = (AbilityMulti)ability;

                //Functionality
                usingTrigger.GetComponent<MAT_FollowupGroup>().StartabilityTrigger(ability, multi_attack.abilities[0].InitialUseDelay[0]);

                System.Enum[] Enums = new System.Enum[multi_attack.abilities.Length];
                for (int i = 0; i < multi_attack.abilities.Length; i++)
                {
                    AbilityAttack abilityi = ((AbilityAttack)multi_attack.abilities[i]);
                    System.Enum _enum = abilityi.Attacks;
                    Enums[i] = _enum;
                }

                //Animation
                AM.FollowUpPackage FollowUpPackage = new AM.FollowUpPackage(
                    usingTrigger,
                    attack,
                    Enums,
                    typeof(AM.AtkAnims),
                    typeof(AM.AtkAnims.Anims),
                    CharacterAnimationController.UPPERBODY,
                    false,
                    false,
                    0.2f
                    );
                StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));

                //Special Functionality
                if (hasAf)
                    Archetype_FollowUpAttack((AbilityMulti)ability);


                break;
        }
    }

    #region Flags

    /// <summary>
    /// Sets Control's alreadyAttacking flag to TRUE
    /// </summary>
    public void StartAttacking()
    {
        cf.Controls.Mode("Attack").isUsing = true;
    }

    /// <summary>
    /// Sets Control's alreadyAttacking flag to FALSE
    /// </summary>
    public void FinishAttacking()
    {
        cf.Controls.Mode("Attack").isUsing = false;
        cf.Controls.didReattack = false;
    }
    #endregion



    #region Archetypes
    void AF_Attack(Ability ability)
    {
        if (ability.AF_Dictionary.TryGetValue("movement", out AF af))
        {

        }
    }

    void AF_Choice(Ability ability, string choice)
    {
        if (ability.AF_Dictionary.TryGetValue("choice", out AF af))
            (af as AF_choice).choice = choice;

    }


    void Archetype_FollowUpAttack(AbilityMulti mltiAbility)
    {

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
                {
                    print($"comparing: ({ability.choices[i]}) to ({choice})");
                    if (ability.choices[i] == choice)
                    {
                        print($"FOUND CHOICE : {choice}");
                        return choice;
                    }
                }
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
            MAT_FollowupGroup trigger = cf.Controls.Mode("Attack").trigger.GetComponent<MAT_FollowupGroup>();
            if(trigger.IncrementTriggerProgress() == true)
            {
                print("didreattack: final trigger prog");
                trigger.DisableThisTrigger();
            }
        }

        if (ability.archetype == Ability.Archetype.Multi_Choice)
        {
            print("+didreattack: Ability_MultiChoice blocked");
            MAT_ChoiceGroup trigger = cf.Controls.Mode("Attack").trigger.GetComponent<MAT_ChoiceGroup>();

            trigger.DisableThisTrigger();
        }
    }

}

#endregion