
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityController;



public class AttackMode : MonoBehaviour, ICombatMode
{
    public CombatFunctionality cf { get; set; }

    public string MODE { get => "Attack"; }

    public RuntimeModeData Mode { get => cf.Controls.Mode(MODE); }

    protected void OnEnable()
    {
        //print("CF" + cf);
        //print(gameObject.GetComponent<CombatFunctionality>());
        if (cf == null)
            cf = gameObject.GetComponent<CombatFunctionality>();
        cf.Controls.MyAttackWasBlocked += AttackBlocked;
    }

    protected void OnDisable()
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
        Ability ability = Mode.ability;
        ModeTriggerGroup trigger = cf.AbilityTriggerEnableUse(MODE);
        bool hasAf = ability.hasAdditionalFunctionality;

        //Flags
        Mode.functionality.Starting();

        //Initial Mutations
        // + SETS the curr ability
        // + SETS the curr Trigger
        Mode.SetAbility(ability);
        Mode.trigger = trigger;

        if(hasAf && ability.hasInitializedAfs == false) ability.InitializeAFValues();

        if(ability.archetype == Ability.Archetype.Singular)
        {
            AbilityAttack ability_Mode = (AbilityAttack)ability;

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
            StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));


        }

    }

    void ICombatMode.Finish()
    {
        (this as ICombatMode).FinishImplementation();
        cf.Controls.didReattack = false;
    }



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
                Mode.functionality.Finish();
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
            MAT_FollowupGroup trigger = Mode.trigger.GetComponent<MAT_FollowupGroup>();
            if(trigger.IncrementTriggerProgress() == true)
            {
                print("didreattack: final trigger prog");
                trigger.DisableThisTrigger();
            }
        }

        if (ability.archetype == Ability.Archetype.Multi_Choice)
        {
            print("+didreattack: Ability_MultiChoice blocked");
            MAT_ChoiceGroup trigger = Mode.trigger.GetComponent<MAT_ChoiceGroup>();

            trigger.DisableThisTrigger();
        }
    }

}

#endregion