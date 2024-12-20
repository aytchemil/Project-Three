using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombatFunctionality : CombatFunctionality
{
    private PlayerController playerControls;

    // Override the base property to return PlayerController instead
    protected override CombatEntityController Controls
    {
        get => playerControls;
        set => playerControls = value as PlayerController;
    }

    protected override void Awake()
    {
        //Not calling base, because base uses only the CombatEntityController

        Controls = GetComponent<PlayerController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerControls.attack.performed += ctx => UseAttackAbility();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerControls.attack.performed -= ctx => UseAttackAbility();
    }
}
