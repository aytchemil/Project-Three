using System.Collections;
using UnityEngine;
using static EntityController;

public interface ICombatMode 
{
    /// <summary>
    /// PARENT VIRTUAL STRING for all names of modes (Set in the child class)
    /// </summary>
    public abstract string MODE { get; }

    /// <summary>
    /// PARENT VIRTUAL FUNCTION for using all mode functionalities
    /// </summary>
    public abstract void UseModeFunctionality();
}
