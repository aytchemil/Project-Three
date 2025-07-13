using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// AbilityExecutor uses the Service Pattern.
// This class acts as a stateless utility that coordinates the execution of AbilityEffects,
// decoupling the logic of effect processing from entities or abilities themselves.
// It centralizes behavior execution into a reusable, shared service that can be called
// from any context, promoting separation of concerns and modular design.
public static class AbilityExecutor
{
    public static void ExecuteAbility(Ability ability, GameObject attacker, GameObject target)
    {
        bool canExecuteDirectional = CheckForDirectionalExecution(attacker, target);

        foreach (var effect in ability.effects)
        {
            if (effect is DirectionalEffect && !canExecuteDirectional)
                continue; // skip this directional effect only

            effect.Execute(attacker, target);
        }


        bool CheckForDirectionalExecution(GameObject attacker, GameObject target)
        {
            bool hasDirectional = ability.effects.Any(e => e is DirectionalEffect);

            if (hasDirectional)
                return DirectionalEffect.CanExecute(attacker, target);

            return true;
        }
    }
}
