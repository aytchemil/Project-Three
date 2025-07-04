using System;
using System.Collections;
using UnityEngine;
using static Ability;
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
        cf.Controls.MyAttackWasBlocked += MyComboWasBlocked;
    }

    private void OnDisable()
    {
        cf.Controls.CombatWheelSelectDirection -= SwitchToCombo;
        cf.Controls.MyAttackWasBlocked -= MyComboWasBlocked;

    }

    public override void UseModeFunctionality() => Combo();

    void Combo()
    {
        print($"[{gameObject.name}] COMBAT [ModeCombo]: Combo Started...");

        //Validation
        if (cf.Controls.cantUseAbility.Invoke())
            return;

        //Setup
        CombatEntityModeData combo = cf.Controls.Mode("Combo");
        AbilityCombo ability = (AbilityCombo)combo.ability;
        ModeTriggerGroup usingTrigger = cf.WheelTriggerEnableUse("Combo");
        AbilityWrapper usingAbility = new((ability as AbilityCombo).abilities, ability);

        //Flags
        combo.isUsing = true;
        (cf.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).StartAttacking();

        //Initial Mutations
        //+Set all Triggers to False
        //+Sets the curr ability
        //+Sets the curr trigger
        for (int i = 0; i < combo.triggers.Length; i++) 
            combo.triggers[i].gameObject.SetActive(false);
        combo.SetAbility(ability); 
        combo.trigger = usingTrigger;

        ComboSetupCompletePrint();

        //Desired Mutations
        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                usingAbility.completedAnimation = usingTrigger.GetComponent<CombotTriggerGroup>().triggerProgress;
                StartCoroutine(WaitForComboToFinish(usingTrigger));

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Attack"), cf.Controls.animController));

                //Actuall Attack
                UseCurrentCombo(cf.Controls.lookDir).GetComponent<CombotTriggerGroup>().StartUsingAbilityTrigger(usingAbility, ability.InitialUseDelay[0]);

                //Special Functionality
                //ArchetypeUse_FollowUpAttack((AbilityMulti)ability);
                break;
        }

        void ComboSetupCompletePrint()
        {
            print($"[Combo Functionality] the abilities for this combo are:{usingAbility.abilities[0]} {usingAbility.abilities[1]} {usingAbility.abilities[2]} for trigger: [{usingTrigger.gameObject.name}]");
            print("combo setup complete");
        }
    }

    /// <summary>
    /// +Sets all trigers to false
    /// +Sets a trigger active
    /// +returns that trigger
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="Exception"></exception>
    CombotTriggerGroup UseCurrentCombo(string dir)
    {
        print($"[{gameObject.name}] ModeComboFunctionality : UseCurrentCombo({dir})");

        CombatEntityModeData combo = cf.Controls.Mode("Combo");
        int trigerIndx = cf.GetDirIndex(dir);

        //Checks
        if (combo == null || combo.triggers == null)
        {
            throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
        }
        if (dir == null)
        {
            throw new Exception($"direction: [ {dir} ] is null");
        }

        //Set all triggers to false
        foreach (GameObject trigger in combo.triggers)
            trigger.SetActive(false);

        combo.triggers[trigerIndx].SetActive(true);

        return combo.triggers[trigerIndx].GetComponent<CombotTriggerGroup>();

    }

    /// <summary>
    /// Switches to another combo, waiting until attack.isusing is false
    /// </summary>
    /// <param name="dir"></param>
    void SwitchToCombo(string dir)
    {
        print($"[{gameObject.name}] ModeComboFunctionality : SwitchToCombo({dir})");

        if (cf.Controls.Mode("Attack").isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));
        }

        cf.Controls.Mode("Combo").ability = cf.ChooseCurrentWheelAbility(dir, "Combo");

        ///Waits until the player stops attacking to switch to another combo
        IEnumerator WaitForComboingToFinishToSwitchToAnother(string dir)
        {
            while (cf.Controls.Mode("Attack").isUsing)
            {
                yield return new WaitForEndOfFrame();
            }
            cf.Controls.Mode("Combo").ability = cf.ChooseCurrentWheelAbility(dir, "Combo");
            waiting = false;
        }
    }

    /// <summary>
    /// Waits for the combo to finish (either reach final trigger proggress or attack.isusing = false)
    /// </summary>
    /// <param name="usingTrigger"></param>
    /// <returns></returns>
    IEnumerator WaitForComboToFinish(ModeTriggerGroup usingTrigger)
    {
        print($"[{gameObject.name}] ModeComboFunctionality : WaitForComboToFinish(ModeTriggerGroup:{usingTrigger.name})");

        CombotTriggerGroup trigger = usingTrigger.GetComponent<CombotTriggerGroup>();

        while (cf.Controls.Mode("Combo").isUsing == true)
        {
            print($"[ComboFunctionality] [{gameObject.name}] waiting for combo to finish");
            yield return new WaitForEndOfFrame();

            if (trigger.triggerProgress[trigger.triggerProgress.Count - 1] == true || cf.Controls.Mode("Attack").isUsing == false)
                cf.Controls.Mode("Combo").isUsing = false;
        }
        print("[ComboFunctionality] combo finished");

    }

    /// <summary>
    /// Listener for combo being blocked
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="ability"></param>
    void MyComboWasBlocked(string myLookdir, Ability ability)
    {
        if (ability.GetType() != typeof(AbilityCombo)) return;
        print("didreattack: mycombo was blocked");
    }
}
