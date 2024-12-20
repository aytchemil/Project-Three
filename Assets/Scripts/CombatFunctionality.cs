using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEditor.ShaderGraph;


[RequireComponent(typeof(CombatEntityController))]
public class CombatFunctionality : MonoBehaviour
{
    public Transform attackTriggerParent;

    public GameObject[] myAttackTriggers = new GameObject[4];
    public AttackTriggerCollider attackTrigger_right;
    public AttackTriggerCollider attackTrigger_left;
    public AttackTriggerCollider attackTrigger_up;
    public AttackTriggerCollider attackTrigger_down;

    public Ability currentAbility;
    string direction;

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

    void InCombat()
    {
        isLockedOn = true;
        //Auto Set the current ability
        currentAbility = Controls.a_up;

        if (!initializedAttackTriggers)
            InstantiateAttackTriggers(Controls.a_right, Controls.a_left, Controls.a_up, Controls.a_down);

        EnableAttackTriggers();

    }
    void ExitCombat()
    {
        isLockedOn = false;
        DisableAttackTriggers();
    }

    #region Trigger Generation

    void CacheAttackTriggers()
    {
        //Debug.Log("Caching attack triggers of childcount: " + attackTriggerParent.childCount);
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
        //Debug.Log("Disabling Attack Triggers");
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

    #endregion

    #region Combat Functionality

    void EnableAbility(string dir)
    {
        direction = dir;
        switch (dir)
        {
            case "right":
                currentAbility = Controls.a_right;
                 break;
            case "left":
                currentAbility = Controls.a_left;
                break;
            case "up":
                currentAbility = Controls.a_up;
                break;
            case "down":
                currentAbility = Controls.a_right;
                break;
        }

    }


    protected virtual void UseAttackAbility()
    {
        if (!isLockedOn) return;

        if (currentAbility == null)
            Debug.LogError("There is currently no selected ability (a_current) that this combat functionality script can use.");

        print("ATTACK");

        AttackTriggerUse();






        if (currentAbility.collisionType == Ability.CollisionType.Box)
        {
            BoxAttack();
        }

    }

    

    void BoxAttack()
    {
        Debug.Log("Box attack");
    }


    void AttackTriggerUse()
    {
        switch (direction)
        {
            case "right":
                attackTrigger_right.AttackTriggerAttack();
                break;
            case "left":
                attackTrigger_left.AttackTriggerAttack();
                break;
            case "up":
                attackTrigger_up.AttackTriggerAttack();
                break;
            case "down":
                attackTrigger_down.AttackTriggerAttack();
                break;
        }
    }

    #endregion



}
