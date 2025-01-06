using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingAI : MonoBehaviour
{
    CombatEntityController Controls;
    CombatLock combatLock;
    CombatFunctionality combatFunctionality;
    EntityLook entityLook;

    public List<EntityAttackPattern> preSelectedAttackPatterns;

    public enum AttackingSpeed
    {
        Slow = 0,
        Medium = 1,
        Fast = 2,
    }
    public AttackingSpeed attackingSpeed;
    public float[] attackingSpeedDelays;
    public bool thinking;
    public Vector2 thinkingPeriodBetweenAttackPatternsRange;
    public bool attackingWithPattern;
    public List<bool> attackPatternProgress;
    public int currentAttackInAttackPattern = 0;

    private void Awake()
    {
        Controls = GetComponent<CombatEntityController>();
        combatLock = GetComponent<CombatLock>();
        combatFunctionality = GetComponent<CombatFunctionality>();
        entityLook = GetComponent<EntityLook>();
    }

    protected void OnEnable()
    {
        Controls.attack += combatFunctionality.UseAttackAbility;
        Controls.lockOn += combatLock.AttemptLock;

        Controls.CompletedAttack += CompleteAttackInPattern;
        Controls.Flinch += Flinch;

        Controls.ExitCombat += ExitCombat;

    }

    protected void OnDisable()
    {
        Controls.attack -= combatFunctionality.UseAttackAbility;
        Controls.lockOn -= combatLock.AttemptLock;

        Controls.CompletedAttack -= CompleteAttackInPattern;
        Controls.Flinch -= Flinch;

        Controls.ExitCombat -= ExitCombat;
    }

    private void FixedUpdate()
    {
        if (!Controls.isAlive) return;


        if (!Controls.isLockedOn && !combatLock.myColliderDetector.targetDescisionMade)
            AttemptLockOn();

        //If not in combat,"Look regularly". else "combat look"
        if (Controls.isLockedOn)
        {
            if (!Controls.targetIsDodging)
            {
                //Controls.CombatFollowTarget?.Invoke(combatLock.myColliderDetector.closestCombatEntity.GetComponent<CombatEntityController>());
            }
            else
            {
               // print("target is dodging, cant hit");
            }

            //Analyzing
            CombatEntityController target = Controls.GetTarget?.Invoke();


            //Dashing
            Controls.targetIsDodging = target.dashing;



            //Attacking
            if (!thinking && !Controls.alreadyAttacking && !attackingWithPattern && !Controls.isFlinching)
            {
                EntityAttackPattern attackPattern = ChoseRandomAttackPattern();
                StartCoroutine(Attack(attackPattern));
            }




        }
    }

    void AttemptLockOn()
    {
        //print(gameObject.name + " attempting to lock on");
        Controls.lockOn?.Invoke();
    }

    EntityAttackPattern ChoseRandomAttackPattern()
    {
        //print("Chosen a random attack pattern");
        int randomIndex = UnityEngine.Random.Range(0, preSelectedAttackPatterns.Count);
        return preSelectedAttackPatterns[randomIndex];
    }

    IEnumerator Attack(EntityAttackPattern attackPattern)
    {
        attackingWithPattern = true;
        int attackPatternLength = attackPattern.attackDir.Count;

       // print("Attacking w/ attack pattern: " + attackPattern);

        //Creates the new progress list
        attackPatternProgress = new List<bool>();
        for (int i = 0; i < attackPatternLength; i++)
            attackPatternProgress.Add(new bool());



       // print("Attack pattern length is : " + attackPatternLength + " indexing until : " + (attackPatternLength));

        for(int i = 0; i < attackPatternLength; i++)
        {
            //print("NEW ATTACK IN PATTERN========");
            //Debug.Log("Selected Attack Direction: " + attackPattern.attackDir[i]);

            //Select the attack we are using
            Controls.SelectCertainAbility?.Invoke(attackPattern.attackDir[i].ToString());

           // print("Attacking...");
            //Actually Start the Attack
            Controls.attack?.Invoke();

            //Debug.Log("waiting for attack to finish now...");
            while (attackPatternProgress[i] == false && attackingWithPattern)
            {
                //Debug.Log("attack in progress");
                yield return new WaitForEndOfFrame();
                if (!Controls.isAlive)
                {
                    ResetAttacking();
                    print("Died, stopping attacking");
                    yield break;
                }
    
            }
             //print("attack has finished: " + i);
        }
        //print("Attacking Period over, Thinking....");
        ResetAttacking();
        StartThinking();
    }

    /// <summary>
    /// This is where the animations of the attack, after compelting that attack will use this to iterate to the next attack in the pattern 
    /// </summary>
    public void CompleteAttackInPattern()
    {
        StartCoroutine(CompleteAttackInPatternContinued());
    }

    IEnumerator CompleteAttackInPatternContinued()
    {
        yield return new WaitForSeconds(attackingSpeedDelays[(int)attackingSpeed]);

        int finalIndexInAttackPattern = attackPatternProgress.Count - 1;

        //Debug.Log("Completing attack [" + currentAttackInAttackPattern + "] in pattern of length " + finalIndexInAttackPattern);

        if (finalIndexInAttackPattern == -1) yield break;

        //print("Last attack in attack pattern is : " + attackPatternProgress[lastIndex]);
        if (attackPatternProgress[finalIndexInAttackPattern] == false)
        {
            //print("Completeing: " + currentAttackInAttackPattern);
            attackPatternProgress[currentAttackInAttackPattern] = true;
            currentAttackInAttackPattern++;
        }
    }

    void StartThinking()
    {
        thinking = true;
        Invoke(nameof(AttackThinkingPeriodEnd), UnityEngine.Random.Range(thinkingPeriodBetweenAttackPatternsRange.x, thinkingPeriodBetweenAttackPatternsRange.y));
    }

    public void AttackThinkingPeriodEnd()
    {
       // print("attack period ended");
        thinking = false;
    }

    void ResetAttacking()
    {
        attackPatternProgress.Clear();
        attackingWithPattern = false;
        currentAttackInAttackPattern = 0;
        StopAllCoroutines();
    }

    void Flinch(float flinchTime)
    {
        ResetAttacking();
        StartThinking();
    }

    void ExitCombat()
    {
        ResetAttacking();
        StartThinking();
    }
}
