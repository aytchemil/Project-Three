using System.Collections.Generic;
using UnityEngine;
using static EntityController;

public class WeaponManager : MonoBehaviour
{

    public Weapon weapon;

    GameObject weaponParent;
    GameObject currentWeaponObject;

    public virtual EntityController Controls { get; set; }

    public void Init(Weapon wpn, Transform animParent)
    {
        Controls = GetComponent<EntityController>();
        weapon = wpn;

        AssignWeaponParent(animParent);
        InstantiateChosenWeapon();

    }


    void AssignWeaponParent(Transform animParent)
    {
        weaponParent = Instantiate(new GameObject(), animParent, false);
        weaponParent.name = "Weapon Parent";

        print("Assigned Wpn Parent");

    }

    void InstantiateChosenWeapon()
    {
        currentWeaponObject = Instantiate(weapon.prefab, weaponParent.transform, false);
        currentWeaponObject.name = weapon.name;

        print("Instantiated Weapon");
    }


}
