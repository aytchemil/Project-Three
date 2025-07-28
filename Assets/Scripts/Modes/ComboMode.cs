using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static EntityController;

public class ComboMode : MonoBehaviour, ICombatMode
{
    public CombatFunctionality cf { get; set; }
    public string MODE { get => "Combo"; }
    public RuntimeModeData Mode { get => cf.Controls.Mode(MODE); }
    public RuntimeModeData AtkMode { get => cf.Controls.Mode("Attack"); }


    bool waiting = false;

    private void OnEnable()
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
        cf = gameObject.GetComponent<CombatFunctionality>();
        cf.Controls.CombatWheelSelectDirection += SwitchToCombo;
        cf.Controls.MyAttackWasBlocked += MyComboWasBlocked;
    }

    private void OnDisable()
    {
        cf.Controls.CombatWheelSelectDirection -= SwitchToCombo;
        cf.Controls.MyAttackWasBlocked -= MyComboWasBlocked;

    }

    public void UseModeImplementation()
    {
        AbilityCombo combo = (AbilityCombo)Mode.ability;
        combo.Use(this, cf, Mode);
    }



    /// <summary>
    /// Switches to another combo, waiting until attack.isusing is false
    /// </summary>
    /// <param name="dir"></param>
    void SwitchToCombo(string dir)
    {
        //print($"[{gameObject.name}] COMBO Direction ({dir})");

        if (AtkMode.isUsing)
        {
            if (waiting == true)
                StopCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));

            waiting = true;
            StartCoroutine(WaitForComboingToFinishToSwitchToAnother(dir));
        }

        Mode.ability = cf.ChooseCurrentWheelAbility(dir, MODE);

        ///Waits until the player stops attacking to switch to another combo
        IEnumerator WaitForComboingToFinishToSwitchToAnother(string dir)
        {
            while (AtkMode.isUsing)
            {
                yield return new WaitForEndOfFrame();
            }
            Mode.ability = cf.ChooseCurrentWheelAbility(dir, MODE);
            waiting = false;
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
