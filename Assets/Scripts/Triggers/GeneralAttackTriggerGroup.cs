using System.Collections;
using UnityEngine;

public class GeneralAttackTriggerGroup : ModeTriggerGroup
{
    public override Ability ability { get; set; }

    //Wrapper for trigger
    public virtual bool attacking { get; set; }
    public override bool trigger
    {
        get => attacking;
        set => attacking = value;
    }


    [SerializeField] public virtual bool hitAttack { get; set; }
    public override bool used
    {
        get => hitAttack;
        set => hitAttack = value;
    }

    [SerializeField] public virtual bool missedAttack { get; set; }

    public override bool unused 
    { 
        get => missedAttack; 
        set => missedAttack = value;
    }



    #region  Template Pattern Overrides
    //Template Pattern Overrides

    protected override void EnableTriggerImplementation()
    {
        missedAttack = false;
        hitAttack = false;
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        //print("ability use delay over, attacking...");
    }

    protected override void DisableThisTriggerImplementation()
    {
        //print(gameObject.name + " | Disabling trigger implementation");

        CompleteAttackCaller();
        (combatFunctionality.Controls.Mode("Attack").data.modeFunctionality as ModeAttackFunctionality).FinishAttacking();

        if (combatFunctionality.Controls.GetTarget?.Invoke() != null)
        {
            //print("going to reset attack caller");
            ResetAttackCaller();

        }
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        missedAttack = false;
        hitAttack = false;
    }

    #endregion

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        //print(combatFunctionality.gameObject.name + " | ability trigger [" + gameObject.name + "] self initializing...");

        ability = abilty;
        attacking = false;
        gameObject.SetActive(false);
    }






    #region  Virtual Methods
    //Virtual Methods
    ///===============================================================================================================

    public virtual void HitAttack()
    {
        //Debug.Log(gameObject.name + " | Attack hit registered");
        hitAttack = true;
    }

    public virtual void MissAttackCuttoff()
    {
        if (isLocal)
        {
            MissAttackCuttoffLocal();
            return;
        }

        //print("[TRIGGER] [GEN] " + combatFunctionality.gameObject.name + "'s Miss Attack Cuttoff Reached");
        if (hitAttack)
        {
            //print("hit atack succesffull, calling combo");
            StartCoroutine(SuccessfullyFinishedAttacked());
            return;
        }

        //print("[TRIGGER] [GEN] missed attack global");

        MissAttackCuttoffLocal();

        MissedAttackCaller();

        Invoke(nameof(DisableThisTrigger), ability.unsuccessDelay[ability.unsuccessDelay.Length-1]);

    }

    public virtual void MissAttackCuttoffLocal()
    {
        if (hitAttack) return;
        missedAttack = true;
        hitAttack = false;
    }

    #endregion









    #region Methods
    // Methods
    ///===================================================================================================================

    public void CompleteAttackCaller()
    {
       // print(gameObject.name + " | Compeleted Attack Caller Called");
        combatFunctionality.Controls.CompletedAttack?.Invoke();
    }

    public void ResetAttackCaller()
    {
        //print(gameObject.name + ": ResetAttackCaller()");

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

    public virtual IEnumerator SuccessfullyFinishedAttacked()
    {
        yield return new WaitForSeconds(ability.successDelay[0]);

        DisableThisTrigger();
    }

    public virtual void AttackTriggerBlocked(string myLookdir, Vector3 effectPos)
    {
        print(gameObject.name + " I am getting Blocked");

        Ability ability = null;

        if (combatFunctionality.Controls.Mode("Combo").isUsing)
            ability = combatFunctionality.Controls.Mode("Combo").ability;
        else if (combatFunctionality.Controls.Mode("Attack").isUsing)
            ability = combatFunctionality.Controls.Mode("Attack").ability;
        else
            throw new System.Exception("AT Collider Single: Block Completed Sequence: ability not found to sent to CF");

        print($"didreattack: ability is {ability}");

        combatFunctionality.Controls.MyAttackWasBlocked?.Invoke(myLookdir, ability);
    }

    protected override void DisableThisTriggerOnDelayImplementation()
    {
        throw new System.NotImplementedException();
    }


    #endregion



}
