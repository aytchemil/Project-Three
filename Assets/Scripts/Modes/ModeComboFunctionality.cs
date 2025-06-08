using System;
using System.Collections;
using UnityEditor.ShaderGraph;
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
        cf.Controls.ComboWheelSelectCombo += SwitchToCombo;
    }

    private void OnDisable()
    {
        cf.Controls.ComboWheelSelectCombo -= SwitchToCombo;
    }

    public override void UseModeFunctionality() => Combo();

    void Combo()
    {
        //Validation
        if (cf.Controls.cantUseAbility.Invoke())
            return;

        cf.Controls.Mode("Combo").isUsing = true;

        print("comboing");

        //Setup
        AbilityCombo ability = (AbilityCombo)cf.Controls.Mode("Combo").ability;

        for (int i = 0; i < cf.Controls.Mode("Combo").triggers.Length; i++)   //All triggers become falsee
            cf.Controls.Mode("Combo").triggers[i].gameObject.SetActive(false);

        cf.Controls.Mode("Combo").SetAbility(ability); //Parse and edit the mode becuase it prints like "Mode: mode" for some reason
        (cf.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).StartAttacking(); //Flag: attacking true (in cf)


        //Trigger
        ModeTriggerGroup usingTrigger = ComboTriggerEnableUse();
        //Ability
        AbilityWrapper usingAbility = new((ability as AbilityCombo).abilities, ability);
        print($"[Combo Functionality] the abilities for this combo are:{usingAbility.Values[0]} {usingAbility.Values[1]} {usingAbility.Values[2]} for trigger: [{usingTrigger.gameObject.name}]");

        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                usingAbility.completedAnimation = usingTrigger.GetComponent<CombotTriggerGroup>().triggerProgress;
                StartCoroutine(WaitForComboToFinish(usingTrigger));

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Attack"), cf.Controls.animController));

                //Actuall Attack
                UseCurrentCombo(cf.Controls.c_current).GetComponent<CombotTriggerGroup>().StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]);


                //Special Functionality
                //ArchetypeUse_FollowUpAttack((AbilityMulti)ability);

                break;
        }
    }

    public ModeTriggerGroup ComboTriggerEnableUse()
    {
        ModeTriggerGroup usingThisTriggerGroup = null;
        CombatEntityModeData m = cf.Controls.Mode("Combo");
        int triggerIndx = GetComboTriggerIndex(cf.Controls.c_current);

        for (int i = 0; i < m.triggers.Length - 1; i++)
            m.triggers[i].gameObject.SetActive(false);

        m.triggers[triggerIndx].gameObject.SetActive(true);
        usingThisTriggerGroup = m.triggers[triggerIndx].gameObject.GetComponent<ModeTriggerGroup>();

        return usingThisTriggerGroup;
    }

    CombotTriggerGroup UseCurrentCombo(string dir)
    {
        CombatEntityModeData mode = cf.Controls.Mode("Combo");
        int trigerIndx = GetComboTriggerIndex(dir);

        //Checks
        if (mode == null || mode.triggers == null)
        {
            throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
        }
        if (dir == null)
        {
            throw new ArgumentOutOfRangeException($"{dir} is null");
        }

        //Set all triggers to false
        foreach (GameObject trigger in mode.triggers)
            trigger.SetActive(false);

        mode.triggers[trigerIndx].SetActive(true);

        return mode.triggers[trigerIndx].GetComponent<CombotTriggerGroup>();

    }

    void SwitchToCombo(string combo)
    {
        //print("switching to combo: " + combo);

        if (cf.Controls.Mode("Attack").isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(combo));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(combo));
        }

        cf.Controls.Mode("Combo").ability = ChooseCurrentCombo(combo);
        cf.Controls.c_current = combo;
    }

    IEnumerator WaitForComboingToFinishToSwitchToAnother(string combo)
    {
        while (cf.Controls.Mode("Attack").isUsing)
        {
            yield return new WaitForEndOfFrame();
        }
        cf.Controls.Mode("Combo").ability = ChooseCurrentCombo(combo);
        cf.Controls.c_current = combo;
        waiting = false;
    }

    Ability ChooseCurrentCombo(string combo)
    {
        Ability ret = null;

        if (combo == "right")
            ret = cf.Controls.Mode("Combo").data.abilitySet.right;

        else if (combo == "left")
            ret = cf.Controls.Mode("Combo").data.abilitySet.left;

        else if (combo == "up")
            ret = cf.Controls.Mode("Combo").data.abilitySet.up;

        else if (combo == "down")
            ret = cf.Controls.Mode("Combo").data.abilitySet.down;

        return ret;
    }

    int GetComboTriggerIndex(string combo)
    {
        if (combo == "right")
            return 0;

        else if (combo == "left")
            return 1;

        else if (combo == "up")
            return 2;

        else if (combo == "down")
            return 3;

        throw new Exception($"[ModeComboFunc] trigger index not found for [{combo}]");
    }

    IEnumerator WaitForComboToFinish(ModeTriggerGroup usingTrigger)
    {
        CombotTriggerGroup trigger = usingTrigger.GetComponent<CombotTriggerGroup>();


        while (cf.Controls.Mode("Combo").isUsing == true)
        {
            print("waiting for combo to finish");
            yield return new WaitForEndOfFrame();
            if (trigger.triggerProgress[trigger.triggerProgress.Count - 1] == true)
            {
                print("combo finished");
                cf.Controls.Mode("Combo").isUsing = false;
            }
        }
    }
}
