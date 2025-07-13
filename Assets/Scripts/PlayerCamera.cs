using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCamera : Look
{
    [Header("Player Camera")]
    [Header("Player Camera: Real-Time Variables")]
    public float xSens;
    public float ySens;
    public Vector3 cameraOffset = new Vector3(0, 1.8f, -3);
    [Space]
    public float sprintFov;
    public float defaultFov;
    public float combatFOV;
    public float fovLerpSpeed = 0.03f;
    [Space]
    [Header("Player Camera: Adjustable Component References")]
    public Camera myCamera;

    //Privates
    Transform camTransform;
    public Vector3 savedTargetLocation;
    Vector2 currentXY;
    Vector2 finalUnlockedXYPos;
    Vector3 vel = Vector3.zero;

    float yRot;
    float xRot;


    public override void Awake()
    {
        base.Awake();
        camTransform = myCamera.GetComponent<Transform>(); 
        //Debug.Log(camTransform);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //Mouse lock states
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        //Callback Additions
        controls.ExitCombat += ExitCombat;

        //Fov
        controls.sprintStart += OnSprint;
        controls.sprintStop += StopSprint;

        controls.EnterCombat += OnCombatFOV;
        controls.ExitCombat += OnStopCombatFOV;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        controls.ExitCombat -= ExitCombat;

        //Fov
        controls.sprintStart -= OnSprint;
        controls.sprintStop -= StopSprint;

        controls.EnterCombat -= OnCombatFOV;
        controls.ExitCombat -= OnStopCombatFOV;
    }


    private void Update()
    {
        //If not in combat,"Look regularly". else "combat look"
        if (!controls.isLockedOn)
        {
           // Debug.Log("Mouse looking normally");
            MouseLooking(controls.look != null ? controls.look.Invoke() : Vector2.zero);
            //set the current xRot yRot
            GetCurrentXY();

            //Re clamp the xRot and yRot so it don't go over 360 or under 360
            if (xRot > 180f) xRot -= 360f;
            if (yRot > 180f) yRot -= 360f;
            if (xRot < -180f) xRot += 360f;
            if (yRot < -180f) yRot += 360f;
        }

        //Updates the final camera position to the one's specified in the update
        //UpdateCamPosition();
    }


    private void LateUpdate()
    {
        //Updates the final camera position to the one's specified in the update
        UpdateCamPosition();
    }

    #region Player MouseLook Specific

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
        //print(xyRot);
        transform.rotation = Quaternion.Euler(0, xyRot.x, 0);
        cameraPosition.rotation = Quaternion.Euler(xyRot.y, xyRot.x, 0);
    }




    /// <summary>
    /// Sets the Cam position to the desired location set by this script
    /// </summary>
    void UpdateCamPosition()
    {
        //print("updating cam position");
        camTransform.position = cameraPosition.position;
        camTransform.rotation = cameraPosition.rotation;
    }

    #endregion

    #region XY calc and setting

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

    #endregion

    #region Combat

    /// <summary>
    /// Observer Method for the player camera to enter combat
    /// </summary>
    /// <param name="target"></param>
    public override void InCombatFollowingTarget(EntityController target)
    {
        base.InCombatFollowingTarget(target);
        UpdateNewXY();
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
    }

    #endregion

    #region sprinting
    void OnSprint()
    {
        // print("onsprint");
        StartCoroutine(SprintFOV());
    }

    void StopSprint()
    {
        //print("on stop sprint");
        StopCoroutine(SprintFOV());
        StartCoroutine(StopSprintFOV());
    }

    #endregion

    #region FOV's



    void OnCombatFOV()
    {
       // print("oncombat fov");
        StartCoroutine(CombatFOV());
    }

    void OnStopCombatFOV()
    {
        //print("on stop combat fov");
        StopCoroutine(CombatFOV());

        StartCoroutine(StopCombatFOV());
    }

    protected IEnumerator SprintFOV()
    {
        float val = 0;

        while(gameObject.GetComponent<Movement>().state == EntityStates.CurrentState.sprinting && myCamera.fieldOfView < sprintFov)
        {
            val += fovLerpSpeed;

            myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, sprintFov, val);


            yield return new WaitForEndOfFrame();
            //print("spr");
        }
    }

    protected IEnumerator StopSprintFOV()
    {
        float val = 0;

        while (gameObject.GetComponent<Movement>().state != EntityStates.CurrentState.sprinting && myCamera.fieldOfView > defaultFov)
        {
            val += fovLerpSpeed;

            myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, defaultFov, val);


            yield return new WaitForEndOfFrame();
            //print("not sptr" + val);
        }

    }

    protected IEnumerator CombatFOV()
    {
        float val = 0;

        while (gameObject.GetComponent<Movement>().state == EntityStates.CurrentState.combat && myCamera.fieldOfView > combatFOV)
        {
            val += fovLerpSpeed;

            myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, combatFOV, val);


            yield return new WaitForEndOfFrame();
            //print("cmbat");
        }
    }

    protected IEnumerator StopCombatFOV()
    {
        //print("starting stop comabt fov");
        float val = 0;

        while (gameObject.GetComponent<Movement>().state != EntityStates.CurrentState.combat && myCamera.fieldOfView < defaultFov)
        {
            val += fovLerpSpeed;

            myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, defaultFov, val);


            yield return new WaitForEndOfFrame();
            //print("not cmbat" + val);
        }

    }

    #endregion


}
