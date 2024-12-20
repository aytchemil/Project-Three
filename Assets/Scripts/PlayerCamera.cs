using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerController))]
public class PlayerCamera : MonoBehaviour
{
    //Real-Time Adjustable Variables
    public float xSens;
    public float ySens;
    public Vector3 cameraOffset = new Vector3(0, 1.8f, -3);
    public float smoothTime = 0.3f;
    public float camLerpTime = 0.01f;
    public Vector3 vel = Vector3.zero;


    //Adjustable Component References
    public Transform camOrientation;
    public Transform cameraPosition;
    public Camera myCamera;

    //Cached Component References
    PlayerController controls;
    Transform camTransform;
    CombatEntityController target;

    //Privates
    public Vector3 savedTargetLocation;
    Vector2 currentXY;
    Vector2 finalUnlockedXYPos;

    //Flags
    bool inCombat = false;

    public float yRot;
    public float xRot;


    private void Awake()
    {
        //Cache
        controls = GetComponent<PlayerController>();
        camTransform = myCamera.GetComponent<Transform>(); 
        //Debug.Log(camTransform);
    }

    private void OnEnable()
    {
        //Mouse lock states
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        inCombat = false;

        //Callback Additions
        controls.CombatFollowTarget += EnterCombatAndFollowTarget;
        controls.ExitCombat += ExitCombat;
    }

    private void OnDisable()
    {
        controls.CombatFollowTarget -= EnterCombatAndFollowTarget;
        controls.ExitCombat -= ExitCombat;
    }


    private void Update()
    {
        //If not in combat,"Look regularly". else "combat look"
        if (!inCombat)
        {
            MouseLooking(controls.look.ReadValue<Vector2>());
            //set the current xRot yRot
            GetCurrentXY();

            //Re clamp the xRot and yRot so it don't go over 360 or under 360
            if (xRot > 180f) xRot -= 360f;
            if (yRot > 180f) yRot -= 360f;
            if (xRot < -180f) xRot += 360f;
            if (yRot < -180f) yRot += 360f;
        }
        else
        {
            //"combat look"
            if (target != null)
            {
                //Debug.Log("Locked onto target by cam");
                CameraLookAtLockTarget(target.transform.position);
                TransformLookAtTarget(target.transform.position);
                UpdateNewXY();
            }
            else
                Debug.LogError("Player Camera: In combat with enemy that doesnt exist");
        }

        //Updates the final camera position to the one's specified in the update
        //UpdateCamPosition();
    }


    private void LateUpdate()
    {
        //Updates the final camera position to the one's specified in the update
        UpdateCamPosition();
    }


    /// <summary>
    /// Sets the horizontal and vertical rotations of the camera alongside the horizontal of the transform
    /// </summary>
    /// <param name="lookInput"></param>
    void MouseLooking(Vector2 lookInput)
    {
        //Debug.Log("Mouse Looking");

        //Cache's the X and Y positions from the Input System
        Vector2 xyRot = CalculateXYRot(lookInput);

        //Sets the rotation to the player inputted rotations
        transform.rotation = Quaternion.Euler(0, xyRot.x, 0);
        camOrientation.rotation = Quaternion.Euler(xyRot.y, xyRot.x, 0);
    }

    /// <summary>
    /// Returns the XY rot with the sens from a given raw input XY
    /// </summary>
    /// <param name="rawXY"></param>
    /// <returns></returns>
    Vector2 CalculateXYRot(Vector2 rawXY)
    {
        //Cache's the X and Y positions from the Input System

        float smoothFactor = 0.1f;
        xRot = Mathf.Lerp(xRot, xRot + rawXY.x * (xSens / 5), smoothFactor);
        yRot = Mathf.Lerp(yRot, yRot + rawXY.y * (ySens / 5) * -1, smoothFactor);

        //xRot += rawXY.x * (xSens / 5);
        //yRot += rawXY.y * (ySens / 5) * -1;

        //Restricts the looking of UP and DOWN
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        return new Vector2(xRot, yRot);
    }

    /// <summary>
    /// Makes the camera look at a given target restricted with X
    /// </summary>
    /// <param name="target"></param>
    void CameraLookAtLockTarget(Vector3 target)
    {
        camOrientation.LookAt(target);
        camOrientation.localEulerAngles = new Vector3(camOrientation.localEulerAngles.x, 0, 0);

    }

    /// <summary>
    /// Transform looks at a target restricted to Y
    /// </summary>
    /// <param name="target"></param>
    void TransformLookAtTarget(Vector3 target)
    {
        transform.LookAt(target);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
    private Vector3 positionVelocity = Vector3.zero; // Velocity for position SmoothDamp
    private Quaternion rotationVelocity;             // Placeholder for rotation smoothing

    /// <summary>
    /// Sets the Cam position to the desired location set by this script
    /// </summary>
    void UpdateCamPosition()
    {
        camTransform.position = cameraPosition.position;
        camTransform.rotation = cameraPosition.rotation;
    }

    /// <summary>
    /// Observer Method for the player camera to enter combat
    /// </summary>
    /// <param name="target"></param>
    void EnterCombatAndFollowTarget(CombatEntityController target)
    {
        Debug.Log("Entering Combat and following target: " + target);
        inCombat = true;
        this.target = target;
    }

    /// <summary>
    /// Observer method player exiting combat
    /// </summary>
    /// <param name="target"></param>
    void ExitCombat()
    {
        //Debug.Log("Exiting Combat");

        UpdateNewXY();
        ApplyNewXYPosition();

        inCombat = false;
        this.target = null;
    }


    /// <summary>
    /// Gets the current XY from local euler angles, for the CAM and TRANSFORM
    /// </summary>
    /// <returns></returns>
    Vector2 GetCurrentXY()
    {
        currentXY.y = transform.localEulerAngles.y;
        currentXY.x = camTransform.localEulerAngles.x;

        if (currentXY.y > 180f) currentXY.y -= 360f;
        if (currentXY.x > 180f) currentXY.x -= 360f;
        if (currentXY.y < -180f) currentXY.y += 360f;
        if (currentXY.x < -180f) currentXY.x += 360f;

        return new Vector2(currentXY.x, currentXY.y);
    }

    /// <summary>
    /// Applies XY rots of the final rotations when the player unlocks combat. Providing a smooth transition from combat to out-of-combat
    /// </summary>
    void ApplyNewXYPosition()
    {
        xRot = finalUnlockedXYPos.y;
        yRot = finalUnlockedXYPos.x;

        //Debug.Log("NEW X : " + xRot + " NEW Y: " + yRot);
    }

    /// <summary>
    /// Sets the final unlocked XY to the current XY
    /// </summary>
    void UpdateNewXY()
    {
        finalUnlockedXYPos = GetCurrentXY();
    }


}
