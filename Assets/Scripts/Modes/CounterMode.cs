using System;
using System.Collections;
using UnityEngine;
using static EntityController;

public class CounterMode : MonoBehaviour, ICombatMode
{
    public CombatFunctionality cf { get; set; }
    public string MODE { get => "Counter"; }
    public RuntimeModeData Mode { get => cf.Controls.Mode(MODE); }


    public string stopBlockingBool = "stopBlocking";

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
        // cf.Controls.blockStart += BlockStart;
        // cf.Controls.blockStop += BlockStop;
    }
    private void OnDisable()
    {
       // cf.Controls.blockStart -= BlockStart;
       // cf.Controls.blockStop -= BlockStop;
    }

    /// <summary>
    /// Override passage to mode functionality
    /// </summary>
    public void UseModeFunctionality()
    {
        Counter();
    }

    void Counter()
    {
        print("countering");

        //Setup
        AbilityCounter ability = (AbilityCounter)Mode.ability;

        //Trigger
        ModeTriggerGroup trigger = cf.AbilityTriggerEnableUse(Mode);

        //Ability

        switch (ability.counterArchetype)
        {
            case AbilityCounter.CounterArchetype.StandingRiposte:

                //Setup
                //ability.completedAnimation = trigger.GetComponent<CounterTriggerGroup>().triggerProgress;

                //Animation
                //StartCoroutine(AnimateFollowUpAbilities(ability, trigger, cf.Controls.Mode("Counter"), cf.Controls.animController));

                //Trigger
                trigger.Use(ability.initialUseDelay[0]);

                //Additional Functionality 
                StandingRiposte(ability);

                break;
        }
    }


    void StandingRiposte(AbilityCounter ability)
    {
        print("Counter attack: Standing riposte");

    }

    public void UseModeImplementation()
    {
        throw new NotImplementedException();
    }
}
