using System.Collections;
using UnityEngine;
using static EntityController;

public interface ICombatMode 
{
    public abstract CombatFunctionality cf { get; set; }
    public abstract string MODE { get; }
    public abstract RuntimeModeData Mode { get; }

    public virtual void Init(CombatFunctionality cf)
    {
        this.cf = cf;
    }

    /// <summary>
    /// PARENT VIRTUAL FUNCTION for using all mode functionalities
    /// </summary>
    public abstract void UseModeFunctionality();

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
