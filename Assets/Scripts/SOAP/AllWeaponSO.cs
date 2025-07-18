using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Weapons", menuName = "ScriptableObject/All Weapons")]
public class AllWeaponSO : ScriptableObject
{
    public List<Weapon> weapons;
}
