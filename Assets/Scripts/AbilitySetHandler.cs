using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Collections.Generic;

public class AbilitySetHandler : MonoBehaviour
{
    public AbilitySet DIset;
    public string dir; // "right", "left", "up", "down"

    public void ChangeAbility(Ability ability)
    {
        if (dir == "right")
            DIset.right = ability;
        else if (dir == "left")
            DIset.left = ability;
        else if (dir == "up")
            DIset.up = ability;
        else if (dir == "down")
            DIset.down = ability;

        Debug.Log($"[{dir.ToUpper()}] Ability changed to: {(ability ? ability.name : "None")}");
    }

    public void ChangeAbilityForDirection(Ability ability, string dir)
    {
        if (dir == "right")
            DIset.right = ability;
        else if (dir == "left")
            DIset.left = ability;
        else if (dir == "up")
            DIset.up = ability;
        else if (dir == "down")
            DIset.down = ability;

        Debug.Log($"[AbilitySetHandler] Set {dir} ability to {ability.name}");
    }

}
