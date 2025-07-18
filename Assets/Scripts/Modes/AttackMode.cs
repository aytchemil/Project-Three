
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
    public void UseModeImplementation()
    {
        Ability ability = Mode.ability;
        ability.Use(this, cf, Mode);
    }


    void ICombatMode.Finish()
    {
        (this as ICombatMode).FinishImplementation();
        cf.Controls.didReattack = false;
    }



    #region Archetypes






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