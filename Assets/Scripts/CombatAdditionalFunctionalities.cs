using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AF
{
    public virtual string name { get; set; }
}

public class AF_movement : AF
{
    public override string name => "movement";
    public float movementAmount;

    public AF_movement(float movementAmount)
    {
        this.movementAmount = movementAmount;
    }
}

public class AF_choice: AF
{
    public override string name => "choice";
    public string choice;

    public AF_choice(string choice)
    {
        this.choice = choice;
    }
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
        MovementLeftOrRight = 5,
    }

    private void OnEnable()
    {
        Controls.UseCombatAdditionalFunctionality += UseCombatAdditionalFunctionality;
    }

    private void OnDisable()
    {
        Controls.UseCombatAdditionalFunctionality -= UseCombatAdditionalFunctionality;
    }

    void UseCombatAdditionalFunctionality(AbilityWrapper usingAbility)
    {
        switch (usingAbility.parentAbility.af)
        { 
            case Function.None:
                break;
            case Function.MovementForward:

                float moveAmount = (usingAbility.GetAF("movement") as AF_movement).movementAmount;

                StartCoroutine(MovementForwardAttack(moveAmount));
                
                break;
            case Function.MovementLeftOrRight:

                float lrmoveAmount = (usingAbility.GetAF("movement") as AF_movement).movementAmount;
                string choice = (usingAbility.GetAF("choice") as AF_choice).choice;

                StartCoroutine(MovementRightOrLeftAttack(lrmoveAmount, choice));

                break;


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
