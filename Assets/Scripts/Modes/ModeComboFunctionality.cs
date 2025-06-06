using System;
using System.Collections;
using UnityEngine;
using static CombatEntityController;

public class ModeComboFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;
    public override string MODE_NAME { get => "Combo"; }

    bool waiting = false;

    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    private void OnEnable()
    {
        cf.Controls.comboOne += SwitchToCombo;
        cf.Controls.comboTwo += SwitchToCombo;
        cf.Controls.comboThree += SwitchToCombo;
        cf.Controls.comboFour += SwitchToCombo;
    }

    private void OnDisable()
    {
        cf.Controls.comboOne -= SwitchToCombo;
        cf.Controls.comboTwo -= SwitchToCombo;
        cf.Controls.comboThree -= SwitchToCombo;
        cf.Controls.comboFour -= SwitchToCombo;
    }

    public override void UseModeFunctionality() => Combo();

    void Combo()
    {
        //Validation
        if (cf.Controls.cantUseAbility.Invoke())
            return;

        print("comboing");

        //Setup
        AbilityCombo ability = (AbilityCombo)cf.Controls.Mode("Combo").ability;

        for (int i = 0; i < cf.Controls.Mode("Combo").triggers.Length; i++)   //All triggers become falsee
            cf.Controls.Mode("Combo").triggers[i].gameObject.SetActive(false);

        cf.Controls.Mode("Combo").SetAbility(ability); //Parse and edit the mode becuase it prints like "Mode: mode" for some reason
        (cf.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).StartAttacking(); //Flag: attacking true (in cf)


        //Trigger
        ModeTriggerGroup usingTrigger = cf.ComboTriggerEnableUse();
        //Ability
        AbilityWrapper usingAbility = new((ability as AbilityCombo).abilities, ability);
        print($"[Combo Functionality] the abilities for this combo are:{usingAbility.Values[0]} {usingAbility.Values[1]} {usingAbility.Values[2]} for trigger: [{usingTrigger.gameObject.name}]");

        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                usingAbility.completedAnimation = usingTrigger.GetComponent<CombotTriggerGroup>().triggerProgress;

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Attack"), cf.Controls.animController));

                //Actuall Attack
                UseCurrentCombo(cf.Controls.c_current).GetComponent<CombotTriggerGroup>().StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]);


                //Special Functionality
                //ArchetypeUse_FollowUpAttack((AbilityMulti)ability);

                break;
        }
    }

    CombotTriggerGroup UseCurrentCombo(int combo)
    {
        CombatEntityModeData mode = cf.Controls.Mode("Combo");

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

    void SwitchToCombo(int combo)
    {
        print("switching to combo: " + combo);

        if (cf.Controls.Mode("Attack").isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(combo));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(combo));
        }

        cf.Controls.Mode("Combo").ability = ChooseCurrentComboAbility(combo);
        cf.Controls.c_current = combo;
    }

    IEnumerator WaitForComboingToFinishToSwitchToAnother(int combo)
    {
        while (cf.Controls.Mode("Attack").isUsing)
        {
            yield return new WaitForEndOfFrame();
        }
        cf.Controls.Mode("Combo").ability = ChooseCurrentComboAbility(combo);
        cf.Controls.c_current = combo;
        waiting = false;
    }

    Ability ChooseCurrentComboAbility(int combo)
    {
        Ability ret = null;

        if (combo == 0)
            ret = cf.Controls.Mode("Combo").data.abilitySet.right;

        else if (combo == 1)
            ret = cf.Controls.Mode("Combo").data.abilitySet.left;

        else if (combo == 2)
            ret = cf.Controls.Mode("Combo").data.abilitySet.up;

        else if (combo == 3)
            ret = cf.Controls.Mode("Combo").data.abilitySet.down;

        return ret;
    }
}
