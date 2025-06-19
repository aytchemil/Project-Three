using System.Collections;
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
        cf.Controls.CombatWheelSelectDirection += ChangeBlockAbility;
        cf.Controls.blockStop += StopBlocking;
    }

    private void OnDisable()
    {
        cf.Controls.CombatWheelSelectDirection -= ChangeBlockAbility;
        cf.Controls.blockStop -= StopBlocking;
    }

    IEnumerator WaitToStartBlockingToUnblock(AbilityBlock ability)
    {
        readyToUnblock = false;
        yield return new WaitForSeconds(ability.defaultBlockTimeToBlocking);
        readyToUnblock = true;
    }

    public void ChangeBlockAbility(string dir)
    {
        CombatEntityModeData d = cf.Controls.Mode("Block");

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
        print("using block functionality");
        if (!cf.Controls.Mode("Block").isUsing)
            Block();
        else
            print("stop block");
    }

    void Block()
    {
        //print("Blocking");
        StartBlocking();

        AbilityBlock ability = (AbilityBlock)cf.Controls.Mode("Block").ability;
        cf.Controls.Mode("Block").SetAbility(ability);
        StartCoroutine(WaitToStartBlockingToUnblock(ability));


        //Trigger
        ModeTriggerGroup usingTrigger = cf.WheelTriggerEnableUse("Block");
        cf.Controls.Mode("Block").trigger = usingTrigger;
        //Ability
        AbilityWrapper usingAbility = new AbilityWrapper(ability);

        switch (ability.type)
        {
            case AbilityBlock.Type.Regular:

                //Setup

                //Animation
                StartCoroutine(WaitForUnblockAnimationSequence(ability));

                //Trigger
                usingTrigger.StartUsingAbilityTrigger(ability, ability.InitialUseDelay[0]);

                //Additional Functionality 
                AF_Regular(ability);

                break;
        }
    }


    void StartBlocking()
    {
        cf.Controls.Mode("Block").isUsing = true;
    }

    void StopBlocking()
    {
        print("stopping block");
        cf.Controls.Mode("Block").isUsing = false;
    }

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

    void AF_Regular(AbilityBlock ability)
    {
        print("[ModeBlockFunctionality] AF Regular");
    }


}
