using UnityEngine;
using TMPro;
using UnityEngine.Events;

using System.Collections.Generic;

public class DropdownHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    // Unity Events for each option
    [SerializeField] private List<UnityEvent> onOptionSelected;


    void Start()
    {
        // Ensure the dropdown is linked
        if (dropdown == null)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        // Add listener for when the value changes
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Method that gets called when the dropdown value changes
    private void OnDropdownValueChanged(int index)
    {
        onOptionSelected[index]?.Invoke();
    }

    private void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }
}
