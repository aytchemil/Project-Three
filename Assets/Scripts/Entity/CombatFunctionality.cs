using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using static EntityController;


[RequireComponent(typeof(EntityController))]
public class CombatFunctionality : MonoBehaviour
{
    int cur_Ability = 0;
    // Virtual property for 'Controls'
    public virtual EntityController Controls { get; set; }

    bool initializedAttackTriggers;
    bool initializedBlockingTrigger;

    protected virtual void Awake()
    {
        Controls = GetComponent<EntityController>();
    }

    #region Enable/Disable

    protected virtual void OnEnable()
    {

        //Adding methods to Action Delegates
        //Debug.Log("Combat functionaly enable");
        //UI

        for (int i = 0; i < EntityController.AMOUNT_OF_ABIL_SLOTS; i++)
            Controls.abilitySlots[i] += EnableAbility;

        Controls.TargetDeath += TargetDeath;

        //Attacking
        Controls.EnterCombat += InCombat;
        Controls.ExitCombat += ExitCombat;
        Controls.CombatFollowTarget += CombatFunctionalityElementsLockOntoTarget;

        Controls.useAbility += UseAbility;

        Controls.switchAbilityMode += SwitchAbilityMode;


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
        for(int i = 0; i < Controls.abilitySlots.Length-1; i++)
            Controls.abilitySlots[i] -= EnableAbility;

        Controls.TargetDeath -= TargetDeath;

        //Attacking
        Controls.EnterCombat -= InCombat;
        Controls.ExitCombat -= ExitCombat;
        Controls.CombatFollowTarget -= CombatFunctionalityElementsLockOntoTarget;

        Controls.useAbility -= UseAbility;

        Controls.switchAbilityMode -= SwitchAbilityMode;


        // Controls.blockStart -= Block;
        //Controls.blockStop -= StopBlock;

        Controls.Flinch -= Flinch;

    }

    #endregion

    #region Delegates
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

        //Auto Set the current ability (in this case right is 0)
        SetDefaultAbilityForAllModes();

        foreach (RuntimeModeData mode in Controls.modes)
        {
            print($"[{gameObject.name}] Initializing mode [{mode.name}]...");
            if (!mode.initializedTriggers)
            {
                InstantiateTriggersForMode(mode.data.abilitySet, mode.individualParent);
                CacheParentTriggers(mode.triggers, mode.individualParent);
                mode.initializedTriggers = true;
                DisableTriggers(true, mode);
                print($"[{gameObject.name}] Initialized mode [{mode.name}] COMPLETE");
            }
        }

        print($"[{gameObject.name}] Initialized Modes COMPLETE");

        //BlockSys: Has to be after The initialization of triggers
        SetDefaultDir();
        //print($"[{gameObject.name}] now in combat");

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
        Controls.Mode("Attack").isUsing = false;
        DisableTriggers(false, Controls.Mode(Controls.mode));
    }

    /// <summary>
    /// Makes the attack triggers lock on to the target, restricted to X and Z (no Y)
    /// </summary>
    /// <param name="target"></param>
    public virtual void CombatFunctionalityElementsLockOntoTarget(EntityController target)
    {
        // Debug.Log("locking onto target");
        CombatFunctionalityElementLockOntoTarget(target, Controls.Mode(Controls.mode).individualParent);
        CombatFunctionalityElementLockOntoTarget(target, Controls.Mode(Controls.mode).individualParent);
    }

    public void CombatFunctionalityElementLockOntoTarget(EntityController target, Transform elementTransform)
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
    public void DisableTriggers(bool local, RuntimeModeData mode)
    {
        //Debug.Log(gameObject.name + " | Disabling Attack All Triggers");
        if (!Controls.Mode(mode.data.name).triggers.Any() || Controls.Mode(mode.data.name).individualParent.childCount == 0)
        {
            //print($"{gameObject.name} triggers not setup or already disabled, not disabling something that isnt there");
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
    public void InstantiateTriggersForMode(AbilitySet abilitySet, Transform parent)
    {
        //print("Instantiating triggers");

        if (abilitySet == null)
            Debug.LogError("Mode AbilitiesSO null");

        //Null Check
        if (abilitySet.right == null || abilitySet.left == null || abilitySet.up == null || abilitySet.down == null)
        {
            Debug.LogError("Abilities Given to Instantiate attack triggers are null:");
            Debug.LogError("right : " + abilitySet.right);
            Debug.LogError("left : " + abilitySet.left);
            Debug.LogError("up : " + abilitySet.up);
            Debug.LogError("down : " + abilitySet.down);
        }



        ModeTriggerGroup one = InitializeTrigger(abilitySet.right, parent, "right trigger");
        ModeTriggerGroup two = InitializeTrigger(abilitySet.left, parent, "left trigger");
        ModeTriggerGroup three = InitializeTrigger(abilitySet.up, parent, "up trigger");
        ModeTriggerGroup four = InitializeTrigger(abilitySet.down, parent, "down trigger");

        if (one == null || two == null || three == null || four == null)
            Debug.LogError("Trigger group triggers not iniailized corectly, check");

        //print("Successfully Instantiating triggers for mode " + parent.name);

    }


    private ModeTriggerGroup InitializeTrigger(Ability ability, Transform parent, string direction)
    {
        //print($"Initializing trigger {ability}...");

        ModeTriggerGroup trigger = null;

        //Null Check
        if (ability == null)
        {
            Debug.LogError($"Ability for {direction} is null in ModeAbilitySO");
            return trigger;
        }

        if (ability is AbilityAttack attackAbility && ability is not AbilityCounter)
            trigger = Instantiate(attackAbility.ColliderPrefab, parent, false).GetComponent<GeneralAttackTriggerGroup>();

        else if (ability is AbilityMulti multiAbility && ability is not AbilityCounter)
            trigger = Instantiate(multiAbility.ColliderPrefab, parent, false).GetComponent<MultiAttackTriggerGroup>();

        else if (ability is AbilityCounter counterAbility)
            trigger = Instantiate(counterAbility.ColliderPrefab, parent, false).GetComponent<CounterTriggerGroup>();

        else if (ability is AbilityBlock blockAbility)
            trigger = Instantiate(blockAbility.ColliderPrefab, parent, false).GetComponent<BlockTriggerCollider>();

        else if (ability is AbilityCombo comboAbility)
            trigger = Instantiate(comboAbility.ColliderPrefab, parent, false).GetComponent<CombotTriggerGroup>();

        else
            Debug.LogError($"Unsupported Ability type for {direction}: {ability}");

        trigger.InitializeSelf(this, ability);

        //print($"Compeleted initialization for {ability} ! ");

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
    void EnableAbility(int num)
    {
        cur_Ability = num;
        string m = Controls.mode;
        //print("enabling ability in dir: " + dir);

        //Reset the current ability for all the modes
        //foreach (RuntimeModeData mode in Controls.modes)
        //    if (mode.name != "Combo")
         //       mode.ability = null;


        switch (cur_Ability)
        {
            case 0:
                Controls.Mode(m).ability = Controls.AbilitySet(m).right;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).right} ");
                break;
            case 1:
                Controls.Mode(m).ability = Controls.AbilitySet(m).left;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).left} ");
                break;
            case 2:
                Controls.Mode(m).ability = Controls.AbilitySet(m).up;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).up} ");
                break;
            case 3:
                Controls.Mode(m).ability = Controls.AbilitySet(m).down;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).down} ");
                break;
        }
    }


    void SetDefaultAbilityForAllModes()
    {
        foreach (RuntimeModeData mode in Controls.modes)
        {
            //print($"[{gameObject.name}] [Combat Functionality] Mode [{mode.name}] Ability [{Controls.AbilitySet(mode.name).right}] is new default ability");
            mode.ability = Controls.AbilitySet(mode.name).right;
        }
    }

    void SetDefaultDir()
    {
        Controls.lookDir = "right";
    }


    /// <summary>
    /// Uses the currently selected ability
    /// </summary>
    public virtual void UseAbility(string mode)
    {
        //print($"[{gameObject.name}] [Combat Functionality]: Attempting To Use an Ability from mode [{mode}]");
        if (Controls.cantUseAbility)
        {
            print($"[{gameObject.name}] [CF] : CANT USE ABILITY ");
            return;
        }

        //print($"[{gameObject.name}] [CF] : Ability ({mode}) used.");
        Controls.Mode(mode).data.modeFunctionality.UseModeFunctionality();

        if (Controls.Mode(mode) == null)
            Debug.LogError("There is currently no selected ability (currentAttackAbility) that this combat functionality script can use.");


    }





    /// <summary>
    /// Enables the selected direction's attack trigger and uses that attack trigger's attacktriggerattack method call with the current ability
    /// </summary>
    /// 
    public ModeTriggerGroup AbilityTriggerEnableUse(string modeName)
    {
        ModeTriggerGroup usingThisTriggerGroup = null;
        RuntimeModeData mode = Controls.Mode(modeName);

        //Set all triggers of this mode to false
        for (int i = 0; i < mode.triggers.Length; i++)
            mode.triggers[i].gameObject.SetActive(false);;

        //Enable the trigger we are using
        mode.triggers[cur_Ability].gameObject.SetActive(true);

        //Set this as the return
        usingThisTriggerGroup = mode.triggers[cur_Ability].GetComponent<ModeTriggerGroup>();


        //print($"[Combat Functionality] Trigger Enabled to use: {usingThisTriggerGroup.name}");

        return usingThisTriggerGroup;
    }

    public ModeTriggerGroup WheelTriggerEnableUse(string modeName)
    {
        //print("Using Wheel Trigger... Enabling it");
        ModeTriggerGroup usingThisTriggerGroup = null;
        RuntimeModeData m = Controls.Mode(modeName);
        int triggerIndx = GetDirIndex(Controls.lookDir);

        for (int i = 0; i < m.triggers.Length - 1; i++)
            m.triggers[i].gameObject.SetActive(false);

        m.triggers[triggerIndx].gameObject.SetActive(true);
        usingThisTriggerGroup = m.triggers[triggerIndx].gameObject.GetComponent<ModeTriggerGroup>();

        return usingThisTriggerGroup;
    }

    public Ability ChooseCurrentWheelAbility(string dir, string mode)
    {
        Ability ret = null;

        if (dir == "right")
            ret = Controls.Mode(mode).data.abilitySet.right;

        else if (dir == "left")
            ret = Controls.Mode(mode).data.abilitySet.left;

        else if (dir == "up")
            ret = Controls.Mode(mode).data.abilitySet.up;

        else if (dir == "down")
            ret = Controls.Mode(mode).data.abilitySet.down;

        return ret;
    }

    public int GetDirIndex(string dir)
    {
        //print($"[{gameObject.name}] Get Dir: [{dir}]");
        if (dir == "right")
            return 0;
        else if (dir == "left")
            return 1;
        else if (dir == "up")
            return 2;
        else if (dir == "down")
            return 3;
        else
        {
            return 0;
        }
    }




    #endregion

    #region Target Death
    ///====================================================================================================================================================================================================

    /// <summary>
    /// Caller for the Target's death Action Delegate on the controls
    /// </summary>
    /// <param name="target"></param>
    public void TargetDeath(EntityController target)
    {
        DisableAllAttackTriggers();
    }

    public void DisableAllAttackTriggers()
    {
        //print("Disabling all attack triggers");
        foreach (RuntimeModeData mode in Controls.modes)
            DisableTriggers(false, Controls.Mode(mode.name));

    }

    #endregion


    #region Flinching

    public void Flinch(float flinchTime)
    {
        //print(this.gameObject.name + " has flinched");
        DisableAllAttackTriggers();
    }

    #endregion

    #region Countering

    public void GetCountered(Vector3 effectPos)
    {
        (Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).FinishAttacking();
        (Controls.Mode("Counter").data.modeFunctionality as ModeCounterFunctionality).FinishCountering();

        DisableTriggers(false, Controls.Mode(Controls.mode));
        Controls.Countered?.Invoke();
    }



    #endregion



    void SwitchAbilityMode()
    {
        int currIndex = Controls.modes.IndexOf(Controls.Mode(Controls.mode));

        if (currIndex >= Controls.modes.Count - 1)
            currIndex = 0;
        else
            currIndex++;

        while (!Controls.modes[currIndex].data.isStance)
        {
            print("SOLO MODE DETECTED");
            if (currIndex >= Controls.modes.Count - 1)
                currIndex = 0;
            else
                currIndex++;
        }

        Controls.mode = Controls.modes[currIndex].data.name;


        print($"Switching Mode to {Controls.Mode(Controls.mode)}");

        Controls.abilitySlots[currIndex]?.Invoke(0);
        //print(Controls.lookDir);


    }

    public virtual System.Enum[] GetAnimEnums(AbilityMulti ability)
    {
        System.Enum[] Enums = new System.Enum[ability.abilities.Length];
        for (int i = 0; i < ability.abilities.Length; i++)
        {
            AbilityAttack abilityi = ((AbilityAttack)ability.abilities[i]);
            System.Enum _enum = abilityi.Attack;
            Enums[i] = _enum;
        }
        return Enums;
    }


}
