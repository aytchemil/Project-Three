using UnityEngine;

public class ModeCounterFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE_NAME { get => "Attack"; }


    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    public override void UseModeFunctionality() => Counter();
    void Counter()
    {
        //print("countering");

        StartCountering();

        AbilityCounter counterAbility = (AbilityCounter)cf.Controls.Mode("Counter").data.currentAbility;

        cf.SearchCurrentModesForMode("Counter").SetAbility(counterAbility);

        switch (counterAbility.counterArchetype)
        {
            case AbilityCounter.CounterArchetype.StandingRiposte:

                cf.TriggerEnableToUse("Counter").StartUsingAbilityTrigger(counterAbility, counterAbility.InitialUseDelay[0]);

                StandingRiposte();

                break;
        }
    }


    /// <summary>
    /// Sets Control's isCountering flag to TRUE
    /// </summary>
    void StartCountering()
    {
        cf.Controls.isCountering = true;
    }

    /// <summary>
    /// Sets Control's isCountering flag to FALSE
    /// </summary>
    public void FinishCountering()
    {
        cf.Controls.isCountering = false;
    }



    void StandingRiposte()
    {
        print("Counter attack: Standing riposte");

    }

}
