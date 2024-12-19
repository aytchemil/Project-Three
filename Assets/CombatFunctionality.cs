using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ControllsHandler))]
public class CombatFunctionality : MonoBehaviour
{
    //Cache
    ControllsHandler c;

    bool isLockedOn;

    private void Awake()
    {
        c = GetComponent<ControllsHandler>();
    }

    private void OnEnable()
    {
        //Debug.Log("Combat functionaly enable");
        c.attack.performed += ctx => UseAttackAbility();
        c.UtilizeAbility += EnableAbility;
        c.EnterCombat += InCombat;


    }

    private void OnDisable()
    {
        c.attack.performed -= ctx => UseAttackAbility();
        c.UtilizeAbility -= EnableAbility;
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
