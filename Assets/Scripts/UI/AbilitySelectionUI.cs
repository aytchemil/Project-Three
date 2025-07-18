using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // for TextMeshProUGUI


public class AbilitySelectionUI : MonoBehaviour
{
    [Header("Setup")]
    public GameObject allAbilitySetsGO;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject abilityPanelPrefab;
    public AllAbilitiesInModeSO allAbilitiesSO;

    // ✅ Track who opened the selector
    private Button currentSlotButton;
    private Image currentSlotImage;
    private System.Action<Ability> onAbilityChosenCallback;

    private readonly List<GameObject> spawnedPanels = new List<GameObject>();

    private void Awake()
    {
        if (!gridLayoutGroup)
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    /// <summary>
    /// Open the selector for a specific slot button
    /// </summary>
    public void OpenForSlot(Button slotButton, System.Action<Ability> callback = null)
    {
        currentSlotButton = slotButton;
        currentSlotImage = slotButton.GetComponent<Image>();
        onAbilityChosenCallback = callback;

        allAbilitySetsGO.SetActive(true);
        PopulateGrid();
    }

    public void Close()
    {
        foreach (var go in spawnedPanels)
            if (go) Destroy(go);
        spawnedPanels.Clear();

        allAbilitySetsGO.SetActive(false);

        // clear current slot
        currentSlotButton = null;
        currentSlotImage = null;
        onAbilityChosenCallback = null;
    }

    private void PopulateGrid()
    {
        if (!abilityPanelPrefab)
        {
            Debug.LogError("[AbilitySelectionUI] Missing abilityPanelPrefab!");
            return;
        }

        if (!allAbilitiesSO || allAbilitiesSO.abilities.Count == 0)
        {
            Debug.LogWarning("[AbilitySelectionUI] No abilities found in SO.");
            return;
        }

        // Clear existing
        foreach (var go in spawnedPanels)
            if (go) Destroy(go);
        spawnedPanels.Clear();

        foreach (var ability in allAbilitiesSO.abilities)
        {
            if (!ability) continue;

            GameObject panelGO = Instantiate(abilityPanelPrefab, gridLayoutGroup.transform);
            spawnedPanels.Add(panelGO);

            // ✅ Get Button child
            Button abilityButton = panelGO.GetComponentInChildren<Button>();
            if (!abilityButton) continue;

            // ✅ Set icon on Button's Image
            Image abilityIconImage = abilityButton.GetComponent<Image>();
            if (abilityIconImage) abilityIconImage.sprite = ability.icon;

            // ✅ Find TMP Text sibling under prefab and set name
            TextMeshProUGUI nameLabel = panelGO.GetComponentInChildren<TextMeshProUGUI>(true);
            if (nameLabel)
                nameLabel.text = ability.abilityName;

            // ✅ Hook click
            Ability captured = ability;
            abilityButton.onClick.AddListener(() => OnAbilitySelected(captured));
        }
    }

    /// <summary>
    /// When an ability from grid is selected
    /// </summary>
    private void OnAbilitySelected(Ability selectedAbility)
    {
        Debug.Log($"[AbilitySelectionUI] Selected: {selectedAbility.name}");

        // ✅ Update the slot button image
        if (currentSlotImage && selectedAbility.icon)
            currentSlotImage.sprite = selectedAbility.icon;

        // ✅ Callback for storing ability in logic
        onAbilityChosenCallback?.Invoke(selectedAbility);

        // Close selector
        Close();
    }
}
