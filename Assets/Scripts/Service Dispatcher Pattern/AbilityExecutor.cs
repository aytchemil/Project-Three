using System;
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

    public static void ExecuteRuntimeAbility<T>(
        Ability ability,
        GameObject attacker,
        Type chosenInterface,
        T data)
    {
        foreach (var effect in ability.effects)
            foreach (var Interface in effect.GetType().GetInterfaces())
                if (Interface == chosenInterface)
                {
                    var method = Interface.GetMethod("Execute") ?? throw new Exception($"Interface {Interface} cannot find method Execute");

                    method.Invoke(effect, new object[] { attacker, data });
                    Debug.Log("Invoking Execute On Runtime Ability");
                }
    }

    public static void OnHit(Ability ability, GameObject attacker, GameObject target)
    {
        foreach (var effect in ability.effects) 
            if(effect is IAEffectOnHit hitEffect)
                hitEffect.OnHit(attacker, target);
    }

    public static void OnHit(Ability ability, string attackDirection, GameObject attacker, GameObject target)
    {
        foreach (var effect in ability.effects)
            if (effect is IAEffectOnHit hitEffect)
                hitEffect.OnHit(attacker, attackDirection, target);
    }
}
