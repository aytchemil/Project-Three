using UnityEngine;
using UnityEngine.Events;
using static EntityController;

[UnityEngine.Scripting.Preserve]
public interface ICombatMode 
{
    public abstract CombatFunctionality cf { get; set; }
    public abstract string MODE { get; }
    public abstract RuntimeModeData Mode { get; }

    public virtual void SetCF(CombatFunctionality cf)
    {
        this.cf = cf;
    }

    public abstract void InitializeFunctionalityAfterOnEnable();

    public void UseMode()
    {
        //Guard Clauses for Every Ability
        if (cf.Controls.cantUseAbility) return;
        if (Mode.initializedTriggers == false) return;

        Mode.functionality.Starting();
        Mode.SetAbility(Mode.ability);
        if (Mode.data.isStance)
            Mode.trigger = cf.AbilityTriggerEnableUse(Mode);
        else
            Mode.trigger = cf.WheelTriggerUse(Mode);

        UseModeImplementation();
    }

    public abstract void UseModeImplementation();

    public void Starting()
    {
        StartingImplementation();
    }

    public void Finish()
    {
        FinishImplementation();
    }

    void StartingImplementation()
    {
        Mode.isUsing = true;
    }

    void FinishImplementation()
    {
        Mode.isUsing = false;
    }


}
