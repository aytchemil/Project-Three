using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ComboAbility", menuName = "ScriptableObjects/Abilities/Combo Ability")]
public class ComboAbility : Ability
{
    public AttackMultiAbility comboChain;
}
