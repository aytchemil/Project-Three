using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class CombatFunctionality : MonoBehaviour
{
    //Cache
    PlayerController c;


    AttackTriggerCollider attackTrigger_current;

    bool isLockedOn;

    private void Awake()
    {
        c = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        //Debug.Log("Combat functionaly enable");
        c.attack.performed += ctx => UseAttackAbility();
        c.SelectCertainAbility += EnableAbility;
        c.EnterCombat += InCombat;


    }

    private void OnDisable()
    {
        c.attack.performed -= ctx => UseAttackAbility();
        c.SelectCertainAbility -= EnableAbility;
    }

    void EnableAbility(string dir)
    {
        switch (dir)
        {
            case "right":
                 c.a_current= c.a_right;
                    break;
            case "left":
                c.a_current = c.a_left;
                break;
            case "up":
                c.a_current = c.a_up;
                break;
            case "down":
                c.a_current = c.a_right;
                break;
        }

    }


    void UseAttackAbility()
    {
        if (!isLockedOn) return;

        print("ATTACK");


        if(c.a_current.collisionType == Ability.CollisionType.Box)
        {
            BoxAttack();
        }

    }

    

    void BoxAttack()
    {
        Debug.Log("Box attack");
    }

    void InCombat()
    {
        isLockedOn = true;
    }
    void ExitCombat()
    {
        isLockedOn = false;
    }




}
