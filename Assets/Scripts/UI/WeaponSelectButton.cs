using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectButton : MonoBehaviour
{
    public AbilitySelectionUI abilitySelectionUI;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            abilitySelectionUI.OpenWeaponPanel();
        });
    }
}