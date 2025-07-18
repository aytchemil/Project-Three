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
    public static void ExecuteAbility(Ability ability, GameObject attacker)
    {
        foreach (var effect in ability.effects)
            effect.Execute(attacker);
    }

    public static void OnHit(Ability ability, GameObject attacker, GameObject target)
    {
        foreach (var effect in ability.effects) 
            if(effect is IAEffectOnHit hitEffect)
                hitEffect.OnHit(attacker, target);
    }
}
