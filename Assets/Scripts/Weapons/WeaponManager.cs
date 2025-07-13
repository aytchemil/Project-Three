using System.Collections.Generic;
using UnityEngine;
using static EntityController;

public class WeaponManager : MonoBehaviour
{
    RuntimeModeData comboMode;

    GameObject characterAnimationParent;
    GameObject weaponParent;
    GameObject currentWeaponObject;

    public virtual EntityController Controls { get; set; }

    protected virtual void Awake()
    {
        Controls = GetComponent<EntityController>();
    }

    private void OnEnable()
    {
        Controls.Init += Init;
    }

    private void OnDisable()
    {
        Controls.Init -= Init;
    }

    private void Init()
    {
        if (gameObject.GetComponent<EntityController>().Mode("Combo") != null)
            comboMode = gameObject.GetComponent<EntityController>().Mode("Combo");

        characterAnimationParent = Controls.animController.gameObject;

        SetAbilitySet();
        AssignWeaponParent();
        InstantiateChosenWeapon();
    }

    void SetAbilitySet()
    {
        //print(Controls.AbilitySet("Combo").name);

        int index = Controls.abilitySetInputs.FindIndex(x => x == Controls.AbilitySet("Combo"));

        if (index != -1) // Check if the item was found
        {
            Controls.abilitySetInputs[index] = Controls.currentWeapon.chosenAbilitySet;
        }
        else
            Debug.LogError("AbilitySet 'Combo' not found!");

    }

    void AssignWeaponParent()
    {
        weaponParent = Instantiate(new GameObject(), characterAnimationParent.transform, false);
        weaponParent.name = "Weapon Parent";
    }

    void InstantiateChosenWeapon()
    {
        currentWeaponObject = Instantiate(Controls.currentWeapon.prefab, weaponParent.transform, false);
    }


}
