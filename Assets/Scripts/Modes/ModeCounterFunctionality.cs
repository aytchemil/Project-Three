using System;
using System.Collections;
using UnityEngine;

public class ModeCounterFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE_NAME { get => "Counter"; }

    public string stopBlockingBool = "stopBlocking";


    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    private void OnEnable()
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
    public override void UseModeFunctionality()
    {
        Counter();
    }

    void Counter()
    {
        print("countering");

        //Setup
        StartCountering();
        AbilityCounter ability = (AbilityCounter)cf.Controls.Mode("Counter").ability;
        cf.Controls.Mode("Counter").SetAbility(ability);

        //Trigger
        ModeTriggerGroup usingTrigger = cf.AbilityTriggerEnableUse("Counter");
        cf.Controls.Mode("Counter").trigger = usingTrigger;

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

    /// <summary>
    /// Override for finishing 1 animation in a sequence, sets the stop blocking bool to false in this case because all counters are 1 block and then something else
    /// </summary>
    /// <param name="count"></param>
    /// <param name="animName"></param>
    /// <param name="animCont"></param>
    public override void FinishedAnAnimation(int count, string animName, CharacterAnimationController animCont)
    {
        base.FinishedAnAnimation(count, animName, animCont);

        animCont.SetBool(stopBlockingBool, true);
        print("Setting bool of stop blocking to true");

    }

    /// <summary>
    /// Reset the stopBlocking bool for future use
    /// </summary>
    /// <param name="animCont"></param>
    /// <returns></returns>
    public override IEnumerator CompletedAnimationSequence(CharacterAnimationController animCont)
    {
        yield return new WaitForEndOfFrame();

        animCont.SetBool(stopBlockingBool, false);

        print("Setting bool of stop blocking to false");
    }

}
