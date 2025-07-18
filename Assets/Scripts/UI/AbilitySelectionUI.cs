using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AbilitySelectionUI : MonoBehaviour
{
    [Header("Ability Grid Setup")]
    public GameObject allAbilitySetsGO;
    public GridLayoutGroup abilityGridLayout;
    public GameObject abilityPanelPrefab;
    public AllAbilitiesInModeSO allAbilitiesSO;

    [Header("Weapon Grid Setup")]
    public GameObject weaponPanelGO;                // weapon panel container
    public GridLayoutGroup weaponGridLayout;        // weapon grid layout
    public GameObject weaponPanelPrefab;            // prefab for weapon entries
    public AllWeaponSO allWeaponsSO;               // list of all weapons

    [Header("Weapon Select Button UI")]
    public Button weaponSelectButton;         // the button you click to open weapon grid
    public Image weaponButtonImage;           // the image on that button
    public TextMeshProUGUI weaponButtonText;  // the TMP text on that button

    // currently selected weapon
    private Weapon currentSelectedWeapon;

    // ✅ Track who opened the selector for abilities
    private Button currentSlotButton;
    private Image currentSlotImage;
    private System.Action<Ability> onAbilityChosenCallback;

    private readonly List<GameObject> spawnedAbilityPanels = new List<GameObject>();
    private readonly List<GameObject> spawnedWeaponPanels = new List<GameObject>();

    private void Awake()
    {
        if (!abilityGridLayout)
            abilityGridLayout = allAbilitySetsGO.GetComponentInChildren<GridLayoutGroup>();
        if (!weaponGridLayout && weaponPanelGO)
            weaponGridLayout = weaponPanelGO.GetComponentInChildren<GridLayoutGroup>();

        if (weaponButtonText != null)
            weaponButtonText.text = string.Empty;
    }

    private void Start()
    {
        HookWeaponButton();
    }

    // ========================
    // ABILITY GRID FUNCTIONS
    // ========================

    public void OpenForSlot(Button slotButton, System.Action<Ability> callback = null)
    {
        currentSlotButton = slotButton;
        currentSlotImage = slotButton.GetComponent<Image>();
        onAbilityChosenCallback = callback;

        allAbilitySetsGO.SetActive(true);
        weaponPanelGO?.SetActive(false); // make sure weapon panel is hidden
        PopulateAbilityGrid();
    }

    public void Close()
    {
        // Clear ability grid
        foreach (var go in spawnedAbilityPanels)
            if (go) Destroy(go);
        spawnedAbilityPanels.Clear();

        allAbilitySetsGO.SetActive(false);

        // clear current slot
        currentSlotButton = null;
        currentSlotImage = null;
        onAbilityChosenCallback = null;
    }

    private void PopulateAbilityGrid()
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
        foreach (var go in spawnedAbilityPanels)
            if (go) Destroy(go);
        spawnedAbilityPanels.Clear();

        foreach (var ability in allAbilitiesSO.abilities)
        {
            if (!ability) continue;

            GameObject panelGO = Instantiate(abilityPanelPrefab, abilityGridLayout.transform);
            spawnedAbilityPanels.Add(panelGO);

            // ✅ Get Button child
            Button abilityButton = panelGO.GetComponentInChildren<Button>();
            if (!abilityButton) continue;

            // ✅ Set icon on Button's Image
            Image abilityIconImage = abilityButton.GetComponent<Image>();
            if (abilityIconImage) abilityIconImage.sprite = ability.icon;

            // ✅ Set TMP Text
            TextMeshProUGUI nameLabel = panelGO.GetComponentInChildren<TextMeshProUGUI>(true);
            if (nameLabel) nameLabel.text = ability.abilityName;

            // ✅ Hook click
            Ability captured = ability;
            abilityButton.onClick.AddListener(() => OnAbilitySelected(captured));
        }
    }

    private void OnAbilitySelected(Ability selectedAbility)
    {
        Debug.Log($"[AbilitySelectionUI] Selected Ability: {selectedAbility.abilityName}");

        // ✅ Update the slot button image
        if (currentSlotImage && selectedAbility.icon)
            currentSlotImage.sprite = selectedAbility.icon;

        // ✅ Callback for storing ability in logic
        onAbilityChosenCallback?.Invoke(selectedAbility);

        // ✅ Also ensure weapon panel is closed
        CloseWeaponPanel();

        // ✅ Close selector
        Close();
    }

    public void ChangeAllAbilitiesListSO(AllAbilitiesInModeSO newAbilities)
    {
        allAbilitiesSO = newAbilities;
    }

    // ========================
    // WEAPON GRID FUNCTIONS
    // ========================

    public void OpenWeaponPanel()
    {
        // ✅ Hide ability panel and show weapon panel
        allAbilitySetsGO.SetActive(false);
        if (weaponPanelGO != null)
        {
            weaponPanelGO.SetActive(true);
            PopulateWeaponGrid();
        }
    }

    public void CloseWeaponPanel()
    {
        if (weaponPanelGO == null) return;

        foreach (var go in spawnedWeaponPanels)
            if (go) Destroy(go);
        spawnedWeaponPanels.Clear();

        weaponPanelGO.SetActive(false);
    }

    private void PopulateWeaponGrid()
    {
        if (!weaponPanelPrefab)
        {
            Debug.LogError("[AbilitySelectionUI] Missing weaponPanelPrefab!");
            return;
        }

        if (!allWeaponsSO || allWeaponsSO.weapons.Count == 0)
        {
            Debug.LogWarning("[AbilitySelectionUI] No weapons found in SO.");
            return;
        }

        foreach (var go in spawnedWeaponPanels)
            if (go) Destroy(go);
        spawnedWeaponPanels.Clear();

        foreach (var weapon in allWeaponsSO.weapons)
        {
            if (!weapon) continue;

            GameObject panelGO = Instantiate(weaponPanelPrefab, weaponGridLayout.transform);
            spawnedWeaponPanels.Add(panelGO);

            Button weaponButton = panelGO.GetComponentInChildren<Button>();
            if (!weaponButton) continue;

            Image weaponIconImage = weaponButton.GetComponent<Image>();
            if (weaponIconImage) weaponIconImage.sprite = weapon.icon;

            TextMeshProUGUI weaponName = panelGO.GetComponentInChildren<TextMeshProUGUI>(true);
            if (weaponName) weaponName.text = weapon.name;

            Weapon capturedWeapon = weapon;
            weaponButton.onClick.AddListener(() => OnWeaponSelected(capturedWeapon));
        }
    }
    private void OnWeaponSelected(Weapon selectedWeapon)
    {
        Debug.Log($"[AbilitySelectionUI] Selected Weapon: {selectedWeapon.name}");

        currentSelectedWeapon = selectedWeapon;

        // ✅ Update weapon button icon/text
        if (weaponButtonImage && selectedWeapon.icon)
            weaponButtonImage.sprite = selectedWeapon.icon;

        if (weaponButtonText)
            weaponButtonText.text = selectedWeapon.name;

        // ✅ Update weapon handler
        if (weaponSelectButton.gameObject.GetComponent<WeaponHandler>() != null)
            weaponSelectButton.gameObject.GetComponent<WeaponHandler>().SetWeapon(selectedWeapon);

        // ✅ Close weapon panel & return to ability panel
        CloseWeaponPanel();
        allAbilitySetsGO.SetActive(true);
    }

    public void HookWeaponButton()
    {
        if (weaponSelectButton != null)
        {
            weaponSelectButton.onClick.RemoveAllListeners();
            weaponSelectButton.onClick.AddListener(OpenWeaponPanel);
        }
    }
}
