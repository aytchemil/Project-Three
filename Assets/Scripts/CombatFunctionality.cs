using UnityEngine;
using System.Linq;
using System.Collections;


[RequireComponent(typeof(CombatEntityController))]
public class CombatFunctionality : MonoBehaviour
{
    public Transform attackTriggerParent;

    public GameObject[] myAttackTriggers = new GameObject[4];
    public AttackTriggerGroup attackTrigger_right;
    public AttackTriggerGroup attackTrigger_left;
    public AttackTriggerGroup attackTrigger_up;
    public AttackTriggerGroup attackTrigger_down;

    public CounterTriggerGroup counterTrigger_right;
    public CounterTriggerGroup counterTrigger_left;
    public CounterTriggerGroup counterTrigger_up;
    public CounterTriggerGroup counterTrigger_down;

    public AttackAbility currentAttackAbility;
    public CounterAbility currentCounterAbility;

    string direction;

    // Virtual property for 'Controls'
    public virtual CombatEntityController Controls { get; set; }

    bool initializedAttackTriggers;
    bool initializedBlockingTrigger;

    public bool initialAbilityUseDelayOver;

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

        Controls.useAbility += UseAbility;

        Controls.blockStart += Block;
        Controls.blockStop += StopBlock;

        Controls.Flinch += Flinch;

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

        Controls.useAbility -= UseAbility;

        Controls.blockStart -= Block;
        Controls.blockStop -= StopBlock;

        Controls.Flinch -= Flinch;
    }

    #endregion

    #region Delegates: InCombat(), ExitCombat(), CombatFunctionalityElementsLockOntoTarget() ,CombatFunctionalityElementLockOntoTarget()
    ///===========================================================================================================================================================================================================================================================================================

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
        currentAttackAbility = Controls.a_up;

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
        DisableAttackTriggers(false);
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

    public void CombatFunctionalityElementLockOntoTarget(CombatEntityController target, Transform elementTransform)
    {
        if (target != null)
        {
            Transform transform = elementTransform.transform;
            transform.LookAt(target.transform.position);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);

        }
    }

    #endregion

    #region Trigger Generation: CacheAttackTriggers(), EnableAttackTriggers() DisableAttackTriggers() InstantiateAttackTriggers()
    ///======================================================================================================================================================================================================================================================================================================

    #region Attack Triggers
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
    public void DisableAttackTriggers(bool local)
    {
        //Debug.Log(gameObject.name + " | Disabling Attack All Triggers");
        if (!myAttackTriggers.Any() || attackTriggerParent.childCount == 0) { print("att triggers not setup, not disabling something that isnt there"); return; }

        if (local)
        {
            foreach (GameObject attkTrigger in myAttackTriggers)
            {
                if (attkTrigger.activeSelf)
                    attkTrigger.GetComponent<AttackTriggerGroup>().DisableThisTriggerOnlyLocally();

                attkTrigger.SetActive(false);

            }
        }
        else
        {
            foreach (GameObject attkTrigger in myAttackTriggers)
            {
                if (attkTrigger.activeSelf)
                    attkTrigger.GetComponent<AttackTriggerGroup>().DisableThisTrigger();

                attkTrigger.SetActive(false);

            }
        }


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
    public void InstantiateAttackTriggers(AttackAbility right, AttackAbility left, AttackAbility up, AttackAbility down)
    {
        if(right == null ||  left == null || up == null || down == null)
        {
            Debug.LogError("Abilities Given to Instantiate attack triggers are null:");
            Debug.LogError("right : " + right);
            Debug.LogError("left : " + left);
            Debug.LogError("up : " + up);
            Debug.LogError("down : " + down);
        }

        //print(gameObject.name + " | combat functionality : initializing all attack triggers");

        initializedAttackTriggers = true;
        attackTrigger_right = Instantiate(right.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerGroup>();
        attackTrigger_left = Instantiate(left.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerGroup>();
        attackTrigger_up = Instantiate(up.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerGroup>();
        attackTrigger_down = Instantiate(down.attackTriggerCollider, attackTriggerParent, false).GetComponent<AttackTriggerGroup>();
        CacheAttackTriggers();
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.GetComponent<AttackTriggerGroup>().InitSelf(this);

        //print(gameObject.name + " | combat functionality : initialization complete");
        //print(gameObject.name + " | combat functionality : disabling all attack triggers");

        DisableAttackTriggers(true);

        //print(gameObject.name + " | combat functionality : attack triggers disabled");

    }
    #endregion

    #endregion

    #region Combat Functionality: EnableAbility(), UseAttackAbility()
    ///====================================================================================================================================================================================================

    /// <summary>
    /// Enables the currently selected ability
    /// </summary>
    /// <param name="dir"></param>
    void EnableAbility(string dir)
    {
       // print("Enabling ability on dir: " + dir);
        direction = dir;

        if(Controls.mode == "attack")
        {
            currentCounterAbility = null;
            switch (dir)
            {
                case "right":
                    currentAttackAbility = Controls.a_right;
                    break;
                case "left":
                    currentAttackAbility = Controls.a_left;
                    break;
                case "up":
                    currentAttackAbility = Controls.a_up;
                    break;
                case "down":
                    currentAttackAbility = Controls.a_down;
                    break;
            }
        }
        else if (Controls.mode == "counter")
        {
            currentAttackAbility = null;
            switch (dir)
            {
                case "right":
                    currentCounterAbility = Controls.c_right;
                    break;
                case "left":
                    currentCounterAbility = Controls.c_left;
                    break;
                case "up":
                    currentCounterAbility = Controls.c_up;
                    break;
                case "down":
                    currentCounterAbility = Controls.c_down;
                    break;
            }
        }

    }


    /// <summary>
    /// Uses the currently selected ability
    /// </summary>
    public virtual void UseAbility(string mode)
    {
        //print("-> Comabt Functionality: Attempting Attack");
        if(Controls.cantUseAbility.Invoke()) 
        { 
            //print("is unable to attack, returning"); 
            return; 
        }


        //print("-> Combat Functionality: Successfull ATTACK");

        if(mode == "attack")
        {
            if (currentAttackAbility == null)
                Debug.LogError("There is currently no selected ability (currentAttackAbility) that this combat functionality script can use.");

            Attack();
        }
        else if(mode == "counter")
        {
            if (currentCounterAbility == null)
                Debug.LogError("There is currently no selected ability (currentCounterAbility) that this combat functionality script can use.");

            Counter();
        }


    }

    void Attack()
    {

        StartAttacking();

        ///to do: create a way for it to animate,
        ///create 4 different attack triggers(like box)
        /// animate all 4, integrate that

        //print("finding archetype: " + currentAttackAbility.archetype);

        switch (currentAttackAbility.archetype)
        {

            case AttackAbility.AttackArchetype.Singular:

                //print("archetype: singular chosen");

                //Archetype's Functionality
                AttackTriggersEnableToUse().StartUsingAbilityTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                SingularAttacksUse();

                break;

            case AttackAbility.AttackArchetype.MultiChoice:

                //print("archetype: multichoice chosen");

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                string choice = GetMultiChoiceAttackChoiceAndUse();

                //print("choice is : " + choice);

                if (choice == "none") break;

                //Archetype's Functionality
                AttackTriggersEnableToUse().GetComponent<AttackTriggerMultiChoice>().MultiChoiceAttack(currentAttackAbility, currentAttackAbility.initialAttackDelay[0], choice);

                break;

            case AttackAbility.AttackArchetype.FollowUp:

                //print("archetype: followup chosen");

                //Archetype's Functionality
                StartCoroutine(AttackTriggersEnableToUse().GetComponent<AttackTriggerFollowUp>().FollowUpAttack(currentAttackAbility));

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                FollowUpAttacksUse();

                break;


        }
    }

    void Counter()
    {
        print("countering");

        StartCountering();

        switch (currentCounterAbility.counterArchetype)
        {
            case CounterAbility.CounterArchetype.StandingRiposte:

                CounterTriggersEnableToUse().StartUsingAbilityTrigger(currentCounterAbility, currentCounterAbility.startDelay);

                StandingRiposte();

                break;
        }
    }

    void StandingRiposte()
    {
        print("Counter attack: Standing riposte");

    }


    #region Attack Collision Types

    IEnumerator MovementForwardAttack()
    {
        //Debug.Log(" * MoveForwardAttacK Called");

        //print("Waiting for attack to start, initialAttackDelayOver not over yet (its false)");
        //print("initial attack delay over?: " + initialAttackDelayOver);
        while (!initialAbilityUseDelayOver)
        {
           // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
       // print("Attacking started, initialAttackDelayOver is over (true)");


        gameObject.GetComponent<Movement>().Lunge("up", currentAttackAbility.movementAmount);
        gameObject.GetComponent<Movement>().DisableMovement();
        Invoke(nameof(ReEnableMovement), gameObject.GetComponent<Movement>().entityStates.dashTime);
    }

    #endregion

    void ReEnableMovement()
    {
        gameObject.GetComponent<Movement>().EnableMovement();
    }

    IEnumerator MovementRightOrLeftAttack(string choice)
    {
        Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ MovementLeftOrRight attack");


        gameObject.GetComponent<Movement>().Lunge(choice, currentAttackAbility.movementAmount);

        print("multi attack trigger, movementatttackrightorleft : lunging in dir " + choice);

        while (!initialAbilityUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
        
    }

    IEnumerator DoubleFrontSlash()
    {
        //Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ Double Front Slash");

        while (!initialAbilityUseDelayOver)
        {
            // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
    }



    /// <summary>
    /// Enables the selected direction's attack trigger and uses that attack trigger's attacktriggerattack method call with the current ability
    /// </summary>
    /// 

    AttackTriggerGroup AttackTriggersEnableToUse()
    {
        AttackTriggerGroup usingThisAttackTriggerGroup;
        switch (direction)
        {
            case "right":
                attackTrigger_right.gameObject.SetActive(true);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisAttackTriggerGroup = attackTrigger_right;
                break;
            case "left":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(true);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisAttackTriggerGroup = attackTrigger_left;
                break;
            case "up":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(true);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisAttackTriggerGroup = attackTrigger_up;
                break;
            case "down":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(true);
                usingThisAttackTriggerGroup = attackTrigger_down;
                break;
            default:
                usingThisAttackTriggerGroup = null;
                Debug.LogError("Havn't chosen an attack trigger group to use out of: right, left, up, down");
                break;
        }
        return usingThisAttackTriggerGroup;
    }

    CounterTriggerGroup CounterTriggersEnableToUse()
    {
        CounterTriggerGroup usingThisCounterTriggerGroup;
        switch (direction)
        {
            case "right":
                attackTrigger_right.gameObject.SetActive(true);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisCounterTriggerGroup = counterTrigger_right;
                break;
            case "left":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(true);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisCounterTriggerGroup = counterTrigger_left;
                break;
            case "up":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(true);
                attackTrigger_down.gameObject.SetActive(false);
                usingThisCounterTriggerGroup = counterTrigger_up;
                break;
            case "down":
                attackTrigger_right.gameObject.SetActive(false);
                attackTrigger_left.gameObject.SetActive(false);
                attackTrigger_up.gameObject.SetActive(false);
                attackTrigger_down.gameObject.SetActive(true);
                usingThisCounterTriggerGroup = counterTrigger_down;
                break;
            default:
                usingThisCounterTriggerGroup = null;
                Debug.LogError("Havn't chosen an attack trigger group to use out of: right, left, up, down");
                break;
        }
        return usingThisCounterTriggerGroup;
    }

    void SingularAttacksUse()
    {
        switch (currentAttackAbility.collisionType)
        {
            case AttackAbility.CollisionType.MovementForward:

                StartCoroutine(MovementForwardAttack());

                break;
        }
    }

    string GetMultiChoiceAttackChoiceAndUse()
    {
        string choice = "";

        switch (currentAttackAbility.collisionType)
        {
            case AttackAbility.CollisionType.MovementLeftOrRight:

                choice = Controls.getMoveDirection?.Invoke();

                if (choice == "foward" || choice == "back" || choice == "none")
                {
                    choice = "none";
                    print("Not able to use ability, critiera not met");
                    FinishAttacking();
                    break;
                }

                StartCoroutine(MovementRightOrLeftAttack(choice));

                break;
        }

        return choice;
    }

    void FollowUpAttacksUse()
    {
        switch (currentAttackAbility.collisionType)
        {
            case AttackAbility.CollisionType.DoubleFrontSlash:

                StartCoroutine(DoubleFrontSlash());

                break;
        }
    }



    /// <summary>
    /// Sets Control's alreadyAttacking flag to TRUE
    /// </summary>
    void StartAttacking()
    {
        Controls.alreadyAttacking = true;
    }

    /// <summary>
    /// Sets Control's alreadyAttacking flag to FALSE
    /// </summary>
    public void FinishAttacking()
    {
        Controls.alreadyAttacking = false;
    }

    /// <summary>
    /// Sets Control's isCountering flag to TRUE
    /// </summary>
    void StartCountering()
    {
        Controls.isCountering = true;
    }

    /// <summary>
    /// Sets Control's isCountering flag to FALSE
    /// </summary>
    public void FinishCountering()
    {
        Controls.isCountering = false;
    }

    #endregion

    #region Target Death
    ///====================================================================================================================================================================================================

    /// <summary>
    /// Caller for the Target's death Action Delegate on the controls
    /// </summary>
    /// <param name="target"></param>
    public void TargetDeathCaller(CombatEntityController target)
    {
        Debug.Log("Target Death Caller called for by : " + gameObject.name);
        if (Controls.TargetDeath == null)
            Debug.LogError("Target death caller null, please check subscribers to ensure theyre are some for : " + gameObject.name);
        else
        {
            //Debug.Log(gameObject.name + "'s target (" + target.name + ") has died, now calling TargetDeathCaller functionality for " + gameObject.name);
        }

        DisableAllAttackTriggers();
        Controls.TargetDeath?.Invoke(target);
    }

    public void DisableAllAttackTriggers()
    {
        print("Disabling all attack triggers");
        foreach(GameObject attackTrigger in myAttackTriggers)
        {
            if(attackTrigger == null)
            {
                Debug.LogError("Attack trigger null");
            }
            attackTrigger.GetComponent<AttackTriggerGroup>().DisableThisTrigger();
        }
    }

    #endregion

    #region Blocking

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

    #endregion

    #region Flinching

    public void Flinch(float flinchTime)
    {
        print(this.gameObject.name + " has flinched");
        DisableAttackTriggers(false);
    }

    #endregion

}
