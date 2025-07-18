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
        Mode.functionality.Starting();
        AbilityCounter ability = (AbilityCounter)Mode.ability;
        Mode.SetAbility(ability);

        //Trigger
        ModeTriggerGroup trigger = cf.AbilityTriggerEnableUse(MODE);
        Mode.trigger = trigger;

        //Ability

        switch (ability.counterArchetype)
        {
            case AbilityCounter.CounterArchetype.StandingRiposte:

                //Setup
                //ability.completedAnimation = trigger.GetComponent<CounterTriggerGroup>().triggerProgress;

                //Animation
                //StartCoroutine(AnimateFollowUpAbilities(ability, trigger, cf.Controls.Mode("Counter"), cf.Controls.animController));

                //Trigger
                trigger.Use(ability.InitialUseDelay[0]);

                //Additional Functionality 
                StandingRiposte(ability);

                break;
        }
    }


    void StandingRiposte(AbilityCounter ability)
    {
        print("Counter attack: Standing riposte");

    }

}
