using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
/// 
public class CombatEntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public static int AMOUNT_OF_ABIL_SLOTS = 4;

    [Header("Controls")]
    public Func<Vector2> look;
    public Func<Vector2> move;
    public Action sprintStart;
    public Action sprintStop;
    public Action lockOn;
    public Action dash;
    public Action<string> useAbility;
    public bool usedCombo;
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
    public Action<string> ComboWheelSelectCombo;
    public Action<CombatEntityController> TargetDeath;
    public Action ResetAttack;
    public Action MissedAttack;
    public Action CompletedAttack;
    public Action Countered;
    public Action<float> Flinch;
    public Func<string> getMoveDirection;

    [System.Serializable]
    public class CombatEntityModeData
    {
        public static int AMOUNT_OF_TRIGGERS = 4;

        public Ability ability;
        public string name;
        public bool isUsing;
        public ModeTemplate data;
        public Transform parent;
        public GameObject[] triggers = new GameObject[AMOUNT_OF_TRIGGERS];

        void AssignMyData(ModeTemplate data)
        {
            this.data = data;
        }

        public CombatEntityModeData(string _mode)
        {
            ability = null;
            name = _mode.Replace("Mode: ", "");
        }

        public void SetAbility(Ability _ability)
        {
            ability = _ability;
        }

        public string GetMode()
        {
            return name;
        }
    }



    [Header("Modes and Data")]
    public List<CombatEntityModeData> modes;

    [Header("Mode Inputted Ability Sets")]
    public List<AbilitySet> abilitySetInputs;

    [Header("Ability Choice")]
    public ModeRuntimeData comboMode;
    public string c_current;
    public Action<int>[] abilitySlots = new Action<int>[AMOUNT_OF_ABIL_SLOTS];

    [Header("Weapon System")]
    public Weapon currentWeapon;

    [Header("Animation System")]
    public CharacterAnimationController animController;
    [Space]

    [Header("Central Flags")]
    public Func<bool> cantUseAbility;
    public bool dashing;
    public bool dashOnCooldown;
    public bool isLockedOn;
    public bool currentlyRetargetting;
    public bool isAlive = true;
    public bool isFlinching = false;
    public bool isBlocking;


    [Header("Combat Flags")]
    public bool targetIsDodging;

    Transform modeParent;
    bool modesInitialized = false;

    protected virtual void Awake()
    {
        abilitySlots = new Action<int>[AMOUNT_OF_ABIL_SLOTS];
    }

    protected virtual void Start()
    {
        //print($"{gameObject.name} onEnable()");
        cantUseAbility = () => (!isLockedOn || Mode("Attack").isUsing || isBlocking || isFlinching || Mode("Counter").isUsing);
        CreateMyOwnModeInstances();
    }

    protected virtual void OnEnable()
    {
        cantUseAbility = () => (!isLockedOn || Mode("Attack").isUsing || isBlocking || isFlinching || Mode("Counter").isUsing);
        useAbility += (input) => 
        {
            if (usedCombo)
            {
                ReAttackCheck(input);
                usedCombo = false;
            }
        };

        UpdateAnimationManagersWeapon();

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
        blockStart = null;
        blockStop = null;
        GetTarget = null;
        cantUseAbility = null;
        ComboWheelSelectCombo = null;
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
        foreach (ModeTemplate info in ModeManager.instance.modes)
        {
            //print("Copying template " + template.modeName + " on iteration " + i);

            //Create new SO Template with inputted data from the ModeManager (External)
            ModeTemplate template = ScriptableObject.CreateInstance<ModeTemplate>();
            template.modeName = info.modeName;
            template.UIIndicator = info.UIIndicator;
            template.modeTextDesc = info.modeTextDesc;
            template.solo = info.solo;

            //Create new ModeData, insert template data into new mode
            CombatEntityModeData newMode = new CombatEntityModeData(template.modeName);
            newMode.data = template;

            //Create a GameObject under the mode parent to organize all modes
            newMode.parent = Instantiate(new GameObject(), modeParent, false).transform;
            newMode.parent.gameObject.name = newMode.name;


            //ModeFunctionality System
            ModeGeneralFunctionality modeFunctionality = gameObject.AddComponent(ModeManager.instance.ReturnModeFunctionality(template.modeName)) as ModeGeneralFunctionality;
            newMode.data.modeFunctionality = modeFunctionality;

            //print($"newmode modeName is {newMode.data.name}");

            modes.Add(newMode);


            i++;
        }

        //Current Ability Being Used For Each Mode System
        //currentModeData = new CombatEntityModeData[modes.Count];
        //for (int j = 0; j < modes.Count; j++)
        //    currentModeData[j] = new CombatEntityModeData(modes[j].name, false);



        AssignAbilitySetsToModeData();
        InstantiateModeParents();

        modesInitialized = true;
        print($"{gameObject.name} Modes initialized");
    }


    void AssignAbilitySetsToModeData()
    {
        foreach (CombatEntityModeData myMode in modes)
            myMode.data.abilitySet = AbilitySet(myMode.data.modeName);

    }

    void InstantiateModeParents()
    {
        foreach (CombatEntityModeData mode in modes)
            if (mode.parent == null)
            {
               // print($"Assigning parent for {mode.data.modeName} ");
                mode.parent = Instantiate(new GameObject(), transform, false).transform;
                mode.parent.rotation = Quaternion.identity;
                mode.parent.name = mode.data.modeName + " Triggers Parent";
            }
    }

    public CombatEntityModeData Mode(string mode)
    {
        foreach(CombatEntityModeData _mode in modes)
            if(_mode.name == mode)
                return _mode;

        if (modes.Count == 0)
            Debug.LogError($"[{gameObject.name}] No modes detected ");


        Debug.LogError($"[CombatEntityController : {gameObject.name}] Could not find a mode of the name [{mode}] in currentModeData");
        throw new InvalidOperationException("Cannot return a ref to a new struct instance.");
    }

    public CombatEntityModeData CurMode()
    {
        return Mode(mode);
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

    void UpdateAnimationManagersWeapon()
    {
        if(animController != null)
            animController.weapon = currentWeapon;
    }

}
