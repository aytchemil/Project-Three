using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static CombatEntityController;

public class ModeComboFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;
    public override string MODE { get => "Combo"; }

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
        print($"[{gameObject.name}] [COMBO]: Combo Started...");

        //Validation
        if (cf.Controls.cantUseAbility)
            return;

        //Setup
        CombatEntityModeData combo = cf.Controls.Mode(MODE);
        AbilityCombo ability = (AbilityCombo)combo.ability;
        ModeTriggerGroup trigger = cf.WheelTriggerEnableUse(MODE);

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
        combo.trigger = trigger;

        //Desired Mutations
        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                StartCoroutine(WaitForComboToFinish(trigger));

                //Actuall Attack
                UseCurrentCombo().Use(ability.InitialUseDelay[0]);

                //Animation
                AM.FollowUpPackage FollowUpPackage = new AM.FollowUpPackage(
                    trigger,
                    combo,
                    GetAnimEnums(ability),
                    typeof(AM.AtkAnims),
                    typeof(AM.AtkAnims.Anims),
                    CharacterAnimationController.UPPERBODY,
                    false,
                    false,
                    0.2f
                    );
                StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));
                break;
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
    CombotTriggerGroup UseCurrentCombo()
    {
        string dir = cf.Controls.lookDir;
        //print($"[MODE] [COMBO] : Direction Chosen ({dir})");

        CombatEntityModeData combo = cf.Controls.Mode(MODE);
        int trigerIndx = cf.GetDirIndex(dir);

        //Errors
        if (combo == null || combo.triggers == null) throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
        if (dir == null) throw new Exception($"direction: [ {dir} ] is null");

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
        //print($"[{gameObject.name}] COMBO Direction ({dir})");

        if (cf.Controls.Mode("Attack").isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));
        }

        cf.Controls.Mode(MODE).ability = cf.ChooseCurrentWheelAbility(dir, MODE);

        ///Waits until the player stops attacking to switch to another combo
        IEnumerator WaitForComboingToFinishToSwitchToAnother(string dir)
        {
            while (cf.Controls.Mode("Attack").isUsing)
            {
                yield return new WaitForEndOfFrame();
            }
            cf.Controls.Mode(MODE).ability = cf.ChooseCurrentWheelAbility(dir, MODE);
            waiting = false;
        }
    }

    /// <summary>
    /// Waits for the combo to finish (either reach final trigger proggress or attack.isusing = false)
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    IEnumerator WaitForComboToFinish(ModeTriggerGroup trigger)
    {
        CombotTriggerGroup comboTrigger = trigger.GetComponent<CombotTriggerGroup>();

        while (cf.Controls.Mode(MODE).isUsing == true)
        {
            //print($"[MODE] [COMBO] waiting for combo to finish");
            yield return new WaitForEndOfFrame();

            if (comboTrigger.triggerProgress[comboTrigger.triggerProgress.Length-1] == true || cf.Controls.Mode("Attack").isUsing == false)
                cf.Controls.Mode(MODE).isUsing = false;
        }
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
