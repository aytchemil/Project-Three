using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static CombatEntityController;

public class ModeBlockFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE_NAME { get => "Block"; }
    bool readyToUnblock = false;

    void Awake()
    {
        cf = gameObject.GetComponent<CombatFunctionality>();
    }

    private void OnEnable()
    {
        cf.Controls.CombatWheelSelectDirection += ChangeBlock;
        cf.Controls.blockStop += StopBlocking;

        cf.Controls.EnterCombat += EnterCombatAutoBlock;
    }

    private void OnDisable()
    {
        cf.Controls.CombatWheelSelectDirection -= ChangeBlock;
        cf.Controls.blockStop -= StopBlocking;

        cf.Controls.EnterCombat -= EnterCombatAutoBlock;

    }

    IEnumerator WaitToStartBlockingToUnblock(AbilityBlock ability)
    {
        readyToUnblock = false;
        yield return new WaitForSeconds(ability.defaultBlockTimeToBlocking);
        readyToUnblock = true;
    }

    void ChangeBlock(string dir)
    {
        print($"[{gameObject.name}] Changing Block");
        cf.Controls.useAbility?.Invoke("Block");
        ChangeBlockAbility(dir);
    }

    /// <summary>
    /// Changes the current block ability in the ModeData
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeBlockAbility(string dir)
    {
        print($"[{gameObject.name}] Changing Block Ability");
        //Setup
        CombatEntityModeData d = cf.Controls.Mode("Block");


        //Mutations
        if (dir == "right")
            d.ability = d.data.abilitySet.right;
        else if(dir == "left")
            d.ability = d.data.abilitySet.left;
        else if (dir == "up")
            d.ability = d.data.abilitySet.up;
        else if (dir == "down")
            d.ability = d.data.abilitySet.down;
    }

    public override void UseModeFunctionality()
    {
        print($"[{gameObject.name}] Used Block");
        Block();
    }

    /// <summary>
    /// Attempt to block an incoming attack in the direction it comes in
    /// + Start to block
    /// + 
    /// </summary>
    void Block()
    {
        //Setup
        AbilityBlock ability = (AbilityBlock)cf.Controls.Mode("Block").ability;
        CombatEntityModeData block = cf.Controls.Mode("Block");
        ModeTriggerGroup usingTrigger = cf.WheelTriggerEnableUse("Block");
        AbilityWrapper usingAbility = new AbilityWrapper(ability);

        //Initial Mutations
        // + SETS flag value for start blocking
        // + SETS the ability in modedata
        // + SETS the trigger as the one we chose
        // + COROUTINE to wait for an unblock input
        StartBlocking();
        block.SetAbility(ability);
        block.trigger = usingTrigger;
        StartCoroutine(WaitToStartBlockingToUnblock(ability));

        //Mutations
        switch (ability.type)
        {
            case AbilityBlock.Type.Regular:

                //Setup

                //Animation
                StartCoroutine(WaitForUnblockAnimationSequence(ability));

                //Trigger
                usingTrigger.StartUsingAbilityTrigger(usingAbility, ability.InitialUseDelay[0]);

                //Additional Functionality 
                AF_Regular(ability);

                break;
        }
    }

    /// <summary>
    /// SETS the modes using flag to TRUE
    /// </summary>
    void StartBlocking()
    {
        cf.Controls.Mode("Block").isUsing = true;
    }

    /// <summary>
    /// SETS the modes using flag to FALSE
    /// </summary>
    void StopBlocking()
    {
        print("stopping block");
        cf.Controls.Mode("Block").isUsing = false;
    }


    void EnterCombatAutoBlock()
    {
        UseModeFunctionality();
    }

    /// <summary>
    /// COROUTINE for the ANIMATION sequence to wait for an unblock
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    IEnumerator WaitForUnblockAnimationSequence(AbilityBlock ability)
    {
        //print("block anim");
        AnimateAblity(ability.AnimName, ability.InitialUseDelay[0], cf.Controls.animController);
        while (cf.Controls.Mode("Block").isUsing || !readyToUnblock)
        {
            yield return new WaitForEndOfFrame();
        }
        AnimateAblity(ability.animationUnblock, ability.InitialUseDelay[0], cf.Controls.animController);
        readyToUnblock = false;
    }

    /// <summary>
    /// ADDITIONAL FUNCTIONALITY (Type: Regular)
    /// </summary>
    /// <param name="ability"></param>
    void AF_Regular(AbilityBlock ability)
    {
        print("[ModeBlockFunctionality] AF Regular");
    }




}
