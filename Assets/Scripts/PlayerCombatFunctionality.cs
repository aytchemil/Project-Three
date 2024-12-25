using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombatFunctionality : CombatFunctionality
{
    private PlayerController playerControls;

    // Override the base property to return PlayerController instead
    public override CombatEntityController Controls
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
        playerControls.attack += UseAttackAbility;
        //print("f");
        playerControls.blockStart += BlockCaller;
        playerControls.blockStop += StopBlockingCaller;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerControls.attack -= UseAttackAbility;

        playerControls.blockStart -= BlockCaller;
        playerControls.blockStop -= StopBlockingCaller;
    }

    void BlockCaller()
    {
       // print("Player Combat : Block Caller called");
        playerControls.Block?.Invoke();
    }

    void StopBlockingCaller()
    {
        //print("Player Combat : Stop Blocking Caller called");
        playerControls.StopBlocking?.Invoke();
    }
}
