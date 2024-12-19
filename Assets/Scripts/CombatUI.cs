using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CombatUI : MonoBehaviour
{

    ControllsHandler controlls;
    public Transform abilityIndicator;
    public Transform locationUpdator;
    public float deadZone = 2.5f;
    public float mouseInputClamp;
    public float changeCooldown = 0.1f;
    public bool changeOnCooldown;
    public float changeCloseProximityMultiplier = 0.6f;

    public GameObject combatUIGameObject;

    public string lookDir;

    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    public RawImage upImg;
    public RawImage downImg;
    public RawImage leftImg;
    public RawImage rightImg;


    private void Awake()
    {
        controlls = GetComponent<ControllsHandler>();
        controlls.EnterCombat += EnableUI;
        controlls.ExitCombat += DisableUI;
    }

    private void OnDisable()
    {
        controlls.EnterCombat -= EnableUI;
        controlls.ExitCombat -= DisableUI;
    }
    private void Start()
    {
        DisableUI(null);
    }


    private void Update()
    {
        if(!changeOnCooldown)
            UpdateAttackIndicatorRotation(controlls.look.ReadValue<Vector2>());
    }


    void UpdateAttackIndicatorRotation(Vector2 mouseInput)
    {
        //Debug.Log(mouseInput);


        bool inXDeadZone1 = (mouseInput.x < deadZone && mouseInput.x > -deadZone);
        bool inYDeadZone1 = (mouseInput.y < deadZone && mouseInput.y > -deadZone);

        float leftDZ = deadZone,
            rightDZ = deadZone,
            upDZ = deadZone,
            downDZ = deadZone;

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




        bool inXDeadZone2 = (mouseInput.x < rightDZ && mouseInput.x > -leftDZ);
        bool inYDeadZone2 = (mouseInput.y < upDZ && mouseInput.y > -downDZ);

        Debug.Log(rightDZ + " " + leftDZ + " " + upDZ + " " + downDZ + "" + mouseInput);


        if (changeOnCooldown) return;
        if (inXDeadZone2 && inYDeadZone2) { print("In deadzone"); return; }

        changeOnCooldown = true;
        Invoke("StopCoolDown", changeCooldown);

        mouseInput.x = Mathf.Clamp(mouseInput.x, -mouseInputClamp, mouseInputClamp);
        mouseInput.y = Mathf.Clamp(mouseInput.y, -mouseInputClamp, mouseInputClamp);


        locationUpdator.localPosition = mouseInput;


        // (-1, 0) left
        // (1, 0) right
        // (0, 1) up
        // (0, -1) down


        if (Mathf.Abs(mouseInput.x) > Mathf.Abs(mouseInput.y))
        {
            Debug.Log(mouseInput);
            if (mouseInput.x > 0)//Right{
            {

                lookDir = "right";

                ChangeOpacityImage(rightImg, 1);
                ChangeOpacityImage(leftImg, 0.2f);
                ChangeOpacityImage(upImg, 0.2f);
                ChangeOpacityImage(downImg, 0.2f);

                right.SetActive(true);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(false);
            }
            else if(mouseInput.x < 0) //Left
            {
                lookDir = "left";

                ChangeOpacityImage(rightImg, 0.2f);
                ChangeOpacityImage(leftImg, 1);
                ChangeOpacityImage(upImg, 0.2f);
                ChangeOpacityImage(downImg, 0.2f);

                right.SetActive(false);
                left.SetActive(true);
                up.SetActive(false);
                down.SetActive(false);
            }
        }
        else
        {
            if (mouseInput.y > 0)//Up
            {
                lookDir = "up";

                ChangeOpacityImage(rightImg, 0.2f);
                ChangeOpacityImage(leftImg, 0.2f);
                ChangeOpacityImage(upImg, 1);
                ChangeOpacityImage(downImg, 0.2f);

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(true);
                down.SetActive(false);

            }
            else if(mouseInput.y < 0) //Down
            {
                lookDir = "down";

                ChangeOpacityImage(rightImg, 0.2f);
                ChangeOpacityImage(leftImg, 0.2f);
                ChangeOpacityImage(upImg, 0.2f);
                ChangeOpacityImage(downImg, 1);

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(true);

            }

        }
        Debug.Log(lookDir);



    }

    void ChangeOpacityImage(RawImage img, float opacity)
    {
        Color color = img.color;
        color.a = opacity;
        img.color = color;
    }

    void EnableUI(CombatEntity targ)
    {
        combatUIGameObject.SetActive(true);
    }

    void DisableUI(CombatEntity targ)
    {
        combatUIGameObject.SetActive(false);
    }

    void StopCoolDown()
    {
        changeOnCooldown = false;
    }

}
