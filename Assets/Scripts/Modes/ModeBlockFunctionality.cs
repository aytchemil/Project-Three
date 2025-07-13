using System.Collections;
using UnityEngine;
using static EntityController;

public class ModeBlockFunctionality : ModeGeneralFunctionality
{
    private CombatFunctionality cf;

    public override string MODE { get => "Block"; }
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
        //print($"[{gameObject.name}] BLOCK Direction Changing to {dir}");
        ChangeBlockAbility(dir);
        cf.Controls.useAbility?.Invoke("Block");

    }

    /// <summary>
    /// Changes the current block ability in the ModeData
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeBlockAbility(string dir)
    {
        //print($"[{gameObject.name}] Blocking {dir}");
        //Setup
        RuntimeModeData d = cf.Controls.Mode("Block");


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
        //print($"[{gameObject.name}] BLOCK used");
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
        RuntimeModeData block = cf.Controls.Mode("Block");
        // + SETS all triggers to false
        for (int i = 0; i < block.triggers.Length; i++)
            block.triggers[i].gameObject.SetActive(false);
        AbilityBlock ability = (AbilityBlock)cf.Controls.Mode("Block").ability;
        ModeTriggerGroup trigger = cf.WheelTriggerEnableUse("Block");
        CharacterAnimationController animCont = cf.Controls.animController;

        //Initial Mutations
        // + SETS flag value for start blocking
        // + SETS the ability in modedata
        // + SETS the trigger as the one we chose
        // + COROUTINE to wait for an unblock input
        StartBlocking();
        block.SetAbility(ability);
        block.trigger = trigger;
        StartCoroutine(WaitToStartBlockingToUnblock(ability));


        //Mutations
        switch (ability.collision)
        {
            case AbilityBlock.Collision.Regular:

                //Setup

                //Animation
                animCont.Play(typeof(AM.BlkAnims), (int)ability.Block, CharacterAnimationController.UPPERBODY, false, false);


                //Trigger
                trigger.Use(ability.InitialUseDelay[0]);

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
    /// ADDITIONAL FUNCTIONALITY (Type: Regular)
    /// </summary>
    /// <param name="ability"></param>
    void AF_Regular(AbilityBlock ability)
    {
        //print("[ModeBlockFunctionality] AF Regular");
    }




}
