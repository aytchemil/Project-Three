using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(CombatEntityController))]
public class CombatFunctionality : MonoBehaviour
{
    public Transform attackTriggerParent;

    public GameObject[] myAttackTriggers = new GameObject[4];
    public AttackTriggerCollider attackTrigger_right;
    public AttackTriggerCollider attackTrigger_left;
    public AttackTriggerCollider attackTrigger_up;
    public AttackTriggerCollider attackTrigger_down;

    // Virtual property for 'Controls'
    protected virtual CombatEntityController Controls { get; set; }

    bool initializedAttackTriggers;
    bool isLockedOn;
    public bool inRange;

    protected virtual void Awake()
    {
        Controls = GetComponent<CombatEntityController>();     
    }

    protected virtual void OnEnable()
    {
        if (attackTriggerParent == null)
        {
            attackTriggerParent = Instantiate(new GameObject(), transform, false).transform;
            attackTriggerParent.name = "Attack Triggers Parent";
        }

        //Debug.Log("Combat functionaly enable");
        Controls.SelectCertainAbility += EnableAbility;
        Controls.EnterCombat += InCombat;
        Controls.ExitCombat += ExitCombat;
    }

    protected virtual void OnDisable()
    {
        Controls.SelectCertainAbility -= EnableAbility;
        Controls.EnterCombat -= InCombat;
        Controls.ExitCombat -= ExitCombat;
    }


    void CacheAttackTriggers()
    {
        Debug.Log("Caching attack triggers of childcount: " + attackTriggerParent.childCount);
        for (int i = 0; i < attackTriggerParent.childCount; i++)
        {
            myAttackTriggers[i] = attackTriggerParent.GetChild(i).gameObject;
        }

        if (myAttackTriggers.Any(item => item == null))
            Debug.LogError("Items in attack trigger not properly cached, please look");
    }

    public void EnableAttackTriggers()
    {
        if (myAttackTriggers.Any(item => item == null))
            Debug.LogError("Trying to enable Attack Triggers that are not initialized, or not avaliable");


        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(true);
    }

    public void DisableAttackTriggers()
    {
        Debug.Log("Disabling Attack Triggers");
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(false);
    }

    public void InstantiateAttackTriggers(Ability right, Ability left, Ability up, Ability down)
    {
        if(right == null ||  left == null || up == null || down == null)
        {
            Debug.LogError("Abilities Given to Instantiate attack triggers are null:");
            Debug.LogError("right : " + right);
            Debug.LogError("left : " + left);
            Debug.LogError("up : " + up);
            Debug.LogError("down : " + down);
        }


        initializedAttackTriggers = true;
        attackTrigger_right = Instantiate(right.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_left = Instantiate(left.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_up = Instantiate(up.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_down = Instantiate(down.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerCollider>();
        CacheAttackTriggers();
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.GetComponent<AttackTriggerCollider>().combatFunctionality = this;
    }

    void EnableAbility(string dir)
    {
        switch (dir)
        {
            case "right":
                Controls.a_current= Controls.a_right;
                 break;
            case "left":
                Controls.a_current = Controls.a_left;
                break;
            case "up":
                Controls.a_current = Controls.a_up;
                break;
            case "down":
                Controls.a_current = Controls.a_right;
                break;
        }

    }


    protected virtual void UseAttackAbility()
    {
        if (!isLockedOn) return;

        if (Controls.a_current == null)
            Debug.LogError("There is currently no selected ability (a_current) that this combat functionality script can use.");

        print("ATTACK");


        if(Controls.a_current.collisionType == Ability.CollisionType.Box)
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

        if (!initializedAttackTriggers)
            InstantiateAttackTriggers(Controls.a_right, Controls.a_left, Controls.a_up, Controls.a_down);

        EnableAttackTriggers();


    }
    void ExitCombat()
    {
        isLockedOn = false;
        DisableAttackTriggers();
    }




}
