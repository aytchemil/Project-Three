using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityWrapper
{
    public Ability parentAbility;
    public List<Ability> Values;
    public List<bool> completedAnimation;

    public AbilityWrapper()
    {
        parentAbility = new Ability();
        Values = new List<Ability>();
        completedAnimation = new List<bool>();
    }

    public AbilityWrapper(Ability[] values, Ability parent)
    {
        parentAbility = parent;
        Values = new List<Ability>();
        completedAnimation = new List<bool>();

        for(int i = 0; i < values.Length; i++)
        {
            Values.Add(values[i]);
            completedAnimation.Add(false);
        }
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

    public override void UseModeFunctionality() => Attack();
    
    void Attack()
    {
        print("[ModeAttackFunctionality] Attacking");


        StartAttacking();

        ///to do: create a way for it to animate,
        ///create 4 different attack triggers(like box)
        /// animate all 4, integrate that


        Ability ability = cf.Controls.Mode("Attack").data.currentAbility;
        print($" + Using attack ability: {ability.abilityName}");

        //Current Ability Being Used For Each Mode System
        cf.SearchCurrentModesForMode("Attack").SetAbility(ability);

        //Trigger----------------------------------------------------
        ModeTriggerGroup usingTrigger = cf.AbilityTriggerEnableUse("Attack");
        //Ability
        AbilityWrapper usingAbility = new AbilityWrapper();

        switch (ability.archetype)
        {

            case AbilityAttack.Archetype.Singular:

                print(" + + archetype: singular");

                //Setup

                //Trigger
                usingAbility.Values.Add(usingTrigger.StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]));

                //Additional Functionality 
                Archetype_SingularAttack((AbilityAttack)usingAbility.Values[0]);

                //Animation
                AnimationAttack((usingAbility.Values[0] as AbilityAttack).anim_name.ToString(), usingAbility.Values[0].InitialUseDelay[0]);

                break;

            case AbilityAttack.Archetype.Multi_Choice:

                print(" + + archetype: multichoice");

                //Setup
                string choice = GetMultiChoiceAttack(ability);
                if (choice == "none") break;

                //Trigger
                StartCoroutine(usingTrigger.GetComponent<MAT_ChoiceGroup>().MultiChoiceAttack((AbilityMulti)ability, ability.InitialUseDelay[0], choice, usingAbility));
                Debug.Log($"[ModeAttackFunctionality] Multi_Choice attack chosen is: [{usingAbility.Values[0]}]");

                //Additional Functionality
                Archetype_MultiChoiceAttack(ability, choice);

                //Animation
                AnimationAttack((usingAbility.Values[0] as AbilityAttack).anim_name.ToString(), usingAbility.Values[0].InitialUseDelay[0]);

                break;

            case AbilityAttack.Archetype.Multi_Followup:

                print(" + + archetype: multi_followup");

                //Setup
                usingAbility = new AbilityWrapper((ability as AbilityMulti).abilities, ability);
                usingAbility.completedAnimation = usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress;

                //Animation
                StartCoroutine(UpdateAnimationsForFollowUpAttacks(usingAbility, usingTrigger));


                //Trigger
                usingTrigger.GetComponent<MAT_FollowupGroup>().StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]);

                //Special Functionality
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
        cf.Controls.alreadyAttacking = true;
    }

    /// <summary>
    /// Sets Control's alreadyAttacking flag to FALSE
    /// </summary>
    public void FinishAttacking()
    {
        cf.Controls.alreadyAttacking = false;
    }
    #endregion

    #region Archetypes
    void Archetype_SingularAttack(AbilityAttack attack)
    {
        switch (attack.trait)
        {
            case AbilityAttack.Trait.MovementForward:

                StartCoroutine(MovementForwardAttack(attack));

                break;
        }
    }

    void Archetype_MultiChoiceAttack(Ability ability, string choice)
    {
        switch (ability.trait)
        {
            case AbilityAttack.Trait.MovementLeftOrRight:

                StartCoroutine(MovementRightOrLeftAttack(choice, ability));

                break;
        }
    }

    void Archetype_FollowUpAttack(AbilityMulti mltiAbility)
    {

    }

    string GetMultiChoiceAttack(Ability ability)
    {
        string choice = "";

        switch (ability.trait)
        {
            case AbilityAttack.Trait.MovementLeftOrRight:

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
    #endregion

    #region Special Attacks
    IEnumerator MovementForwardAttack(AbilityAttack attack)
    {
        Debug.Log(" * MoveForwardAttacK Called");

        //print("Waiting for attack to start, initialAttackDelayOver not over yet (its false)");
        //print("initial attack delay over?: " + initialAttackDelayOver);
        while (!cf.initialAbilityUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
        // print("Attacking started, initialAttackDelayOver is over (true)");


        gameObject.GetComponent<Movement>().Lunge("up", attack.movementAmount);
        gameObject.GetComponent<Movement>().DisableMovement();
        Invoke(nameof(ReEnableMovement), gameObject.GetComponent<Movement>().entityStates.dashTime);
    }


    IEnumerator MovementRightOrLeftAttack(string choice, Ability ability)
    {
        Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ MovementLeftOrRight attack");




        gameObject.GetComponent<Movement>().Lunge(choice, ability.movementAmount);

        print("multi attack trigger, movementatttackrightorleft : lunging in dir " + choice);

        while (!cf.initialAbilityUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }

    }
    #endregion

    void ReEnableMovement()
    {
        gameObject.GetComponent<Movement>().EnableMovement();
    }

    #region Animation
    void AnimationAttack(string animationName, float delay)
    {
        Debug.Log($"[{gameObject.name}] [CombatFunctionality] is using AnimationAttack( [{name}] )");
        cf.Controls.animController.UseAnimation?.Invoke(animationName, delay);
    }


    IEnumerator UpdateAnimationsForFollowUpAttacks(AbilityWrapper usingAbility, ModeTriggerGroup usingTrigger)
    {
        for(int i = 0; i < usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress.Count && cf.Controls.alreadyAttacking; i++)
        {
            AnimationAttack((usingAbility.Values[i] as AbilityAttack).anim_name.ToString(), usingAbility.parentAbility.InitialUseDelay[i]);
            while (cf.Controls.alreadyAttacking && usingAbility.completedAnimation[i] == false)
            {
                Debug.Log("Updating animation");
                yield return new WaitForEndOfFrame();
                usingAbility.completedAnimation = usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress;
            }
        }

    }




    #endregion
}
