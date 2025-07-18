using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerController))]
public class CombatUI : MonoBehaviour
{
    //Cache
    PlayerController Controls;

    #region UI and Basic Functionality

    [Header("UI and Basic Functionality")]
    //Adjustable variables
    public float deadZone = 20f;
    public float mouseInputClamp;
    public float changeCooldown = 0.1f;
    public bool changeOnCooldown;
    public float changeCloseProximityMultiplier = 0.6f;
    public int lastComboChoice = 0;

    //Prefabs
    [SerializeField] private GameObject combatUIParent;

    //Adjustable component references
    public Transform locationUpdator;

    [Space]
    [Header("UI References")]
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    [SerializeField] private Image upImgRef;
    [SerializeField] private Image downImgRef;
    [SerializeField] private Image leftImgRef;
    [SerializeField] private Image rightImgRef;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI comboText;

    [SerializeField] private Image[] abilityImgs;



    #endregion

    public virtual void InternalInit()
    {
        Controls = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        InternalInit();
        //Input Action Observers
        Controls.EnterCombat += EnableUI;
        Controls.ExitCombat += DisableUI;

        Controls.switchAbilityMode += SwitchAbilityMode;
        
        for (int i = 0; i < EntityController.AMOUNT_OF_ABIL_SLOTS; i++)
        {
            Controls.abilitySlots[i] += AbilityChoose;
            Controls.abilitySlots[i] += UpdateCurrentAbilityText;
        }

        DisableUI();
    }

    private void OnDisable()
    {
        //Input Action Observers
        Controls.EnterCombat -= EnableUI;
        Controls.ExitCombat -= DisableUI;

        Controls.switchAbilityMode -= SwitchAbilityMode;

        for (int i = 0; i < EntityController.AMOUNT_OF_ABIL_SLOTS; i++)
        {
            Controls.abilitySlots[i] -= AbilityChoose;
            Controls.abilitySlots[i] -= UpdateCurrentAbilityText;
        }
    }


    private void Update()
    {
        //Updates the Attack Indicator rotation
        if (!changeOnCooldown && combatUIParent.activeInHierarchy)
            UpdateAttackIndicatorRotation(Controls.ia_look.ReadValue<Vector2>());

        //Controls.CombatWheelSelectDirection?.Invoke(Controls.lookDir);
    }

    #region UI and Basic Functionality

    /// <summary>
    /// Desc: Updates the location of the attack indicator to the mouseInput's local position
    /// Turns on 1 of 4 indicators, and the respective ability to match the players input
    /// Functionality: Deadzone, the closer proximity abilities have more sensitivity, cooldown,  clamps the input
    /// </summary>
    /// <param name="mouseInput"></param>
    void UpdateAttackIndicatorRotation(Vector2 mouseInput)
    {
        //Debug.Log(mouseInput);

        //Deadzone check for the proximal sensitivity 
        bool inXDeadZone1 = (mouseInput.x < deadZone && mouseInput.x > -deadZone);
        bool inYDeadZone1 = (mouseInput.y < deadZone && mouseInput.y > -deadZone);

        //Save the deadzones
        float leftDZ = deadZone,
            rightDZ = deadZone,
            upDZ = deadZone,
            downDZ = deadZone;

        //Apply the proximal multiplier (lessens) to the sensitivy of the proximal input direction
        if (Controls.lookDir == "right" || Controls.lookDir == "left")
        {
            upDZ *= changeCloseProximityMultiplier;
            downDZ *= changeCloseProximityMultiplier;
        }

        if (Controls.lookDir == "up" || Controls.lookDir == "down")
        {
            leftDZ *= changeCloseProximityMultiplier;
            rightDZ *= changeCloseProximityMultiplier;
        }



        //Set the new deadzones based on the proximities
        bool inXDeadZone2 = (mouseInput.x < rightDZ && mouseInput.x > -leftDZ);
        bool inYDeadZone2 = (mouseInput.y < upDZ && mouseInput.y > -downDZ);

        //Debug to view the proximiities
        //Debug.Log(rightDZ + " " + leftDZ + " " + upDZ + " " + downDZ + "" + mouseInput);

        //Return gates for: cooldown and if in deadzone
        if (changeOnCooldown) return;
        if (inXDeadZone2 && inYDeadZone2) 
        {
            //Debug.Log("mouse " + mouseInput + " dz");
            return; 
        }

        //Start the cooldown of time changeCooldown
        changeOnCooldown = true;
        Invoke("StopCoolDown", changeCooldown);

        //Clamp the mouseInputs (Idk if its neccessary)
        mouseInput.x = Mathf.Clamp(mouseInput.x, -mouseInputClamp, mouseInputClamp);
        mouseInput.y = Mathf.Clamp(mouseInput.y, -mouseInputClamp, mouseInputClamp);


        //Update the locational updator visual's position
        locationUpdator.localPosition = mouseInput;

        //mouseInput vlaues visualized
        // (-1, 0) left
        // (1, 0) right
        // (0, 1) up
        // (0, -1) down

        //Further specifiy which one out of left right, (the right or the left one)
        //Debug.Log("mouse " + mouseInput);
        string newLookdir = GetHighestDirection(mouseInput.x, -mouseInput.x, mouseInput.y, -mouseInput.y);

        Controls.lookDir = newLookdir;
        UpdateCombatUIVisuals(newLookdir);

        string GetHighestDirection(float right, float left, float up, float down)
        {
            float maxValue = Mathf.Max(right, left, up, down);

            if (maxValue == right)
                return "right";
            if (maxValue == left)
                return "left";
            if (maxValue == up)
                return "up";
            if (maxValue == down)
                return "down";

            return ""; // Fallback in case all values are equal and zero
        }
    }

    void ChangeOpacityImage(Image img, float opacity)
    {
        Color color = img.color;
        color.a = opacity;
        img.color = color;
    }

    /// <summary>
    /// Enables the The root combat UI
    /// </summary>
    /// <param name="targ"></param>
    void EnableUI()
    {
        //print("enabling ui");
        combatUIParent.SetActive(true);
        SwitchAbilityMode();
        UpdateAttackIndicatorRotation(new Vector2(0, deadZone + 1));
        InitComboWheel();
        AbilityChoose(lastComboChoice);
    }

    /// <summary>
    /// Disabled the root combat ui
    /// </summary>
    /// <param name="targ"></param>
    void DisableUI()
    {
       // print("disabling ui");
        combatUIParent.SetActive(false);
    }

    /// <summary>
    /// This is put in a method because it needs to be invoked with a delay for the cooldown
    /// </summary>
    void StopCoolDown()
    {
        changeOnCooldown = false;
    }

    /// <summary>
    /// Manually update the visuals by their references
    /// </summary>
    /// <param name="lookDir"></param>
    void UpdateCombatUIVisuals(string lookDir)
    {
        switch (lookDir)
        {
            case "right":
                ChangeOpacityImage(rightImgRef, 1);
                ChangeOpacityImage(leftImgRef, 0.2f);
                ChangeOpacityImage(upImgRef, 0.2f);
                ChangeOpacityImage(downImgRef, 0.2f);

                right.SetActive(true);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(false);
                break;
            case "left":
                ChangeOpacityImage(rightImgRef, 0.2f);
                ChangeOpacityImage(leftImgRef, 1);
                ChangeOpacityImage(upImgRef, 0.2f);
                ChangeOpacityImage(downImgRef, 0.2f);

                right.SetActive(false);
                left.SetActive(true);
                up.SetActive(false);
                down.SetActive(false);
                break;
            case "up":
                ChangeOpacityImage(rightImgRef, 0.2f);
                ChangeOpacityImage(leftImgRef, 0.2f);
                ChangeOpacityImage(upImgRef, 1);
                ChangeOpacityImage(downImgRef, 0.2f);

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(true);
                down.SetActive(false);
                break;
            case "down":
                ChangeOpacityImage(rightImgRef, 0.2f);
                ChangeOpacityImage(leftImgRef, 0.2f);
                ChangeOpacityImage(upImgRef, 0.2f);
                ChangeOpacityImage(downImgRef, 1);

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(true);
                break;
        }

        SetWheelInfo();
    }

    void UpdateCurrentAbilityText(int slot)
    {
        Ability currentAbility = Controls.Mode(Controls.mode).ability;
        text.text = "Current Ability: " + currentAbility.abilityName;
    }

    void SetWheelIcons()  
    {
        rightImgRef.sprite = Controls.Mode("Combo").data.abilitySet.right.icon;
        leftImgRef.sprite = Controls.Mode("Combo").data.abilitySet.left.icon;
        upImgRef.sprite = Controls.Mode("Combo").data.abilitySet.up.icon;
        downImgRef.sprite = Controls.Mode("Combo").data.abilitySet.down.icon;
    }

    void SetModeInfo()
    {
        modeText.text = "ABILITIES: " + Controls.CurMode().data.modeTextDesc;
        //print("updated mode ui");

    }

    void SetWheelInfo()
    {
        if(Controls.Mode("Combo").ability != null)
            comboText.text = "COMBO: " + Controls.Mode("Combo").ability.abilityName;
    }


    #endregion

    void SwitchAbilityMode()
    {
        //print("Updating mode UIS");
        SetWheelIcons();
        SetModeInfo();
        SetAbilityImgs();
    }

    void InitComboWheel()
    {
        Color less = new Color(1, 1, 1, 0.2f);
        Color more = new Color(1, 1, 1, 1);

        rightImgRef.sprite = Controls.Mode("Combo").data.abilitySet.right.icon;
        leftImgRef.sprite = Controls.Mode("Combo").data.abilitySet.left.icon;
        upImgRef.sprite = Controls.Mode("Combo").data.abilitySet.up.icon;
        downImgRef.sprite = Controls.Mode("Combo").data.abilitySet.down.icon;

        rightImgRef.color = more;
        leftImgRef.color = less;
        upImgRef.color = less;
        downImgRef.color = less;
    }

    void SetAbilityImgs()
    {
        //print("Current mode:" + Controls.mode);
        abilityImgs[0].sprite = Controls.Mode(Controls.mode).data.abilitySet.right.icon;
        abilityImgs[1].sprite = Controls.Mode(Controls.mode).data.abilitySet.left.icon;
        abilityImgs[2].sprite = Controls.Mode(Controls.mode).data.abilitySet.up.icon;
        abilityImgs[3].sprite = Controls.Mode(Controls.mode).data.abilitySet.down.icon;
    }

    void AbilityChoose(int choice)
    {
        Color less = new Color(1, 1, 1, 0.2f);
        Color more = new Color(1, 1, 1, 1);
        lastComboChoice = choice;

        if (Controls.CurMode().data.abilityIndividualSelection == true)
        {
            //print($"[CombatUI] Chosen ability is [{choice}]");
            foreach (Image img in abilityImgs)
                img.color = less;

            abilityImgs[choice].color = more;
        }
        else
            foreach (Image img in abilityImgs)
                img.color = more;
        //TODO LATER: Denote its used, mabye cooldowns idk

    }

}
