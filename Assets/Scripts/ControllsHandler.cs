using UnityEngine;
using UnityEngine.InputSystem;

public class ControllsHandler : MonoBehaviour
{
    public PlayerInputActions controls;

    //Input Actions
    public InputAction look;
    public InputAction move;
    public InputAction sprint;
    public InputAction lockOn;


    //Flags
    public bool inCombat;

    private void Awake()
    {
        controls = new PlayerInputActions();

        //Input Cache
        look = controls.Player.Look;
        move = controls.Player.Move;
        sprint = controls.Player.Sprint;
        lockOn = controls.Player.LockOn;
    }


    #region enable/disable movement

    private void OnEnable()
    {
        look.Enable();
        move.Enable();
        sprint.Enable();
        lockOn.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
        sprint.Disable();
        lockOn.Disable();
    }
    #endregion 
}
