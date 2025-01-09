using UnityEngine;

public class AttackTriggerGroup : ModeTriggerGroup
{
    private AttackingAbility myAttackingAbility;

    //Overriding base class Ability reference
    public override Ability myAbility
    {
        get => myAttackingAbility;
        set => myAttackingAbility = value as AttackingAbility;
    }

    //Wrapper for usingTrigger
    public bool attacking
    {
        get => usingTrigger;
        set => usingTrigger = value;
    }

    public bool hitAttack;
    public bool missedAttack;




    #region  Template Pattern Overrides
    //Template Pattern Overrides

    protected override void EnableTriggerImplementation()
    {
        missedAttack = false;
        hitAttack = false;
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        print("ability use delay over, attacking...");
    }

    protected override void DisableThisTriggerImplementation()
    {
        print(gameObject.name + " | Disabling trigger implementation");

        CompleteAttackCaller();
        combatFunctionality.FinishAttacking();

        if (combatFunctionality.Controls.GetTarget?.Invoke() != null)
            ResetAttackCaller();
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        missedAttack = false;
        hitAttack = false;
    }

    #endregion

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality)
    {
        //print(combatFunctionality.gameObject.name + " | ability trigger [" + gameObject.name + "] self initializing...");
    }






    #region  Virtual Methods
    //Virtual Methods
    ///===============================================================================================================

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

        print(gameObject.name + " | Missed attack delay over, DisableTrigger() (on delay)");
        Invoke(nameof(DisableThisTrigger), myAttackingAbility.missDelay);
    }

    public virtual void MissAttackCuttoffLocal()
    {
        //print("missed attack");
        missedAttack = true;
        hitAttack = false;
    }

    #endregion









    #region Methods
    // Methods
    ///===================================================================================================================

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
            DisableThisTrigger();
        }
    }

    #endregion

}
