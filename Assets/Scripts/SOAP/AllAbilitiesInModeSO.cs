using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Abilites in Mode List", menuName = "ScriptableObjects/AllAbilitiesList")]

public class AllAbilitiesInModeSO : ScriptableObject
{
    public List<Ability> abilities;
}
