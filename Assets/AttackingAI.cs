using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingAI : MonoBehaviour
{
    CombatEntityController controls;
    CombatLock combatLock;
    CombatFunctionality combatFunctionality;
    EntityLook entityLook;

    public List<EntityAttackPattern> preSelectedAttackPatterns;
    public Action look;
    public Action move;
    public Action sprint;
    public Action lockOn;
    public Action dash;
    public Action attack;
    public Action block;

    public bool thinking;
    public Vector2 thinkingPeriodBetweenAttackPatternsRange;
    public bool attackingWithPattern;
    public List<bool> attackPatternProgress;
    public int currentAttackInAttackPattern = 0;

    private void Awake()
    {
        controls = GetComponent<CombatEntityController>();
        combatLock = GetComponent<CombatLock>();
        combatFunctionality = GetComponent<CombatFunctionality>();
        entityLook = GetComponent<EntityLook>();
    }

    protected void OnEnable()
    {
        attack += combatFunctionality.UseAttackAbility;
        lockOn += combatLock.AttemptLock;

        //print("f");
        block += BlockCaller;
        block += StopBlockingCaller;

    }

    protected void OnDisable()
    {
        attack -= combatFunctionality.UseAttackAbility;
        lockOn -= combatLock.AttemptLock;

        block -= BlockCaller;
        block -= StopBlockingCaller;
    }

    private void FixedUpdate()
    {
        if (!controls.isAlive) return;


        if (!controls.isLockedOn)
            AttemptLockOn();

        //If not in combat,"Look regularly". else "combat look"
        if (controls.isLockedOn)
        {
            controls.CombatFollowTarget?.Invoke(combatLock.myColliderDetector.closestCombatEntity.GetComponent<CombatEntityController>());

            if(!thinking && !controls.alreadyAttacking && !attackingWithPattern)
            {
                EntityAttackPattern attackPattern = ChoseRandomAttackPattern();
                StartCoroutine(Attack(attackPattern));
            }




        }
    }

    void AttemptLockOn()
    {
        //print(gameObject.name + " attempting to lock on");
        lockOn?.Invoke();
    }



    void BlockCaller()
    {
        // print("Player Combat : Block Caller called");
        controls.Block?.Invoke();
    }

    void StopBlockingCaller()
    {
        //print("Player Combat : Stop Blocking Caller called");
        controls.StopBlocking?.Invoke();
    }

    EntityAttackPattern ChoseRandomAttackPattern()
    {
        print("Chosen a random attack pattern");
        int randomIndex = UnityEngine.Random.Range(0, preSelectedAttackPatterns.Count);
        return preSelectedAttackPatterns[randomIndex];
    }

    IEnumerator Attack(EntityAttackPattern attackPattern)
    {
        attackingWithPattern = true;
        int attackPatternLength = attackPattern.attackDir.Count;

        print("Attacking w/ attack pattern: " + attackPattern);

        //Creates the new progress list
        attackPatternProgress = new List<bool>();
        for (int i = 0; i < attackPatternLength; i++)
            attackPatternProgress.Add(new bool());

        print("Attack pattern length is : " + attackPatternLength + " indexing until : " + (attackPatternLength));

        for(int i = 0; i < attackPatternLength; i++)
        {
            Debug.Log("ACTUAL ATTACK: (i=" + i + ")   |  DIRECTION: " + attackPattern.attackDir[i]);

            controls.SelectCertainAbility?.Invoke(attackPattern.attackDir[i].ToString());
            attack?.Invoke();

            while (attackPatternProgress[i] == false)
            {

                yield return new WaitForSeconds(1f);
                if (!controls.isAlive)
                {
                    ResetAttacking();
                    print("Died, stopping attacking");
                    yield break;
                }

                CompleteAttackInPattern();// For now
            }

            print("Ended: " + i);
        }
        print("Coroutine Period of attacking over, on thinking cooldown");
        thinking = true;
        ResetAttacking();
        Invoke("AttackThinkingPeriodEnd", UnityEngine.Random.Range(thinkingPeriodBetweenAttackPatternsRange.x, thinkingPeriodBetweenAttackPatternsRange.y));
    }

    /// <summary>
    /// This is where the animations of the attack, after compelting that attack will use this to iterate to the next attack in the pattern 
    /// </summary>
    public void CompleteAttackInPattern()
    {
        if (attackPatternProgress[attackPatternProgress.Count-1] == false)
        {
            print("Completeing: " + currentAttackInAttackPattern);
            attackPatternProgress[currentAttackInAttackPattern] = true;
            currentAttackInAttackPattern++;
        }
        else
        {
            print("All Attacks complete");
        }
    }

    public void AttackThinkingPeriodEnd()
    {
        print("attack period ended");
        thinking = false;
    }

    void ResetAttacking()
    {
        attackPatternProgress.Clear();
        attackingWithPattern = false;
        currentAttackInAttackPattern = 0;
    }

}
