using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(PlayerController))]
public class CombatUI : MonoBehaviour
{
    //Cache
    PlayerController controls;

    #region UI and Basic Functionality

    [Header("UI and Basic Functionality")]
    //Adjustable variables
    public float deadZone = 20f;
    public float mouseInputClamp;
    public float changeCooldown = 0.1f;
    public bool changeOnCooldown;
    public float changeCloseProximityMultiplier = 0.6f;

    //Prefabs
    [SerializeField] private GameObject combatUIParent;

    //Privates
    string lookDir;

    //Adjustable component references
    public Transform locationUpdator;

    [Space]
    [Header("UI References")]
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;

    [SerializeField] private RawImage upImgRef;
    [SerializeField] private RawImage downImgRef;
    [SerializeField] private RawImage leftImgRef;
    [SerializeField] private RawImage rightImgRef;

    #endregion

    private void Awake()
    {

        //Cache
        controls = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        //Input Action Observers
        controls.EnterCombat += EnableUI;
        controls.ExitCombat += DisableUI;
    }

    private void OnDisable()
    {
        //Input Action Observers
        controls.EnterCombat -= EnableUI;
        controls.ExitCombat -= DisableUI;
    }
    private void Start()
    {
        DisableUI();
    }


    private void Update()
    {
        //Updates the Attack Indicator rotation
        if (!changeOnCooldown && combatUIParent.activeInHierarchy)
            UpdateAttackIndicatorRotation(controls.look.ReadValue<Vector2>());
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
        if (lookDir == "right" || lookDir == "left")
        {
            upDZ *= changeCloseProximityMultiplier;
            downDZ *= changeCloseProximityMultiplier;
        }

        if (lookDir == "up" || lookDir == "down")
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
            //print("In deadzone"); 
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

        //Abs value because left right is x and up down is y. if one is greater than the other, we can see if were are going left right or up down
        if (Mathf.Abs(mouseInput.x) > Mathf.Abs(mouseInput.y))
        {
            //Further specifiy which one out of left right, (the right or the left one)
            //Debug.Log(mouseInput);
            if (mouseInput.x > 0)//Right{
                lookDir = "right"; 
            else if (mouseInput.x < 0) //Left
                lookDir = "left";
        }
        else
        {
            if (mouseInput.y > 0)//Up
                lookDir = "up";
            else if (mouseInput.y < 0) //Down
                lookDir = "down";
        }

        //Updates the actual UI with the value specified
        UpdateCombatUIVisuals(lookDir);

        //Pushes the direction to the functionality (and any other listeners)
        controls.SelectCertainAbility?.Invoke(lookDir);
        //Debug.Log(lookDir);



    }

    void ChangeOpacityImage(RawImage img, float opacity)
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
        combatUIParent.SetActive(true);
        ChangeAllImageIcons();
        UpdateAttackIndicatorRotation(new Vector2(0, deadZone + 1));
    }

    /// <summary>
    /// Disabled the root combat ui
    /// </summary>
    /// <param name="targ"></param>
    void DisableUI()
    {
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
    }

    void ChangeAllImageIcons()
    {
        rightImgRef.texture = controls.a_right.icon;
        leftImgRef.texture = controls.a_left.icon;
        upImgRef.texture = controls.a_up.icon;
        downImgRef.texture = controls.a_down.icon;
    }




    #endregion



}
