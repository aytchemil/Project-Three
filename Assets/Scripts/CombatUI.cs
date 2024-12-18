using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatUI : MonoBehaviour
{
    ControllsHandler controlls;
    public Transform abilityIndicator;
    public Transform locationUpdator;
    public float deadZone = 2.5f;
    public float mouseInputClamp;

    public string lookDir;

    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;


    private void Awake()
    {
        controlls = GetComponent<ControllsHandler>();
    }


    private void Update()
    {

        UpdateAttackIndicatorRotation(controlls.look.ReadValue<Vector2>());
    }


    void UpdateAttackIndicatorRotation(Vector2 mouseInput)
    {
        Debug.Log(mouseInput);
        bool inXDeadZone = (mouseInput.x < deadZone && mouseInput.x > -deadZone);
        bool inYDeadZone = (mouseInput.y < deadZone && mouseInput.y > -deadZone);

        if (inXDeadZone && inYDeadZone) { print("In deadzone"); return; }

        mouseInput.x = Mathf.Clamp(mouseInput.x, -mouseInputClamp, mouseInputClamp);
        mouseInput.y = Mathf.Clamp(mouseInput.y, -mouseInputClamp, mouseInputClamp);
        locationUpdator.localPosition = mouseInput;

        // (-1, 0) left
        // (1, 0) right
        // (0, 1) up
        // (0, -1) down


        if (Mathf.Abs(mouseInput.x) > Mathf.Abs(mouseInput.y))
        {
            if (mouseInput.x > 0)//Right{
            {

                lookDir = "right";

                right.SetActive(true);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(false);
            }
            else if(mouseInput.x < 0) //Left
            {
                lookDir = "left";

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

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(true);
                down.SetActive(false);

            }
            else if(mouseInput.y < 0) //Down
            {
                lookDir = "down";

                right.SetActive(false);
                left.SetActive(false);
                up.SetActive(false);
                down.SetActive(true);

            }

        }
        Debug.Log(lookDir);



    }


    

}
