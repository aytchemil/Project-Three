using System.Collections;
using UnityEngine;

public class ModeCounterFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE_NAME { get => "Attack"; }

    public string stopBlockingBool = "stopBlocking";


    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    public override void UseModeFunctionality() => Counter();
    void Counter()
    {
        print("countering");

        //Setup
        StartCountering();
        AbilityCounter ability = (AbilityCounter)cf.Controls.Mode("Counter").ability;
        cf.Controls.Mode("Counter").SetAbility(ability);

        //Trigger
        ModeTriggerGroup usingTrigger = cf.AbilityTriggerEnableUse();
        //Ability
        AbilityWrapper usingAbility = new((ability as AbilityCounter).abilities, ability);

        switch (ability.counterArchetype)
        {
            case AbilityCounter.CounterArchetype.StandingRiposte:

                //Setup
                usingAbility.completedAnimation = usingTrigger.GetComponent<CounterTriggerGroup>().triggerProgress;

                //Animation
                StartCoroutine(AnimateFollowUpAbilities(usingAbility, usingTrigger, cf.Controls.Mode("Counter"), cf.Controls.animController));

                //Trigger
                usingAbility.Values.Add(usingTrigger.StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]));

                //Additional Functionality 
                StandingRiposte(ability);

                break;
        }
    }


    /// <summary>
    /// Sets Control's isCountering flag to TRUE
    /// </summary>
    void StartCountering()
    {
        cf.Controls.Mode("Counter").isUsing = true;
    }

    /// <summary>
    /// Sets Control's isCountering flag to FALSE
    /// </summary>
    public void FinishCountering()
    {
        cf.Controls.Mode("Counter").isUsing = false;
    }

    void StandingRiposte(AbilityCounter ability)
    {
        print("Counter attack: Standing riposte");

    }

    public override void FinishedAnAnimation(int count, string animName, CharacterAnimationController animCont)
    {
        base.FinishedAnAnimation(count, animName, animCont);

        animCont.SetBool(stopBlockingBool, true);
        print("Setting bool of stop blocking to true");

    }

    public override IEnumerator CompletedAnimationSequence(CharacterAnimationController animCont)
    {
        yield return new WaitForEndOfFrame();

        animCont.SetBool(stopBlockingBool, false);

        print("Setting bool of stop blocking to false");
    }

}
