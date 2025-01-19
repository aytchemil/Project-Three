using UnityEngine;

public class GeneralAttackTriggerGroup : ModeTriggerGroup
{
    //Wrapper for usingTrigger
    public virtual bool attacking { get; set; }
    public override bool usingTrigger
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

    public virtual bool missedAttack { get; set; }

    public override bool unused 
    { 
        get => missedAttack; 
        set => missedAttack = value;
    }

    public bool countered;




    #region  Template Pattern Overrides
    //Template Pattern Overrides

    protected override void EnableTriggerImplementation()
    {
        missedAttack = false;
        hitAttack = false;
        countered = false;
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        //print("ability use delay over, attacking...");
    }

    protected override void DisableThisTriggerImplementation()
    {
        //print(gameObject.name + " | Disabling trigger implementation");

        CompleteAttackCaller();
        combatFunctionality.FinishAttacking();

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
        countered = false;
    }

    #endregion

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        //print(combatFunctionality.gameObject.name + " | ability trigger [" + gameObject.name + "] self initializing...");

        myAbility = abilty;
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
        countered = false;
    }

    public virtual void MissAttackCuttoff()
    {
        if (isLocal)
        {
            MissAttackCuttoffLocal();
            return;
        }

        //print(combatFunctionality.gameObject.name + "'s Miss Attack Cuttoff Reached" + ": " + myAttackingAbility);
        if (hitAttack)
        {
            //print("hit atack succesffull, calling combo");
            Invoke(nameof(ComboOffOfHitNowAvaliable), myAbility.successDelay);
            return;
        }

        MissAttackCuttoffLocal();

        MissedAttackCaller();

        Invoke(nameof(DisableThisTrigger), myAbility.unsuccessDelay);

    }

    public virtual void MissAttackCuttoffLocal()
    {
        if (hitAttack) return;

        //print("missed attack local");
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

    public void ComboOffOfHitNowAvaliable()
    {
        print("Comboing");
       // print(gameObject.name + " | Combo hit avaliable, DisableTrigger()");
        DisableThisTrigger();
    }

    public void GetCountered(Vector3 effectPos)
    {
        //print(gameObject.name + " I am getting countered");
        countered = true;
        combatFunctionality.GetCountered(effectPos);
    }

    #endregion



}
