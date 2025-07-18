using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySideButton : MonoBehaviour
{
    [Header("Setup")]
    public AbilitySelectionUI abilitySelector;  // assign in Inspector
    public AbilitySetHandler handler;           // assign your AbilitySetHandler

    private Button myButton;
    private Image myImage;
    private TextMeshProUGUI nameText; // sibling text

    void Awake()
    {
        myButton = GetComponent<Button>();
        myImage = GetComponent<Image>();

        // ✅ Find the sibling TMP text under the same parent
        foreach (Transform sibling in transform.parent)
        {
            if (sibling != this.transform)
            {
                TextMeshProUGUI tmp = sibling.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    nameText = tmp;
                    break;
                }
            }
        }

        // ✅ Clear text initially
        if (nameText != null)
            nameText.text = string.Empty;

        myButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnSlotClicked()
    {
        abilitySelector.OpenForSlot(myButton, OnAbilityPickedFromSelector);
    }

    private void OnAbilityPickedFromSelector(Ability ability)
    {
        if (ability != null)
        {
            // ✅ Update icon
            if (ability.icon != null)
                myImage.sprite = ability.icon;

            // ✅ Update sibling TMP text
            if (nameText != null)
                nameText.text = ability.abilityName;

            // ✅ Tell handler to assign
            handler.ChangeAbility(ability);

            Debug.Log($"[AbilitySideButton] Updated icon + sibling name → {ability.name}");
        }
        else
        {
            // ✅ If null ability selected, clear
            if (nameText != null)
                nameText.text = string.Empty;

            myImage.sprite = null;
            Debug.Log("[AbilitySideButton] Cleared selection");
        }
    }
}
