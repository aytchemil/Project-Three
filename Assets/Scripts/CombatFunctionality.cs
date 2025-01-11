using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor.ShaderGraph;
using System.Collections.Generic;


[RequireComponent(typeof(CombatEntityController))]
public class CombatFunctionality : MonoBehaviour
{
    string dir = "";

    // Virtual property for 'Controls'
    public virtual CombatEntityController Controls { get; set; }

    bool initializedAttackTriggers;
    bool initializedBlockingTrigger;

    public bool initialAbilityUseDelayOver;
    public ParticleSystem counteredEffect;

    protected virtual void Awake()
    {
        Controls = GetComponent<CombatEntityController>();
    }

    #region Enable/Disable

    protected virtual void OnEnable()
    {
        counteredEffect.gameObject.SetActive(false);


        //Adding methods to Action Delegates
        //Debug.Log("Combat functionaly enable");
        //UI
        Controls.SelectCertainAbility += EnableAbility;

        //Attacking
        Controls.EnterCombat += InCombat;
        Controls.ExitCombat += ExitCombat;
        Controls.CombatFollowTarget += CombatFunctionalityElementsLockOntoTarget;

        Controls.useAbility += UseAbility;

        //Controls.blockStart += Block;
        //Controls.blockStop += StopBlock;

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

       // Controls.blockStart -= Block;
        //Controls.blockStop -= StopBlock;

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
        Controls.Mode("Attack").data.currentAbility = Controls.AbilitySet("Attack").up;

        foreach (ModeRuntimeData mode in Controls.modes)
            if (!mode.data.initializedTriggers)
            {
                InstantiateTriggersForMode(mode.data.abilitySet, mode.parent);
                CacheParentTriggers(mode.triggers, mode.parent);
                mode.data.initializedTriggers = true;
                DisableTriggers(true, mode);
            }

        Controls.UpdateMainTriggers();





        //if (!initializedBlockingTrigger)
        //    InitializeBlockingTrigger();

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
        //StopBlock(); //Must be first
        Controls.alreadyAttacking = false;
        DisableTriggers(false, Controls.Mode(Controls.mode));
    }

    /// <summary>
    /// Makes the attack triggers lock on to the target, restricted to X and Z (no Y)
    /// </summary>
    /// <param name="target"></param>
    public virtual void CombatFunctionalityElementsLockOntoTarget(CombatEntityController target)
    {
        // Debug.Log("locking onto target");
        CombatFunctionalityElementLockOntoTarget(target, Controls.Mode(Controls.mode).parent);
        CombatFunctionalityElementLockOntoTarget(target, Controls.Mode(Controls.mode).parent);
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

    void CacheParentTriggers(GameObject[] triggers, Transform parent)
    {
        //print("Caching triggers for " + parent.name);

        //Debug.Log("Caching attack triggers of childcount: " + attackTriggerParent.childCount);
        for (int i = 0; i < parent.childCount; i++)
            triggers[i] = parent.GetChild(i).gameObject;


        if (triggers.Any(item => item == null)) //Checks if any item just put into that list are null, if one is, then error
            Debug.LogError("Items in attack trigger not properly cached, please look");
        //else
            //print("Successfully Cached triggers for mode " + parent.name);

    }



    /// <summary>
    /// Disables all the attack triggers
    /// </summary>
    public void DisableTriggers(bool local, ModeRuntimeData mode)
    {
        //Debug.Log(gameObject.name + " | Disabling Attack All Triggers");
        if (!Controls.Mode(mode.data.modeName).triggers.Any() || Controls.Mode(mode.data.modeName).parent.childCount == 0) 
        { 
            print("triggers not setup, not disabling something that isnt there"); 
            return;
        }


        if (local)
        {
            foreach (GameObject trigger in mode.triggers)
                if (trigger.activeSelf)
                    trigger.GetComponent<ModeTriggerGroup>().DisableThisTriggerOnlyLocally();

        }
        else
            foreach (GameObject trigger in mode.triggers)
                if (trigger.activeSelf)
                    trigger.GetComponent<ModeTriggerGroup>().DisableThisTrigger();


        //print("Successfully Disabled triggers for mode: " + mode.data.modeName);

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
    public void InstantiateTriggersForMode(ModeAbilitiesSO modeAbilities, Transform parent)
    {
        //print("Instantiating triggers");

        if (modeAbilities == null)
            Debug.LogError("Mode AbilitiesSO null");

        if(modeAbilities.right == null || modeAbilities.left == null || modeAbilities.up == null || modeAbilities.down == null)
        {
            Debug.LogError("Abilities Given to Instantiate attack triggers are null:");
            Debug.LogError("right : " + modeAbilities.right);
            Debug.LogError("left : " + modeAbilities.left);
            Debug.LogError("up : " + modeAbilities.up);
            Debug.LogError("down : " + modeAbilities.down);
        }

        Controls.t_right = InitializeTrigger(modeAbilities.right, parent, "right trigger");
        Controls.t_left = InitializeTrigger(modeAbilities.left, parent, "left trigger");
        Controls.t_up = InitializeTrigger(modeAbilities.up, parent, "up trigger");
        Controls.t_down = InitializeTrigger(modeAbilities.down, parent, "down trigger");

        if(Controls.t_right == null || Controls.t_left == null || Controls.t_up  == null || Controls.t_down == null)
            Debug.LogError("Trigger group triggers not iniailized corectly, check");

        print("Successfully Instantiating triggers for mode " + parent.name);

    }


    private ModeTriggerGroup InitializeTrigger(Ability ability, Transform parent, string direction)
    {
        //print($"Initializing trigger {ability}...");

        ModeTriggerGroup trigger = null;

        if (ability == null)
        {
            Debug.LogError($"Ability for {direction} is null in ModeAbilitySO");
            return trigger;
        }

        if (ability is AttackAbility attackAbility && ability is not CounterAbility)
        {
            print("creating trigger for attack");
            trigger = Instantiate(attackAbility.triggerCollider, parent, false).GetComponent<AttackTriggerGroup>();

        }
        else if (ability is CounterAbility counterAbility)
            trigger = Instantiate(counterAbility.counterTriggerGroup, parent, false).GetComponent<CounterTriggerGroup>();

        else if (ability is BlockAbility blockAbility)
            trigger = Instantiate(blockAbility.blockTriggerCollider, parent, false).GetComponent<BlockTriggerGroup>();
        else
            Debug.LogError($"Unsupported Ability type for {direction}: {ability}");

        trigger.InitializeSelf(this, ability);

        print($"Compeleted initialization for {ability} ! ");

        return trigger;
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
        this.dir = dir;
        string m = Controls.mode;
        //print("enabling ability in dir: " + dir);

        //Reset the current ability for all the modes
        foreach (ModeRuntimeData mode in Controls.modes)
            mode.data.currentAbility = null;


        switch (dir)
        {
            case "right":
                Controls.Mode(m).data.currentAbility = Controls.AbilitySet(m).right;
               // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).right} ");
                break;
            case "left":
                Controls.Mode(m).data.currentAbility = Controls.AbilitySet(m).left;
               // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).left} ");
                break;
            case "up":
                Controls.Mode(m).data.currentAbility = Controls.AbilitySet(m).up;
               // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).up} ");
                break;
            case "down":
                Controls.Mode(m).data.currentAbility = Controls.AbilitySet(m).down;
               // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).down} ");
                break;
        }
    }



    /// <summary>
    /// Uses the currently selected ability
    /// </summary>
    public virtual void UseAbility(string mode)
    {
        //print("-> Comabt Functionality: Attempting Attack");
        if(Controls.cantUseAbility.Invoke())
            return;


        if (Controls.Mode(mode) == null)
            Debug.LogError("There is currently no selected ability (currentAttackAbility) that this combat functionality script can use.");


        if (mode == "Attack")
            Attack();
        else if (mode == "Counter")
            Counter();
        else
            Debug.LogError(gameObject.name + " | Error: No mode found to use in UseAbility(mode)");


    }

    void Attack()
    {

        //print("attacking");

        StartAttacking();

        ///to do: create a way for it to animate,
        ///create 4 different attack triggers(like box)
        /// animate all 4, integrate that

        AttackAbility attack = Controls.Mode("Attack").data.currentAbility as AttackAbility;


        switch (attack.archetype)
        {

            case AttackAbility.Archetype.Singular:

                //print("archetype: singular chosen");

                //Archetype's Functionality
                TriggerEnableToUse().StartUsingAbilityTrigger(attack, attack.initialAttackDelay[0]);

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                ArchetypeUse_SingularAttack(attack);

                break;

            case AttackAbility.Archetype.Multi_Choice:

                //print("archetype: multichoice chosen");

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                string choice = ArchetypeUse_MultiChoiceAttack(attack);

                //print("choice is : " + choice);

                if (choice == "none") break;

                //Archetype's Functionality
                TriggerEnableToUse().GetComponent<AttackTriggerMultiChoice>().MultiChoiceAttack(attack, attack.initialAttackDelay[0], choice);

                break;

            case AttackAbility.Archetype.Multi_FollowUp:

                //print("archetype: followup chosen");

                //Archetype's Functionality
                TriggerEnableToUse().GetComponent<AttackTriggerFollowUp>().StartUsingAbilityTrigger(attack, attack.initialAttackDelay[0]);

                //Find the Specific Attack this ability is using, and use this attack abilities's Functionality
                ArchetypeUse_FollowUpAttack(attack);

                break;


        }
    }

    void Counter()
    {
        //print("countering");

        StartCountering();

        CounterAbility counterAbility = Controls.Mode("Counter").data.currentAbility as CounterAbility;


        switch (counterAbility.counterArchetype)
        {
            case CounterAbility.CounterArchetype.StandingRiposte:

                TriggerEnableToUse().StartUsingAbilityTrigger(counterAbility, counterAbility.initialAttackDelay[0]);

                StandingRiposte();

                break;
        }
    }

    void StandingRiposte()
    {
        print("Counter attack: Standing riposte");

    }


    #region Attack Collision Types

    IEnumerator MovementForwardAttack(AttackAbility attack)
    {
        Debug.Log(" * MoveForwardAttacK Called");

        //print("Waiting for attack to start, initialAttackDelayOver not over yet (its false)");
        //print("initial attack delay over?: " + initialAttackDelayOver);
        while (!initialAbilityUseDelayOver)
        {
           // print("waiting...");
            yield return new WaitForEndOfFrame();
        }
       // print("Attacking started, initialAttackDelayOver is over (true)");


        gameObject.GetComponent<Movement>().Lunge("up", attack.movementAmount);
        gameObject.GetComponent<Movement>().DisableMovement();
        Invoke(nameof(ReEnableMovement), gameObject.GetComponent<Movement>().entityStates.dashTime);
    }

    #endregion

    void ReEnableMovement()
    {
        gameObject.GetComponent<Movement>().EnableMovement();
    }

    IEnumerator MovementRightOrLeftAttack(string choice, AttackAbility attack)
    {
        Debug.Log(gameObject.name + " | Combat Functionality: attacking w/ MovementLeftOrRight attack");


        gameObject.GetComponent<Movement>().Lunge(choice, attack.movementAmount);

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

    ModeTriggerGroup TriggerEnableToUse()
    {
        ModeTriggerGroup usingThisTriggerGroup = null;

        switch (dir)
        {
            case "right":
                Controls.t_right.gameObject.SetActive(true);
                Controls.t_left.gameObject.SetActive(false);
                Controls.t_up.gameObject.SetActive(false);
                Controls.t_down.gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.t_right;
                break;
            case "left":
                Controls.t_right.gameObject.SetActive(false);
                Controls.t_left.gameObject.SetActive(true);
                Controls.t_up.gameObject.SetActive(false);
                Controls.t_down.gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.t_left;
                break;
            case "up":
                Controls.t_right.gameObject.SetActive(false);
                Controls.t_left.gameObject.SetActive(false);
                Controls.t_up.gameObject.SetActive(true);
                Controls.t_down.gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.t_up;
                break;
            case "down":
                Controls.t_right.gameObject.SetActive(false);
                Controls.t_left.gameObject.SetActive(false);
                Controls.t_up.gameObject.SetActive(false);
                Controls.t_down.gameObject.SetActive(true);
                usingThisTriggerGroup = Controls.t_down;
                break;
            default:
                usingThisTriggerGroup = null;
                Debug.LogError("Havn't chosen an attack trigger group to use out of: right, left, up, down");
                break;
        }
        return usingThisTriggerGroup;
    }


    void ArchetypeUse_SingularAttack(AttackAbility attack)
    {
        switch (attack.trait)
        {
            case AttackAbility.Trait.MovementForward:

                StartCoroutine(MovementForwardAttack(attack));

                break;
        }
    }

    string ArchetypeUse_MultiChoiceAttack(AttackAbility attack)
    {
        string choice = "";

        switch (attack.trait)
        {
            case AttackAbility.Trait.MovementLeftOrRight:

                choice = Controls.getMoveDirection?.Invoke();

                if (choice == "foward" || choice == "back" || choice == "none")
                {
                    choice = "none";
                    print("Not able to use ability, critiera not met");
                    FinishAttacking();
                    break;
                }

                StartCoroutine(MovementRightOrLeftAttack(choice, attack));

                break;
        }

        return choice;
    }

    void ArchetypeUse_FollowUpAttack(AttackAbility attack)
    {
        switch (attack.trait)
        {
            case AttackAbility.Trait.DoubleFrontSlash:

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
        foreach(ModeRuntimeData mode in Controls.modes)
            DisableTriggers(false, Controls.Mode(mode.data.modeName));

    }

    #endregion


    #region Flinching

    public void Flinch(float flinchTime)
    {
       // print(this.gameObject.name + " has flinched");
        DisableTriggers(false, Controls.Mode(Controls.mode));
    }

    #endregion

    #region Countering

    public void GetCountered(Vector3 effectPos)
    {
        FinishAttacking();
        FinishCountering();
        DisableTriggers(false, Controls.Mode(Controls.mode));
        Controls.Countered?.Invoke();
        StartCoroutine(CounteredEffect(effectPos));
    }

    IEnumerator CounteredEffect(Vector3 effectPos)
    {
        counteredEffect.gameObject.transform.position = effectPos;
        counteredEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        counteredEffect.gameObject.SetActive(false);

    }




    #endregion

}
