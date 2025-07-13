using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor.SceneManagement;
/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
/// 
public class EntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public static int AMOUNT_OF_ABIL_SLOTS = 4;

    [Header("Controls")]
    public Func<Vector2> look;
    public Func<Vector2> move;
    public Action<Vector2> moveDirInput;
    public enum MoveDirections
    {
        NONE = 0,
        RIGHT = 1,
        LEFT = 2,
        FORWARD = 3,
        BACK = 4,
        JUMP = 5
    }
    public Action<MoveDirections, int> MoveDirection;
    public Action sprintStart;
    public Action sprintStop;
    public Action lockOn;
    public Action dash;
    public Action<string> useAbility; //param: ?
    public bool usedCombo;
    public Action switchAbilityMode;
    public string mode = "Attack";
    [SerializeField] private string _lookDir;
    public string lookDir
    {
        get => _lookDir;

        set
        {
            if(_lookDir != value)
                StartCoroutine(Wait(value));
        }


    }

    IEnumerator Wait(string value)
    {
        while(cantUseAbility)
            yield return new WaitForEndOfFrame();

        _lookDir = value;
        CombatWheelSelectDirection?.Invoke(lookDir);
    }

    //Combo Reattacking
    public Action<float> comboReattackDelay;
    public bool waitingToReattack;
    public bool didReattack = false;
    public bool reattackChecking = false;

    [Header("Observer Events")]
    public Func<EntityController> GetTarget;
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<EntityController> CombatFollowTarget;
    public Action<string> CombatWheelSelectDirection; //param: dir
    public Action<EntityController> TargetDeath;
    public Action blockStart;
    public Action blockStop;
    public Action ResetAttack;
    public Action MissedAttack;
    public Action CompletedAttack;
    public Action<string> BlockedAttack; //param: dir
    public Action<string, Ability> MyAttackWasBlocked; //param dir
    public Action Countered;
    public Action<float> Flinch; //param: flinchTime
    public Func<string> getMoveDirection; //ret: moveDir
    public Action Init;
    public Action<ModeTriggerGroup> UseCombatAdditionalFunctionality;
    public Action Death;


    [System.Serializable]
    public class RuntimeModeData
    {
        public static int AMOUNT_OF_TRIGGERS = 4;

        public Ability ability;
        public ModeTriggerGroup trigger;
        public string name;
        public bool isUsing;
        public ModeTemplate data;
        public Transform parent;
        public GameObject[] triggers = new GameObject[AMOUNT_OF_TRIGGERS];

        void AssignMyData(ModeTemplate data)
        {
            this.data = data;
        }

        public RuntimeModeData(string _mode)
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
    public List<RuntimeModeData> modes;

    [Header("Mode Inputted Ability Sets")]
    public List<AbilitySet> abilitySetInputs;

    [Header("Ability Choice")]
    public ModeRuntimeData comboMode;
    public Action<int>[] abilitySlots = new Action<int>[AMOUNT_OF_ABIL_SLOTS];

    [Header("Weapon System")]
    public Weapon currentWeapon;

    [Header("Animation System")]
    public CharacterAnimationController animController;


    [Header("Central Flags")]
    [SerializeField]
    public bool cantUseAbility => (!isLockedOn || Mode("Attack").isUsing || isFlinching || Mode("Counter").isUsing);
    public bool dashing;
    public bool dashOnCooldown;
    public bool isLockedOn;
    public bool currentlyRetargetting;
    public bool isAlive = true;
    public bool isFlinching = false;


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
        CreateMyOwnModeInstances();
        ResetAttack += ResetmyAttack;
        Init?.Invoke();
    }

    protected virtual void OnEnable()
    {
        useAbility += (input) => 
        {
            if (usedCombo)
            {
                ReAttackCheck(input);
                usedCombo = false;
            }
        };

        Flinch += BaseFlinch;
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
        CombatWheelSelectDirection = null;
        Flinch -= BaseFlinch;
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
            template.isStance = info.isStance;

            //Create new ModeData, insert template data into new mode
            RuntimeModeData newMode = new RuntimeModeData(template.modeName);
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
        //currentModeData = new RuntimeModeData[modes.Count];
        //for (int j = 0; j < modes.Count; j++)
        //    currentModeData[j] = new RuntimeModeData(modes[j].name, false);



        AssignAbilitySetsToModeData();
        InstantiateModeParents();

        modesInitialized = true;
        print($"[{gameObject.name}] Copied Modes from ModeManager COMPLETED");
    }


    void AssignAbilitySetsToModeData()
    {
        foreach (RuntimeModeData myMode in modes)
            myMode.data.abilitySet = AbilitySet(myMode.data.modeName);

    }

    void InstantiateModeParents()
    {
        foreach (RuntimeModeData mode in modes)
            if (mode.parent == null)
            {
               // print($"Assigning parent for {mode.data.modeName} ");
                mode.parent = Instantiate(new GameObject(), transform, false).transform;
                mode.parent.rotation = Quaternion.identity;
                mode.parent.name = mode.data.modeName + " Triggers Parent";
            }
    }

    public RuntimeModeData Mode(string mode)
    {
        foreach(RuntimeModeData _mode in modes)
            if(_mode.name == mode)
                return _mode;

        if (modes.Count == 0)
            Debug.LogError($"[{gameObject.name}] No modes detected ");


        Debug.LogError($"[EntityController : {gameObject.name}] Could not find a mode of the name [{mode}] in currentModeData");
        throw new InvalidOperationException("Cannot return a ref to a new struct instance.");
    }

    public RuntimeModeData CurMode()
    {
        return Mode(mode);
    }

    public void SetAllModesNotUsing()
    {
        foreach (RuntimeModeData m in modes)
            m.isUsing = false;
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
        if (waitingToReattack)
            didReattack = true;
        else
            didReattack = false;

        //print($"[{gameObject.name}] Reattack check: didReattack = {didReattack}");
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
            animController.wpn = currentWeapon;
    }

    void BaseFlinch(float flinchTime)
    {
        SetAllModesNotUsing();
        ResetAttack?.Invoke();
    }
    void ResetmyAttack()
    {
        //print("Reattack System: RESSETING ATTACKING VARIABLES");
        waitingToReattack = false;
        didReattack = false;
        reattackChecking = false;
        dashing = false;
        Mode("Attack").isUsing = false;
    }

}
