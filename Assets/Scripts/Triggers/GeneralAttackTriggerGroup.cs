using System.Collections;
using UnityEngine;

public class GeneralAttackTriggerGroup : ModeTriggerGroup
{
    public override Ability ability { get; set; }

    //Wraps trigger
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
        cf.Controls.Mode("Attack").functionality.Finish();

        if (cf.Controls.GetTarget?.Invoke() != null)
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

    protected override void InitializeSelfImplementation(CombatFunctionality cf, Ability abilty)
    {
        //print(cf.gameObject.name + " | ability trigger [" + gameObject.name + "] self initializing...");

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
    
    /// <summary>
    /// The Animation Event For When the miss attack cuttoff point is reached
    /// </summary>
    public virtual void FinishSingleAttack()
    {
        //print("[TRIGGER] [GEN] " + cf.gameObject.name + "'s Miss Attack Cuttoff Reached");
        MissAttackCuttoffLocal();

        if (isLocal)
            return;


        float delay;
        if (hitAttack)
            delay = ability.successDelay[0];
        else
        {
            cf.Controls.MissedAttack?.Invoke();
            delay = ability.unsuccessDelay[0];
        }

        WaitExtension.Wait(cf, delay, DisableThisTrigger);
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
        cf.Controls.CompletedAttack?.Invoke();
    }

    public void ResetAttackCaller()
    {
        //print(gameObject.name + ": ResetAttackCaller()");

        #region debug check for resetattack
        if (cf.Controls.ResetAttack != null)
        {
            foreach (var subscriber in cf.Controls.ResetAttack.GetInvocationList())
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

        cf.Controls.ResetAttack?.Invoke();
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

        if (cf.Controls.Mode("Combo").isUsing)
            ability = cf.Controls.Mode("Combo").ability;
        else if (cf.Controls.Mode("Attack").isUsing)
            ability = cf.Controls.Mode("Attack").ability;
        else
            throw new System.Exception("AT Collider Single: Block Completed Sequence: ability not found to sent to CF");

        print($"didreattack: ability is {ability}");

        cf.Controls.MyAttackWasBlocked?.Invoke(myLookdir, ability);
    }

    protected override void DisableThisTriggerOnDelayImplementation()
    {
        throw new System.NotImplementedException();
    }


    #endregion



}
