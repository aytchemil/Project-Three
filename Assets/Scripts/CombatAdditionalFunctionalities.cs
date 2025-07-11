using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    void UseCombatAdditionalFunctionality(Ability ability)
    {

        if(ability.AF_Dictionary.TryGetValue("movement", out AF afmove))
        {
            print($"[AF] Movement {(afmove as AF_movement).movementAmount}");
            
            StartCoroutine(MovementForwardAttack((afmove as AF_movement).movementAmount));
        }
        if (ability.AF_Dictionary.TryGetValue("choice", out AF afchoice))
        {
            print($"[AF] Choice");
            StartCoroutine(MovementRightOrLeftAttack((afmove as AF_movement).movementAmount,
                                                    (afchoice as AF_choice).choice));
        }
    }



    IEnumerator MovementForwardAttack(float moveAmount)
    {
        print($"[{gameObject.name}] [ModeAttack] COROUTINE MovementForward STARTED...");

        //print("Waiting for attack to start, initialAttackDelayOver not over yet (its false)");
        //print("initial attack delay over?: " + initialAttackDelayOver);
        while (!cf.initialAbilityUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
        // print("Attacking started, initialAttackDelayOver is over (true)");


        gameObject.GetComponent<Movement>().Lunge("up", moveAmount);
        gameObject.GetComponent<Movement>().DisableMovement();
        Invoke(nameof(ReEnableMovement), gameObject.GetComponent<Movement>().entityStates.dashTime);
    }


    IEnumerator MovementRightOrLeftAttack(float moveAmount, string choice)
    {
        Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ MovementLeftOrRight attack");

        gameObject.GetComponent<Movement>().Lunge(choice, moveAmount);

        print("multi attack trigger, movementatttackrightorleft : lunging in dir " + choice);

        while (!cf.initialAbilityUseDelayOver)
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
