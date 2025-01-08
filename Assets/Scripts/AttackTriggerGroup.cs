using UnityEngine;

public class AttackTriggerGroup : MonoBehaviour
{

    [Header("Attack Trigger Group: Real-Time Variables")]
    public CombatFunctionality combatFunctionality;
    public AttackAbility myAttackAbility;

    public bool attacking;
    public bool hitAttack;
    public bool missedAttack;


    public virtual void Awake()
    {

    }

    public virtual void InitSelf(CombatFunctionality combatFunctionality)
    {
        this.combatFunctionality = combatFunctionality;
        //print("initializing self: " + gameObject.name);

    }

    /// <summary>
    /// Tells this script what its attack parameters are
    /// </summary>
    /// <param name="currentAttackAbility"></param>
    public virtual void StartAttackFromAttackTrigger(AttackAbility currentAttackAbility, float delay)
    {
        if (attacking)
        { 
            //print("already attacking, cannot re attack"); 
            return; 
        }
        //Debug.Log("Starting an Attack: " + currentAbility.name);
        myAttackAbility = currentAttackAbility;
        InitializeTrigger();
        Invoke(nameof(InitialAttackDelayOverReEnableTrigger), delay);
    }

    public virtual void InitializeTrigger()
    {
        //print("initializing attack trigger: " + gameObject.name);
        attacking = true;
        missedAttack = false;
        hitAttack = false;
        combatFunctionality.initialAttackDelayOver = false;
    }

    public virtual void InitialAttackDelayOverReEnableTrigger()
    {
        // print("Initial Attack Delay over, reenbling trigger");
        combatFunctionality.initialAttackDelayOver = true;
    }

    public virtual void DisableTrigger()
    {
        print(gameObject.name + " | Disabling trigger");

        CompleteAttackCaller();
        combatFunctionality.initialAttackDelayOver = false;
        combatFunctionality.FinishAttacking();

        if (combatFunctionality.Controls.GetTarget?.Invoke() != null)
            ResetAttackCaller();

        DisableThisTriggerOnlyLocally();
    }

    public virtual void DisableThisTriggerOnlyLocally()
    {
        attacking = false;
        missedAttack = false;
        hitAttack = false;
        //print(gameObject.name + " | Disabled trigger");

        gameObject.SetActive(false);
    }

    public void CompleteAttackCaller()
    {
        print(gameObject.name + " | Compeleted Attack Caller Called");
        combatFunctionality.Controls.CompletedAttack?.Invoke();
    }

    public void ResetAttackCaller()
    {
        //print("Attack Trigger: ResetAttackCaller()");

        #region debug check for resetattack
        if (combatFunctionality.Controls.ResetAttack != null)
        {
            foreach (var subscriber in combatFunctionality.Controls.ResetAttack.GetInvocationList())
                if (subscriber != null)
                {
                    //print(subscriber);
                }
                else
                    Debug.LogError("No subscribers found in reset attack, this needs movement subscribed to it, check for that first");
        }
        else
        {
            //print("No subscribers found on reset attack, if this script requres a movement script it needs to be subscribed to it.");
        }
        #endregion

        combatFunctionality.Controls.ResetAttack?.Invoke();
    }

    public virtual void HitAttack()
    {
        //Debug.Log("Attack hit registered");
        hitAttack = true;
    }

    public virtual void MissAttackCuttoff()
    {
        //print("Miss Attack Cuttoff Reached");
        if (hitAttack) return;

        MissAttackCuttoffLocal();

        MissedAttackCaller();

        print(gameObject.name +  " | Missed attack delay over, DisableTrigger() (on delay)");
        Invoke(nameof(DisableTrigger), myAttackAbility.missDelayUntilAbleToAttackAgain);
    }

    public virtual void MissAttackCuttoffLocal()
    {
        //print("missed attack");
        missedAttack = true;
        hitAttack = false;
    }

    public void MissedAttackCaller()
    {
        combatFunctionality.Controls.MissedAttack?.Invoke();
    }

    public void ComboOffOfHitNowAvaliable()
    {
        if (hitAttack)
        {
            //print("Combo off hit time period reached, can now combo because attack hit");
            print(gameObject.name + " | Combo hit avaliable, DisableTrigger()");
            DisableTrigger();
        }
    }


}
