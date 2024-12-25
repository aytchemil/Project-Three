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

    public Ability currentAbility;
    string direction;

    // Virtual property for 'Controls'
    public virtual CombatEntityController Controls { get; set; }

    bool initializedAttackTriggers;
    bool initializedBlockingTrigger;

    [Header("Blocking")]
    public BlockingTriggerCollider blockingTrigger;
    public GameObject blockingTriggerPrefab;
    public Transform blockingTriggerParent;

    protected virtual void Awake()
    {
        Controls = GetComponent<CombatEntityController>();     
    }

    #region Enable/Disable

    protected virtual void OnEnable()
    {
        //If the attack trigger parent DNE, then create it
        if (attackTriggerParent == null)
        {
            attackTriggerParent = Instantiate(new GameObject(), transform, false).transform;
            attackTriggerParent.rotation = Quaternion.identity;
            attackTriggerParent.name = "Attack Triggers Parent";
        }

        if(blockingTriggerParent == null)
        {
            blockingTriggerParent = Instantiate(new GameObject(), transform, false).transform;
            blockingTriggerParent.rotation = Quaternion.identity;
            blockingTriggerParent.name = "Blocking Trigger Parent";
        }

        //Adding methods to Action Delegates
        //Debug.Log("Combat functionaly enable");
        //UI
        Controls.SelectCertainAbility += EnableAbility;

        //Attacking
        Controls.EnterCombat += InCombat;
        Controls.ExitCombat += ExitCombat;
        Controls.CombatFollowTarget += CombatFunctionalityElementsLockOntoTarget;

        Controls.attack += UseAttackAbility;

        Controls.blockStart += Block;
        Controls.blockStop += StopBlock;

    }

    /// <summary>
    /// Remove the Action Delegates on Disable
    /// </summary>
    protected virtual void OnDisable()
    {
        //UI
        Controls.SelectCertainAbility -= EnableAbility;

        //Attacking
        Controls.EnterCombat -= InCombat;
        Controls.ExitCombat -= ExitCombat;
        Controls.CombatFollowTarget -= CombatFunctionalityElementsLockOntoTarget;

        Controls.attack -= UseAttackAbility;

        Controls.blockStart -= Block;
        Controls.blockStop -= StopBlock;
    }

    #endregion

    /// <summary>
    /// Action Delegate Method for Being "In Combat" 
    /// - Sets Locked on flag
    /// - Sets the current Ability to default (up)
    /// - Initialize the Attack triggers on the parent with the player's inputted abilities
    /// </summary>
    void InCombat()
    {
        //print("combatFunctionality: in combat");
        //Auto Set the current ability
        currentAbility = Controls.a_up;

        if (!initializedAttackTriggers)
            InstantiateAttackTriggers(Controls.a_right, Controls.a_left, Controls.a_up, Controls.a_down);

        if (!initializedBlockingTrigger)
            InitializeBlockingTrigger();

    }

    /// <summary>
    /// Action Delegate Method for exiting combat
    /// - Sets the locked flag to false
    /// - Sets the attacking flag to false for ensuring player isnt attacking
    /// - disables the attack triggers (performance)
    /// </summary>
    void ExitCombat()
    {
        //print("exiting combat");
        StopBlock(); //Must be first
        Controls.alreadyAttacking = false;
        DisableAttackTriggers();
    }

    public void CombatFunctionalityElementLockOntoTarget(CombatEntityController target, Transform elementTransform)
    {
        if (target != null)
        {
            Transform transform = elementTransform.transform;
            transform.LookAt(target.transform.position);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);

        }
    }

    #region Trigger Generation

    /// <summary>
    /// Cache the attack triggers to a List for easy referenceing
    /// </summary>
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

    /// <summary>
    /// Enables all the attack triggers
    /// </summary>
    public void EnableAttackTriggers()
    {
        if (myAttackTriggers.Any(item => item == null))
            Debug.LogError("Trying to enable Attack Triggers that are not initialized, or not avaliable");


        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(true);
    }

    /// <summary>
    /// Disables all the attack triggers
    /// </summary>
    public void DisableAttackTriggers()
    {
        //Debug.Log("Disabling Attack Triggers");
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(false);
    }

    /// <summary>
    /// Instantiates all the attack triggers on the attack trigger parent
    /// - Checks if the abilities inputted to the parameter list are null
    /// - Sets the initialized attack triggers flag to true
    /// - Initializes all the attack triggers on the parent, and caches them to this script
    /// - Cache them to the list
    /// - Set's their combat functionality reference to this script
    /// - Disables the attack triggers
    /// </summary>
    /// <param name="right"></param>
    /// <param name="left"></param>
    /// <param name="up"></param>
    /// <param name="down"></param>
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
        DisableAttackTriggers();
    }

    #endregion

    #region Combat Functionality

    /// <summary>
    /// Enables the currently selected ability
    /// </summary>
    /// <param name="dir"></param>
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
                currentAbility = Controls.a_down;
                break;
        }

    }

    /// <summary>
    /// Makes the attack triggers lock on to the target, restricted to X and Z (no Y)
    /// </summary>
    /// <param name="target"></param>
    public virtual void CombatFunctionalityElementsLockOntoTarget(CombatEntityController target)
    {
       // Debug.Log("locking onto target");
        CombatFunctionalityElementLockOntoTarget(target, attackTriggerParent);
        CombatFunctionalityElementLockOntoTarget(target, blockingTriggerParent);
    }


    /// <summary>
    /// Uses the currently selected ability
    /// </summary>
    public virtual void UseAttackAbility()
    {
        if (!Controls.isLockedOn || Controls.alreadyAttacking || Controls.isBlocking) return;

        if (currentAbility == null)
            Debug.LogError("There is currently no selected ability (a_current) that this combat functionality script can use.");

        //print("ATTEMPT ATTACK");

        StartAttacking();
        AttackTriggerUse();
        AnimationAttackLookAtBufferPeriod();

        ///to do: create a way for it to animate,
        ///create 4 different attack triggers(like box)
        /// animate all 4, integrate that


        switch(currentAbility.collisionType)
        {
            case Ability.CollisionType.Box:

                BoxAttack();

                break;
            case Ability.CollisionType.Overhead:

                OverheadAttack();

                break;
            case Ability.CollisionType.Pierce:

                PierceAttack();

                break;
            case Ability.CollisionType.SideSlash:

                SideSlashAttack();

                break;

            case Ability.CollisionType.MovementForward:

                MovementForwardAttack();

                break;
            case Ability.CollisionType.MovementRightOrLeft:

                MovementRightOrLeftAttack();

                break;


        }

    }

    void AnimationAttackLookAtBufferPeriod()
    {

    }

    /// <summary>
    /// Attack with the box attack
    /// </summary>
    void BoxAttack()
    {
        //Debug.Log("Attempting Box attack");
    }
    void OverheadAttack()
    {
        Debug.Log("Attempting Overhead attack");
    }
    void PierceAttack()
    {
       // Debug.Log("Attempting Pierce attack");
    }

    void SideSlashAttack()
    {
        //Debug.Log("Attempting Side Slash attack");
    }

    void MovementForwardAttack()
    {
        Debug.Log("Attempting Movement Forward Attack");

        gameObject.GetComponent<Movement>().Dash(new Vector2(0, 5), currentAbility.movementAmount);
    }

    void MovementRightOrLeftAttack()
    {
        Debug.Log("Attempting Movemnt left or right atack");
    }

    void MovementRightAttack()
    {
        Debug.Log("Right attack");
    }

    void MovementLeftAttack()
    {
        Debug.Log("Left attack");
    }


    /// <summary>
    /// Enables the selected direction's attack trigger and uses that attack trigger's attacktriggerattack method call with the current ability
    /// </summary>
    void AttackTriggerUse()
    {
        switch (direction)
        {
            case "right":
                attackTrigger_right.gameObject.SetActive(true);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                attackTrigger_right.AttackTriggerAttack(currentAbility);
                break;
            case "left":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(true);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                attackTrigger_left.AttackTriggerAttack(currentAbility);
                break;
            case "up":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(true);
                attackTrigger_down.gameObject.SetActive(false);
                attackTrigger_up.AttackTriggerAttack(currentAbility);
                break;
            case "down":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(true);
                attackTrigger_down.AttackTriggerAttack(currentAbility);
                break;
        }
    }
    
    /// <summary>
    /// Sets the attacking flag
    /// </summary>
    void StartAttacking()
    {
        Controls.alreadyAttacking = true;
    }

    /// <summary>
    /// sets the attacking flag
    /// </summary>
    public void FinishAttacking()
    {
        Controls.alreadyAttacking = false;
    }

    #endregion

    #region Target Death

    /// <summary>
    /// Caller for the Target's death Action Delegate on the controls
    /// </summary>
    /// <param name="target"></param>
    public void TargetDeathCaller(CombatEntityController target)
    {
        Debug.Log("Target Death Caller called for by : " + gameObject.name);
        if (Controls.TargetDeath == null)
            Debug.LogError("Target death caller null, please check subscribers to ensure theyre are some for : " + gameObject.name);

        Controls.TargetDeath?.Invoke(target);
    }


    #endregion

    void InitializeBlockingTrigger()
    {
        initializedBlockingTrigger = true;
        blockingTrigger = Instantiate(blockingTriggerPrefab, blockingTriggerParent, false).GetComponent<BlockingTriggerCollider>();
        blockingTrigger.myCombatFunctionality = this;
        blockingTrigger.gameObject.SetActive(false);
    }


    void Block()
    {
        if (!Controls.isLockedOn || Controls.alreadyAttacking) return;

        Controls.isBlocking = true;
        blockingTrigger.gameObject.SetActive(true);

    }

    void StopBlock()
    {
        if (!Controls.isLockedOn || Controls.alreadyAttacking || !Controls.isBlocking) return;

        Controls.isBlocking = false;
        blockingTrigger.gameObject.SetActive(false);

    }



}
