using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public ModeRuntimeData comboMode;

    public GameObject characterAnimationParent;
    public GameObject weaponParent;
    public GameObject currentWeaponObject;

    public virtual CombatEntityController Controls { get; set; }

    protected virtual void Awake()
    {
        Controls = GetComponent<CombatEntityController>();
    }

    private void Start()
    {
        if(gameObject.GetComponent<CombatEntityController>().Mode("Combo") != null)
            comboMode = gameObject.GetComponent<CombatEntityController>().Mode("Combo");

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
