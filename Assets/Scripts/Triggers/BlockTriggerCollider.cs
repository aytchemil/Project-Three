using System.Collections;
using UnityEngine;

public class BlockTriggerCollider : ModeTriggerGroup
{
    public bool blockUp;

    public virtual AbilityBlock myBlockAbility { get; set; }
    public override Ability myAbility
    {
        get => myBlockAbility;
        set => myBlockAbility = value as AbilityBlock;

    }

    public virtual bool blocking { get; set; }
    public override bool usingTrigger
    {
        get => blocking;
        set => blocking = value;
    }

    public virtual bool blockMissed { get; set; }

    public LayerMask blockCollisionWith;
    public Collider col;
    public Animator animator;

    public virtual void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    protected override void EnableTriggerImplementation()
    {
        blockUp = false;
        blocking = false;
        blockMissed = false;
        animator.SetBool("counter", false);
        animator.SetBool("windupDone", false);


        print("enabled block trigger");
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        blockUp = true;
        animator.SetBool("windupDone", true);

        Color seeThroughBlue = new Color(0, 0, 1, 0.2f);

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            gameObject.GetComponent<MeshRenderer>().material.color = seeThroughBlue;
        else
            gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (myBlockAbility.hasBlockUpTime)
            StartCoroutine(CounterDownDelayed());
        else
            StartCoroutine(WaitForBlockToStop());

        print("intial delay done. block trigger is up...");

    }

    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.Controls.BlockedAttack?.Invoke(combatFunctionality.Controls.lookDir);

        print("Disabling this Block Trigger, Not locally");
        StopAllCoroutines();
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        blockUp = false;
        blocking = false;
        blockMissed = false;

        animator.SetBool("counter", false);
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        myAbility = (AbilityBlock)abilty;
    }



    public virtual void OnTriggerEnter(Collider other)
    {
        GeneralAttackTriggerGroup AT = other.gameObject.GetComponent<GeneralAttackTriggerGroup>();

        //Null Check
        if (AT == null)
            return;

        // + Block up TRUE
        // + countering FLASE
        // + Attack Trigger attacking TRUE
        if (blockUp && !blocking && AT.attacking)
        {
            print("BlockSys: Potential Block Detected");


            //Setup
            Vector3 effectpos = other.ClosestPoint(transform.position);
            string myBlocksDirection = myBlockAbility.dir.ToString();
            string opponentAbilityDir = AT.myAbility.dir.ToString();
            bool sidePerspectivesHappened = IsLeftRightPerspectiveCheckHappened(myBlocksDirection, opponentAbilityDir);
            bool sideDirsTheSame = IsSideDirsTheSame(myBlocksDirection, opponentAbilityDir);

            //Mutations
            if ((myBlocksDirection == opponentAbilityDir || sidePerspectivesHappened) && !sideDirsTheSame)
            {
                AT.AttackTriggerBlocked(myBlocksDirection, effectpos);
                print($"BlockSys: YES Match -> BLOCKED : ({myBlocksDirection}) and ({opponentAbilityDir})");
            }
            else
            {
                print($"BlockSys: NO Match -> HIT : ({myBlocksDirection}) and ({opponentAbilityDir})");
            }

            //TODO: If in defensive mode counter
            //CounterAttack();
        }
    }

    /// <summary>
    /// Checks the perspective of two (string) directions that if left and right is given they are same dir
    /// </summary>
    /// <param name="my"></param>
    /// <param name="opp"></param>
    /// <returns></returns>
    bool IsLeftRightPerspectiveCheckHappened(string my, string opp)
    {
        bool ret = false;
        if (my == "right" && opp == "left")
            ret = true;
        else if (my == "left" && opp == "right")
            ret = true;
        return ret;
    }

    /// <summary>
    /// Checks if the dirs are both either left or right, if they are check if they are the same. if they are return true
    /// </summary>
    /// <param name="my"></param>
    /// <param name="opp"></param>
    /// <returns></returns>
    bool IsSideDirsTheSame(string my, string opp)
    {
        bool ret = false;

        if(my == "left" ||  my == "right" && opp == "left" || opp == "right")
            if (my == opp)
                ret = true;

        return ret;
    }

    void BlockAttack()
    {
        combatFunctionality.Controls.Mode("Counter");
    }

    void CounterAttack()
    {
        blocking = true;

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        animator.SetBool("counter", true);
    }

    IEnumerator CounterDownDelayed()
    {
        yield return new WaitForSeconds(myBlockAbility.blockUpTime);
        CounterDown();
    }
    IEnumerator WaitForBlockToStop()
    {
        while(combatFunctionality.Controls.Mode("Block").isUsing)
        {
            yield return new WaitForEndOfFrame();
        }
        CounterDown();
    }

    void CounterDown()
    {
        print("counter collider is down");
        unused = true;
        DisableThisTriggerOnlyLocally();
    }

    protected override void DisableThisTriggerOnDelayImplementation()
    {
        throw new System.NotImplementedException();
    }
}
