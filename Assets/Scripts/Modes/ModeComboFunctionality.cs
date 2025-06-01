using System;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class ModeComboFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;


    public override string MODE_NAME { get => "Combo"; }

    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    public override void UseModeFunctionality() => Combo();

    void Combo()
    {
        if (cf.Controls.cantUseAbility.Invoke())
            return;

        print("comboing");
        AbilityCombo ability = (AbilityCombo)cf.Controls.Mode("Combo").data.currentAbility;

        //Set all combos to false
        for (int i = 0; i < cf.Controls.Mode("Combo").triggers.Length; i++)
            cf.Controls.Mode("Combo").triggers[i].gameObject.SetActive(false);

        cf.SearchCurrentModesForMode("Combo").SetAbility(ability);

        (cf.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).StartAttacking();

        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Actuall Attack
                UseCurrentCombo(cf.Controls.c_current).GetComponent<CombotTriggerGroup>().StartUsingAbilityTrigger(ability, ability.initialUseDelay[0]);


                //Special Functionality
                //ArchetypeUse_FollowUpAttack((AbilityMulti)ability);

                break;
        }
    }

    CombotTriggerGroup UseCurrentCombo(int combo)
    {
        ModeRuntimeData mode = cf.Controls.Mode("Combo");

        //Checks
        if (mode == null || mode.triggers == null)
        {
            throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
        }
        if (combo < 0 || combo >= mode.triggers.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(combo), "Combo index is out of range.");
        }

        foreach (GameObject trigger in mode.triggers)
            trigger.SetActive(false);

        mode.triggers[combo].SetActive(true);

        return mode.triggers[combo].GetComponent<CombotTriggerGroup>();

    }
}
