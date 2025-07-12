using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class AttackingAI : MonoBehaviour
{
    CombatEntityController Controls;
    CombatLock combatLock;
    CombatFunctionality combatFunctionality;
    EntityLook entityLook;

    string[] lookDirs = { "right", "left", "up", "down" };

    public bool thinking;

    [System.Serializable]
    public struct aiStats
    {
        public float thinkDelay;
        public float resetAttacksThinkTime;
        public float[] attackingSpeedDelays;
    }
    [SerializeField]
    public aiStats stats;

    public bool blockOnly = false;


    public class AIInputs
    {
        public static Action<CombatEntityController> input_UseCombo = (Controls) =>
        {
            print($"[AttackingAI] [{Controls.gameObject.name}] input: Use Combo");
            Controls.usedCombo = true;
            Controls.useAbility?.Invoke("Combo");
        };

        public static Action<CombatEntityController , string> input_changeLookDir = (Controls, s) =>
        {
            print($"[AttackingAI] [{Controls.gameObject.name}] input: Change look direction");
            Controls.lookDir = s;
        };

        public static Action<CombatEntityController> input_UseBlock = (Controls) =>
        {
            print($"[AttackingAI] [{Controls.gameObject.name}] input: Block");
            Controls.blockStart?.Invoke();
            Controls.useAbility?.Invoke("Block");
        };


    }



    private void Awake()
    {
        Controls = GetComponent<CombatEntityController>();
        combatLock = GetComponent<CombatLock>();
        combatFunctionality = GetComponent<CombatFunctionality>();
        entityLook = GetComponent<EntityLook>();
    }

    protected void OnEnable()
    {
        Controls.useAbility += combatFunctionality.UseAbility;
        Controls.lockOn += combatLock.AttemptLock;

        Controls.Flinch += Flinch;
        Controls.ResetAttack += ResetAiAttacks;
    }

    protected void OnDisable()
    {
        Controls.useAbility -= combatFunctionality.UseAbility;
        Controls.lockOn -= combatLock.AttemptLock;

        Controls.Flinch -= Flinch;
        Controls.ResetAttack -= ResetAiAttacks;
    }

    private void Start()
    {
        StartCoroutine(CheckForLockons());

        IEnumerator CheckForLockons()
        {
            while (Controls.isAlive)
            {
                print($"[{gameObject.name}] [AttackingAI] Checking for lockon...");
                yield return new WaitForSeconds(1f);
                if (!Controls.isLockedOn && !combatLock.myColliderDetector.targetDescisionMade)
                    AttemptLockOn();
            }
        }
    }

    private void FixedUpdate()
    {
        //Checks
        if (!Controls.isAlive || thinking || Controls.isFlinching) return;


        //Lock on
        if (Controls.isLockedOn)
        {
            if (!Controls.targetIsDodging)
            {
                Controls.CombatFollowTarget?.Invoke(combatLock.myColliderDetector.closestCombatEntity.GetComponent<CombatEntityController>());
            }
            else
            {
               // print("target is dodging, cant hit");
            }

            //Analyzing
            CombatEntityController target = Controls.GetTarget?.Invoke();

            //Dashing
            Controls.targetIsDodging = target.dashing;



            if (blockOnly)
            {
                if(!Controls.Mode("Block").isUsing)
                    StartCoroutine(BlockCycleTest());

                return;
            }

            //Attacking
            if (!Controls.Mode("Attack").isUsing && !Controls.Mode("Combo").isUsing)
                StartCoroutine(InCombatThinkCycle());





        }
    }

    void AttemptLockOn()
    {
        print(gameObject.name + " attempting to lock on");
        Controls.lockOn?.Invoke();
    }

    AbilityCombo RandomCombo(int indx)
    {
        for(int i = 0; i < Controls.abilitySetInputs.Count; i++)
            if (Controls.abilitySetInputs[i].mode == ModeManager.Modes.Combo)
                return (Controls.abilitySetInputs[i])[indx] as AbilityCombo;

        throw new Exception("No Combo found");
    }

    AbilityBlock RandomBlock(int indx)
    {
        for (int i = 0; i < Controls.abilitySetInputs.Count; i++)
            if (Controls.abilitySetInputs[i].mode == ModeManager.Modes.Block)
                return Controls.abilitySetInputs[i][indx] as AbilityBlock;

        throw new Exception("No Combo found");
    }

    IEnumerator InCombatThinkCycle()
    {
        Controls.Mode("Combo").isUsing = true;

        //Setup
        int randIndx = UnityEngine.Random.Range(0, AbilitySet.MAX_ABILITIES);
        AbilityCombo combo = RandomCombo(randIndx);

        AIInputs.input_changeLookDir(Controls, SetupAIlookDir(randIndx));
        //print($"[{gameObject.name}] LookDir is ({Controls.lookDir})");

        AIInputs.input_UseCombo(Controls);

        while (Controls.Mode("Combo").isUsing)
        {
            TestForResetCycleFlags();

            print($"[{gameObject.name}] [AttackingAI] Changing combo to " + combo);

            yield return new WaitForSeconds(stats.thinkDelay);

            InCombatChancesToDoMoves(Random.Range(0, 10));


        }
    }

    IEnumerator BlockCycleTest()
    {
        print("Block cycle");

        //Setup
        int randIndx = UnityEngine.Random.Range(0, AbilitySet.MAX_ABILITIES);
        AbilityBlock block = RandomBlock(randIndx);

        AIInputs.input_changeLookDir(Controls, SetupAIlookDir(randIndx));
        print("new look dir is " + Controls.lookDir);

        yield return new WaitForSeconds(stats.thinkDelay);

        AIInputs.input_UseBlock(Controls);

        Controls.Mode("Block").isUsing = true;
    }

    string SetupAIlookDir(int abilitySetIndex)
    {
        return AbilitySet.GetLookDir(abilitySetIndex);
    }

    string ChooseRandomString(string string1, string string2, string string3, string string4)
    {
        string[] strings = { string1, string2, string3, string4 };
        return strings[Random.Range(0, strings.Length)];
    }

    void InCombatChancesToDoMoves(int num)
    {
        //print($"[AttackingAI] chance to do moves: {num}");

        if (num <= 8)
            if (!Controls.Mode("Combo").isUsing)
                AIInputs.input_changeLookDir(Controls, lookDirs[Random.Range(0, 10) % 4]);

        if (num <= 9)
            AIInputs.input_UseCombo(Controls);

    }


    void TestForResetCycleFlags()
    {
        if (thinking)
            Controls.Mode("Combo").isUsing = false;

    }


    void Flinch(float flinchTime)
    {
        Controls.SetAllModesNotUsing();
        Controls.ResetAttack?.Invoke();
    }

    /// <summary>
    /// Reset the attacks with a delay amount, if no delay put 0
    /// </summary>
    /// <param name="amount"></param>
    void ResetAiAttacks()
    {
        StartCoroutine(DoReset(stats.resetAttacksThinkTime));

        IEnumerator DoReset(float amount)
        {
            thinking = true;
            print($"[AttackingAI] [{gameObject.name}] is thinking at a delay of [{amount}]...");
            yield return new WaitForSeconds(amount);
            thinking = false;
        }
    }

    
}
