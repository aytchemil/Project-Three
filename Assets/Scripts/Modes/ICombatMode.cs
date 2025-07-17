using System.Collections;
using UnityEngine;
using static EntityController;

public interface ICombatMode 
{
    public abstract CombatFunctionality cf { get; set; }
    public abstract string MODE { get; }

    /// <summary>
    /// PARENT VIRTUAL FUNCTION for using all mode functionalities
    /// </summary>
    public abstract void UseModeFunctionality();
}
