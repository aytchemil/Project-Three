using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;

/// <summary>
/// WRAPPER CLASS for an ability
/// + provided for abilities with child abilities
/// </summary>
public class AbilityWrapper
{
    public Ability parentAbility;
    public List<AF> afs;
    public List<Ability> abilities;
    public List<bool> completedAnimation;

    public AbilityWrapper(Ability parent)
    {
        parentAbility = parent;
        abilities = new List<Ability>();
        completedAnimation = new List<bool>();
        afs = new List<AF>();
    }

    public AbilityWrapper(Ability[] values, Ability parent)
    {
        parentAbility = parent;
        completedAnimation = new List<bool>();
        afs = new List<AF>();
        abilities = new List<Ability>();

        for (int i = 0; i < values.Length; i++)
        {
            abilities.Add(values[i]);
            completedAnimation.Add(false);
        }
    }
    public AF GetAF(string name)
    {
        foreach (AF af in afs)
            if (af.name == name)
                return af;
        throw new System.Exception($"Combat Additional Functionality ({name}) package Not Found");
    }
}


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
        print($"[{gameObject.name}] [ModeAttack] Attacking STARTED...");


        //Setup
        CombatEntityController.CombatEntityModeData attack = cf.Controls.Mode("Attack");
        Ability ability = attack.ability;
        ModeTriggerGroup usingTrigger = cf.AbilityTriggerEnableUse("Attack");
        AbilityWrapper usingAbility = new AbilityWrapper(ability);
        bool hasAf = ability.hasAdditionalFunctionality;

        //Flags
        StartAttacking();

        //Initial Mutations
        // + SETS the curr ability
        // + SETS the curr Trigger
        attack.SetAbility(ability);
        attack.trigger = usingTrigger;


        //Mutations
        switch (ability.archetype)
        {
            case AbilityAttack.Archetype.Singular:

                print($"[{gameObject.name}] [ModeAttack] Archetype: Singular");

                //Setup

                //Mutations
                usingAbility.abilities.Add(ability);

                //Additional Functionality 
                if (hasAf)
                    AF_Attack(usingAbility);

                //Trigger
                usingTrigger.StartUsingAbilityTrigger(usingAbility, ability.InitialUseDelay[0]);

                //Animation
                AnimateAblity(usingAbility.abilities[0].AnimName.ToString(), usingAbility.abilities[0].InitialUseDelay[0], cf.Controls.animController);

                break;
            case AbilityAttack.Archetype.Multi_Choice:

                print($"[{gameObject.name}] [ModeAttack] Archetype: Multi-Choice");

                //Setup
                string choice = GetMultiChoiceAttack(ability); if (choice == "none") break;
                MAT_ChoiceGroup parentTrigger = usingTrigger.GetComponent<MAT_ChoiceGroup>();

                //Additional Functionality
                if (hasAf)
                {
                    AF_Attack(usingAbility);
                    AF_Choice(usingAbility, choice);
                }

                //Trigger 
                StartCoroutine(parentTrigger.MultiChoiceAttack(usingAbility, ability.InitialUseDelay[0], choice, usingAbility));

                //Animation
                AnimateAblity(usingAbility.abilities[0].AnimName.ToString(), usingAbility.abilities[0].InitialUseDelay[0], cf.Controls.animController);

                break;
            case AbilityAttack.Archetype.Multi_Followup:

                print($"[{gameObject.name}] [ModeAttack] Archetype: Multi-Followup");

                //Setup
                usingAbility = new AbilityWrapper((ability as AbilityMulti).abilities, ability);

                usingAbility.completedAnimation = usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress;

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Attack"), cf.Controls.animController));

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
    void AF_Attack(AbilityWrapper usingAbility)
    {
        Ability ability = usingAbility.parentAbility;

        if (ability.af == CombatAdditionalFunctionalities.Function.MovementForward)
        {
            AF_movement afmove = new(ability.movementAmount);
            usingAbility.afs.Add(afmove);
        }
        else if (ability.af == CombatAdditionalFunctionalities.Function.MovementLeftOrRight)
        {
            AF_movement afmove = new(ability.movementAmount);
            usingAbility.afs.Add(afmove);
        }
    }

    void AF_Choice(AbilityWrapper usingAbility, string choice)
    {
        Ability ability = usingAbility.parentAbility;

        if (ability.af == CombatAdditionalFunctionalities.Function.MovementLeftOrRight)
        {
            AF_choice afchoice = new(choice);
            usingAbility.afs.Add(afchoice);
        }
    }


    void Archetype_FollowUpAttack(AbilityMulti mltiAbility)
    {

    }

    string GetMultiChoiceAttack(Ability ability)
    {
        string choice = "";

        switch (ability.af)
        {
            case CombatAdditionalFunctionalities.Function.MovementLeftOrRight:

                choice = cf.Controls.getMoveDirection?.Invoke();

                if (choice == "foward" || choice == "back" || choice == "none")
                {
                    choice = "none";
                    print("Not able to use ability, critiera not met");
                    FinishAttacking();
                    break;
                }

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