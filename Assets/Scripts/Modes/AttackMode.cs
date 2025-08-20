
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
        if (cf == null)
            cf = gameObject.GetComponent<CombatFunctionality>();
        if (cf.Controls == null)
            cf.Controls = cf.gameObject.GetComponent<EntityController>();

        //Waiting for EntityController to initialize
        if (cf.Controls.initialized == false)
        {
            WaitExtension.WaitAFrame(this, OnEnable);
            return;
        }

        //Null Guards
        if (cf == null)
            Debug.LogError($"[{gameObject.name}] CF not properly set");
        if (cf.Controls == null)
            Debug.LogError($"[{gameObject.name}] Controls not properly set");
        if (Mode == null)
            Debug.LogError($"[{gameObject.name}] Mode Reference not getting");

        InitializeFunctionalityAfterOnEnable();
    }

    public void InitializeFunctionalityAfterOnEnable()
    {
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
        print("BlockSys: AttackBlocked?");
        if (ability.modeBase != Ability.Mode.AttackBased) return;
        print($"BlockSys: +YES:[{gameObject.name}]");

        if (ability.archetype == Ability.Archetype.Multi_Followup)
        {
            print("+BlockSys: AbilityMulti blocked");
            //Nothing Will Happen
        }

        if (ability.archetype == Ability.Archetype.Multi_Choice)
        {
            print("+didreattack: Ability_MultiChoice blocked");
            MAT_ChoiceGroup trigger = Mode.trigger.GetComponent<MAT_ChoiceGroup>();

            trigger.DisableThisTrigger();
        }

        Mode.isUsing = false;
    }

}

#endregion