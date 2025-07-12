using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class AF 
{
    public virtual string afname { get; set; }
}





public class CombatAdditionalFunctionalities : MonoBehaviour
{
    CombatFunctionality cf;
    CombatEntityController Controls;

    private void Awake()
    {
        cf = GetComponent<CombatFunctionality>();
        Controls = GetComponent<CombatEntityController>();
    }
    public enum Function
    {
        None = 0,
        MovementForward = 4,
        ChoiceMoveLeftOrRight = 5,
    }

    private void OnEnable()
    {
        Controls.UseCombatAdditionalFunctionality += UseCombatAdditionalFunctionality;
    }

    private void OnDisable()
    {
        Controls.UseCombatAdditionalFunctionality -= UseCombatAdditionalFunctionality;
    }

    void UseCombatAdditionalFunctionality(ModeTriggerGroup trigger)
    {
        bool choice = false;
        bool move = false;

        print("[AF] TRIGGER: " + trigger + "ABILITY: " + trigger.Ability());

        if (trigger.Ability().AF_Dictionary.TryGetValue("movement", out AF afmove))
        {
            print($"[AF] Movement");
            move = true;
        }
        if (trigger.Ability().AF_Dictionary.TryGetValue("choice", out AF afchoice))
        {
            print($"[AF] Choice");
            choice = true;
        }

        if(move && !choice)
            StartCoroutine(MovementForwardAttack(trigger, (afmove as AF_movement).movementAmount));

        if(move && choice)
            StartCoroutine(MovementRightOrLeftAttack(trigger, (afmove as AF_movement).movementAmount,
                                                              (afchoice as AF_choice).choice));

    }



    IEnumerator MovementForwardAttack(ModeTriggerGroup trigger, float moveAmount)
    {
        print($"[{gameObject.name}] [ModeAttack] COROUTINE MovementForward STARTED...");

        //print("Waiting for attack to start, initialAttackDelayOver not over yet (its false)");
        //print("initial attack delay over?: " + initialAttackDelayOver);
        while (!trigger.initialUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
        // print("Attacking started, initialAttackDelayOver is over (true)");


        gameObject.GetComponent<Movement>().Lunge("up", moveAmount);
        gameObject.GetComponent<Movement>().DisableMovement();
        Invoke(nameof(ReEnableMovement), gameObject.GetComponent<Movement>().entityStates.dashTime);
    }


    IEnumerator MovementRightOrLeftAttack(ModeTriggerGroup trigger, float moveAmount, string choice)
    {
        Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ MovementLeftOrRight attack");

        gameObject.GetComponent<Movement>().Lunge(choice, moveAmount);

        print("multi attack trigger, movementatttackrightorleft : lunging in dir " + choice);

        while (!trigger.initialUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }

    }


    void ReEnableMovement()
    {
        gameObject.GetComponent<Movement>().EnableMovement();
    }


}
