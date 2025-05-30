using UnityEngine;
using System;
using UnityEngine.InputSystem;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
public class CombatEntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay

    [Header("Controls")]
    public Func<Vector2> look;
    public Func<Vector2> move;
    public Action sprintStart;
    public Action sprintStop;
    public Action lockOn;
    public Action dash;
    public Action<string> useAbility;
    public Action<string> useCombo;
    public Action blockStart;
    public Action blockStop;
    public Action switchAttackMode;
    public string mode = "Attack";
    public string lookDir;

    //Combo Reattacking
    public Action<float> comboReattackDelay;
    public bool waitingToReattack;
    public bool didReattack = false;
    public bool reattackChecking = false;

    [Header("Observer Events")]
    public Func<CombatEntityController> GetTarget;
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<CombatEntityController> CombatFollowTarget;
    public Action<string> SelectCertainAbility;
    public Action<CombatEntityController> TargetDeath;
    public Action ResetAttack;
    public Action MissedAttack;
    public Action CompletedAttack;
    public Action Countered;
    public Action<float> Flinch;
    public Func<string> getMoveDirection;

    [Header("Modes")]
    public List<ModeRuntimeData> modes = new List<ModeRuntimeData>();

    [Header("Mode Inputted Ability Sets")]
    public List<AbilitySet> abilitySetInputs;

    [Header("Combo Choice")]
    public ModeRuntimeData comboMode;
    public int c_current;
    public Action<int> comboOne;
    public Action<int> comboTwo;
    public Action<int> comboThree;
    public Action<int> comboFour;

    [Header("Weapon System")]
    public Weapon currentWeapon;

    [Header("Main Current Ability Sets")]
    public ModeTriggerGroup t_right;
    public ModeTriggerGroup t_left;
    public ModeTriggerGroup t_up;
    public ModeTriggerGroup t_down;
    [Space]

    [Header("Central Flags")]
    public Func<bool> cantUseAbility;
    public bool dashing;
    public bool dashOnCooldown;
    public bool isLockedOn;
    public bool alreadyAttacking;
    public bool isBlocking;
    public bool currentlyRetargetting;
    public bool isAlive = true;
    public bool isFlinching = false;
    public bool isCountering;

    [Header("Combat Flags")]
    public bool targetIsDodging;

    Transform modeParent;
    bool modesInitialized = false;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        //print($"{gameObject.name} onEnable()");
        cantUseAbility = () => (!isLockedOn || alreadyAttacking || isBlocking || isFlinching || isCountering);
        CreateMyOwnModeInstances();
    }

    protected virtual void OnEnable()
    {
        cantUseAbility = () => (!isLockedOn || alreadyAttacking || isBlocking || isFlinching || isCountering);
        useCombo += ReAttackCheck;
    }

    protected virtual void OnDisable()
    {
        look = null;
        move = null;
        sprintStart = null;
        sprintStop = null;
        lockOn = null;
        dash = null;
        useAbility = null;
        useCombo = null;
        blockStart = null;
        blockStop = null;
        GetTarget = null;
        cantUseAbility = null;
        comboOne = null;
        comboTwo = null;
        comboThree = null;
        comboFour = null;
    }

    void CreateMyOwnModeInstances()
    {
        if (modesInitialized) return;

        modes.Clear();

        if (modeParent == null)
            modeParent = Instantiate(new GameObject(), transform, false).transform;
        modeParent.name = "Mode Parent";

        int i = 0;

        //print(ModeManager.instance);
        //print(ModeManager.instance.modes);

        //Take in all the modes
        foreach (ModeData template in ModeManager.instance.modes)
        {
            //print("Copying template " + template.modeName + " on iteration " + i);
            //Create new tempalte data
            ModeData newModeData = ScriptableObject.CreateInstance<ModeData>();
            newModeData.modeName = template.modeName;
            newModeData.UIIndicator = template.UIIndicator;
            newModeData.modeTextDesc = template.modeTextDesc;
            newModeData.solo = template.solo;

            //Create NEW MODE runtime wrapper, put the data in the wrapper
            GameObject modeWrapper = Instantiate(new GameObject(), modeParent, false);


            ModeRuntimeData newMode = modeWrapper.AddComponent<ModeRuntimeData>();
            modeWrapper.transform.parent = modeParent;
            newMode.data = newModeData;
            newMode.name = "Mode: " + newMode.data.modeName;

            //print($"newmode modeName is {newMode.data.name}");

            modes.Add(newMode);
            i++;
        }


        AssignAbilitySetsToModeData();
        InstantiateModeParents();

        modesInitialized = true;
        print($"{gameObject.name} Modes initialized");
    }

    void AssignAbilitySetsToModeData()
    {
        foreach (ModeRuntimeData myMode in modes)
            myMode.data.abilitySet = AbilitySet(myMode.data.modeName);

    }

    void InstantiateModeParents()
    {
        foreach (ModeRuntimeData mode in modes)
            if (mode.parent == null)
            {
               // print($"Assigning parent for {mode.data.modeName} ");
                mode.parent = Instantiate(new GameObject(), transform, false).transform;
                mode.parent.rotation = Quaternion.identity;
                mode.parent.name = mode.data.modeName + " Triggers Parent";
            }
    }

    public ModeRuntimeData Mode(string name)
    {
        ModeRuntimeData retMode = null;

        foreach(ModeRuntimeData mode in modes)
        {
            print($"[{gameObject.name}] Mode check comparing {mode.data.modeName} against {name}");

            if (mode.data.modeName == name)
                retMode = mode;
        }

        if(modes.Count == 0)
            Debug.LogError($"[{gameObject.name}] No modes detected ");

        if (retMode == null)
            Debug.LogError($"[{gameObject.name}] Could not find a mode of name [{name}] please change it");

        return retMode;
    }

    public AbilitySet AbilitySet(string modeName)
    {
        AbilitySet retAbilitySet = null;

        foreach (AbilitySet abilitySet in abilitySetInputs)
        {
            //print(gameObject.name + "Ability Set Search: " + abilitySet.mode.ToString() + " against " + modeName);
            if (abilitySet.mode.ToString() == modeName)
                retAbilitySet = abilitySet;
        }


        if (retAbilitySet == null)
            Debug.LogError(gameObject.name + $"Ability Set Search: Did not find an ability set with the modename {modeName}");


        return retAbilitySet;
    }

    public void UpdateMainTriggers()
    {
        //print("going to " + controls.mode);
        t_right = Mode(mode).triggers[0].GetComponent<ModeTriggerGroup>();
        t_left = Mode(mode).triggers[1].GetComponent<ModeTriggerGroup>();
        t_up = Mode(mode).triggers[2].GetComponent<ModeTriggerGroup>();
        t_down = Mode(mode).triggers[3].GetComponent<ModeTriggerGroup>();
    }


    public void ReAttackCheck(string dir)
    {
        print("Reattack check");
        if (waitingToReattack)
            didReattack = true;
        else
            didReattack = false;
    }

    IEnumerator DelayReAttackCheck()
    {
        yield return new WaitForSeconds(0.05f);
        if (waitingToReattack)
            didReattack = true;
        else
            didReattack = false;
    }



}
