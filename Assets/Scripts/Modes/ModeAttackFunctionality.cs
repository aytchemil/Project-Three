using System.Collections;
using UnityEngine;

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
        print("attacking");


        StartAttacking();

        ///to do: create a way for it to animate,
        ///create 4 different attack triggers(like box)
        /// animate all 4, integrate that


        Ability ability = cf.Controls.Mode("Attack").data.currentAbility;
        print($"Using attack ability: {ability.abilityName}");

        //Current Ability Being Used For Each Mode System
        cf.SearchCurrentModesForMode("Attack").SetAbility(ability);

        switch (ability.archetype)
        {

            case AbilityAttack.Archetype.Singular:

                print("attackingarchetype: singular");

                //Actuall Attack
                cf.TriggerEnableToUse("Attack").StartUsingAbilityTrigger(ability, ability.initialUseDelay[0]);

                //Special Functionality
                Archetype_SingularAttack((AbilityAttack)ability);


                AnimationAttack((ability as AbilityAttack).anim_name.ToString());

                break;

            case AbilityAttack.Archetype.Multi_Choice:

                print("attacking archetype: multichoice");

                //Gets the Choice
                string choice = GetMultiChoiceAttack(ability);

                if (choice == "none") break;

                //Actuall Attack
                StartCoroutine(cf.TriggerEnableToUse("Attack").GetComponent<MAT_ChoiceGroup>().MultiChoiceAttack((AbilityMulti)ability, ability.initialUseDelay[0], choice));

                //Special Functionality
                Archetype_MultiChoiceAttack(ability, choice);


                break;

            case AbilityAttack.Archetype.Multi_Followup:

                print("attacking archetype: multi_followup");

                //Actuall Attack
                cf.TriggerEnableToUse("Attack").GetComponent<MAT_FollowupGroup>().StartUsingAbilityTrigger(ability, ability.initialUseDelay[0]);

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
    void AnimationAttack(string animationName)
    {
        Debug.Log($"[{gameObject.name}] [CombatFunctionality] is using AnimationAttack( [{name}] )");
        cf.Controls.animController.UseAnimation?.Invoke(animationName);

    }




    #endregion
}
