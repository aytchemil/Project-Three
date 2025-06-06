using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using static CombatEntityController;


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

        Controls.switchAttackMode += SwitchAttackMode;

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

        Controls.switchAttackMode -= SwitchAttackMode;


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
        Controls.Mode("Attack").ability = Controls.AbilitySet("Attack").up;
        Controls.Mode("Combo").ability = Controls.AbilitySet("Combo").right;
        //up is 3
        //left is 2

        foreach (CombatEntityModeData mode in Controls.modes)
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
        Controls.Mode("Attack").isUsing = false;
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
    public void DisableTriggers(bool local, CombatEntityModeData mode)
    {
        //Debug.Log(gameObject.name + " | Disabling Attack All Triggers");
        if (!Controls.Mode(mode.data.modeName).triggers.Any() || Controls.Mode(mode.data.modeName).parent.childCount == 0)
        {
            print("triggers not setup or already disabled, not disabling something that isnt there");
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
        print("Instantiating triggers");

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


        Controls.t_right = InitializeTrigger(abilitySet.right, parent, "right trigger");
        Controls.t_left = InitializeTrigger(abilitySet.left, parent, "left trigger");
        Controls.t_up = InitializeTrigger(abilitySet.up, parent, "up trigger");
        Controls.t_down = InitializeTrigger(abilitySet.down, parent, "down trigger");

        if (Controls.t_right == null || Controls.t_left == null || Controls.t_up == null || Controls.t_down == null)
            Debug.LogError("Trigger group triggers not iniailized corectly, check");

        print("Successfully Instantiating triggers for mode " + parent.name);

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
            trigger = Instantiate(attackAbility.prefab, parent, false).GetComponent<GeneralAttackTriggerGroup>();

        else if (ability is AbilityMulti multiAbility && ability is not AbilityCounter)
            trigger = Instantiate(multiAbility.prefab, parent, false).GetComponent<MultiAttackTriggerGroup>();

        else if (ability is AbilityCounter counterAbility)
            trigger = Instantiate(counterAbility.prefab, parent, false).GetComponent<CounterTriggerGroup>();

        //else if (ability is BlockAbility blockAbility)
        //    trigger = Instantiate(blockAbility.prefab, parent, false).GetComponent<BlockTriggerGroup>();

        else if (ability is AbilityCombo comboAbility)
            trigger = Instantiate(comboAbility.prefab, parent, false).GetComponent<CombotTriggerGroup>();

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
    void EnableAbility(string dir)
    {
        this.dir = dir;
        string m = Controls.mode;
        //print("enabling ability in dir: " + dir);

        //Reset the current ability for all the modes
        foreach (CombatEntityModeData mode in Controls.modes)
            if (mode.name != "Combo")
                mode.ability = null;


        switch (dir)
        {
            case "right":
                Controls.Mode(m).ability = Controls.AbilitySet(m).right;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).right} ");
                break;
            case "left":
                Controls.Mode(m).ability = Controls.AbilitySet(m).left;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).left} ");
                break;
            case "up":
                Controls.Mode(m).ability = Controls.AbilitySet(m).up;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).up} ");
                break;
            case "down":
                Controls.Mode(m).ability = Controls.AbilitySet(m).down;
                // print($"Mode: {Controls.Mode(m)}, Ability set: {Controls.AbilitySet(m).down} ");
                break;
        }
    }



    /// <summary>
    /// Uses the currently selected ability
    /// </summary>
    public virtual void UseAbility(string mode)
    {
        print($"[Combat Functionality]: Attempting To Use an Ability from mode [{mode}]");
        if (Controls.cantUseAbility.Invoke())
            return;

        Controls.Mode(mode).data.modeFunctionality.UseModeFunctionality();

        if (Controls.Mode(mode) == null)
            Debug.LogError("There is currently no selected ability (currentAttackAbility) that this combat functionality script can use.");


    }





    #region Attack Collision Types


    #endregion



    /// <summary>
    /// Enables the selected direction's attack trigger and uses that attack trigger's attacktriggerattack method call with the current ability
    /// </summary>
    /// 

    public ModeTriggerGroup AbilityTriggerEnableUse(string mode)
    {
        ModeTriggerGroup usingThisTriggerGroup = null;

        switch (dir)
        {
            case "right":
                Controls.Mode(mode).triggers[0].gameObject.SetActive(true);
                Controls.Mode(mode).triggers[1].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[2].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[3].gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.Mode(mode).triggers[0].gameObject.GetComponent<ModeTriggerGroup>();
                break;
            case "left":
                Controls.Mode(mode).triggers[0].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[1].gameObject.SetActive(true);
                Controls.Mode(mode).triggers[2].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[3].gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.Mode(mode).triggers[1].gameObject.GetComponent<ModeTriggerGroup>();
                break;
            case "up":
                Controls.Mode(mode).triggers[0].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[1].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[2].gameObject.SetActive(true);
                Controls.Mode(mode).triggers[3].gameObject.SetActive(false);
                usingThisTriggerGroup = Controls.Mode(mode).triggers[2].gameObject.GetComponent<ModeTriggerGroup>();
                break;
            case "down":
                Controls.Mode(mode).triggers[0].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[1].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[2].gameObject.SetActive(false);
                Controls.Mode(mode).triggers[3].gameObject.SetActive(true);
                usingThisTriggerGroup = Controls.Mode(mode).triggers[3].gameObject.GetComponent<ModeTriggerGroup>();
                break;
            default:
                usingThisTriggerGroup = null;
                Debug.LogError("Havn't chosen an attack trigger group to use out of: right, left, up, down");
                break;
        }

        //print($"[Combat Functionality] Trigger Enabled to use: {usingThisTriggerGroup.name}");

        return usingThisTriggerGroup;
    }


    public ModeTriggerGroup ComboTriggerEnableUse()
    {
        ModeTriggerGroup usingThisTriggerGroup = null;
        CombatEntityModeData m = Controls.Mode("Combo");

        for(int i = 0; i < m.triggers.Length-1; i++)
            m.triggers[i].gameObject.SetActive(false);

        m.triggers[Controls.c_current].gameObject.SetActive(true);
        usingThisTriggerGroup = m.triggers[Controls.c_current].gameObject.GetComponent<ModeTriggerGroup>();

        return usingThisTriggerGroup;
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
        foreach (CombatEntityModeData mode in Controls.modes)
            DisableTriggers(false, Controls.Mode(mode.name));

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
        (Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).FinishAttacking();
        (Controls.Mode("Counter").data.modeFunctionality as ModeCounterFunctionality).FinishCountering();

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



    void SwitchAttackMode()
    {
        int currIndex = Controls.modes.IndexOf(Controls.Mode(Controls.mode));

        if (currIndex >= Controls.modes.Count - 1)
            currIndex = 0;
        else
            currIndex++;

        if (Controls.modes[currIndex].data.solo)
        {
            print("SOLO MODE DETECTED");
            if (currIndex >= Controls.modes.Count - 1)
                currIndex = 0;
            else
                currIndex++;
        }

        Controls.mode = Controls.modes[currIndex].data.modeName;


        print($"Switching Mode to {Controls.Mode(Controls.mode)}");
        Controls.UpdateMainTriggers();

        Controls.SelectCertainAbility?.Invoke(Controls.lookDir);
        //print(Controls.lookDir);


    }






}
