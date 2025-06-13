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
        cf.Controls.CombatWheelSelectDirection += SwitchToCombo;
    }

    private void OnDisable()
    {
        cf.Controls.CombatWheelSelectDirection -= SwitchToCombo;
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

        //Set all Triggers to False
        for (int i = 0; i < cf.Controls.Mode("Combo").triggers.Length; i++) 
            cf.Controls.Mode("Combo").triggers[i].gameObject.SetActive(false);


        cf.Controls.Mode("Combo").SetAbility(ability); 
        (cf.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).StartAttacking(); //Flag: attacking true (in cf)


        //Trigger
        ModeTriggerGroup usingTrigger = cf.WheelTriggerEnableUse("Combo");
        //Ability
        AbilityWrapper usingAbility = new((ability as AbilityCombo).abilities, ability);
        print($"[Combo Functionality] the abilities for this combo are:{usingAbility.Values[0]} {usingAbility.Values[1]} {usingAbility.Values[2]} for trigger: [{usingTrigger.gameObject.name}]");

        print("combo setup complete");
        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                usingAbility.completedAnimation = usingTrigger.GetComponent<CombotTriggerGroup>().triggerProgress;
                StartCoroutine(WaitForComboToFinish(usingTrigger));

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Attack"), cf.Controls.animController));

                //Actuall Attack
                UseCurrentCombo(cf.Controls.lookDir).GetComponent<CombotTriggerGroup>().StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]);


                //Special Functionality
                //ArchetypeUse_FollowUpAttack((AbilityMulti)ability);

                break;
        }
    }


    CombotTriggerGroup UseCurrentCombo(string dir)
    {
        CombatEntityModeData mode = cf.Controls.Mode("Combo");
        int trigerIndx = cf.GetDirIndex(dir);

        //Checks
        if (mode == null || mode.triggers == null)
        {
            throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
        }
        if (dir == null)
        {
            throw new Exception($"direction: [ {dir} ] is null");
        }

        //Set all triggers to false
        foreach (GameObject trigger in mode.triggers)
            trigger.SetActive(false);

        mode.triggers[trigerIndx].SetActive(true);

        return mode.triggers[trigerIndx].GetComponent<CombotTriggerGroup>();

    }

    void SwitchToCombo(string dir)
    {
        //print("switching to combo: " + combo);

        if (cf.Controls.Mode("Attack").isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));
        }

        cf.Controls.Mode("Combo").ability = cf.ChooseCurrentWheelAbility(dir, "Combo");
    }

    IEnumerator WaitForComboingToFinishToSwitchToAnother(string dir)
    {
        while (cf.Controls.Mode("Attack").isUsing)
        {
            yield return new WaitForEndOfFrame();
        }
        cf.Controls.Mode("Combo").ability = cf.ChooseCurrentWheelAbility(dir, "Combo");
        waiting = false;
    }

    IEnumerator WaitForComboToFinish(ModeTriggerGroup usingTrigger)
    {
        CombotTriggerGroup trigger = usingTrigger.GetComponent<CombotTriggerGroup>();


        while (cf.Controls.Mode("Combo").isUsing == true)
        {
            print($"[ComboFunctionality] [{gameObject.name}] waiting for combo to finish");
            yield return new WaitForEndOfFrame();
            if (trigger.triggerProgress[trigger.triggerProgress.Count - 1] == true || cf.Controls.Mode("Attack").isUsing == false)
            {
                print("[ComboFunctionality] combo finished");
                cf.Controls.Mode("Combo").isUsing = false;
            }
        }
    }
}
