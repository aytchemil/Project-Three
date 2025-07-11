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
    public override bool trigger
    {
        get => blocking;
        set => blocking = value;
    }

    public virtual bool blockMissed { get; set; }

    public LayerMask blockCollisionWith;
    public Collider col;

    public virtual void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
    }

    protected override void EnableTriggerImplementation()
    {
        blockUp = false;
        blocking = false;
        blockMissed = false;


        //print($"[{combatFunctionality.gameObject.name}] Block Collider ENABLED");
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        blockUp = true;
        if (!gameObject.activeInHierarchy) return;

        if (myBlockAbility.hasBlockUpTime)
            StartCoroutine(CounterDownDelayed());
        else
            StartCoroutine(WaitForBlockToStop());

        //print($"[{combatFunctionality.gameObject.name}] Block Delay COMPLETE");

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

    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        myAbility = (AbilityBlock)abilty;
    }



    public virtual void OnTriggerEnter(Collider other)
    {
        GeneralAttackTriggerGroup AT = other.gameObject.GetComponent<GeneralAttackTriggerGroup>();

        //Null Check
        if (AT == null) return;
        if (other.GetComponent<AT_ColliderSingle>().combatFunctionality == combatFunctionality) return;

        // + Block up TRUE
        // + countering FLASE
        // + Attack Trigger attacking TRUE
        if (blockUp && !blocking && AT.attacking)
        {
            //print("BlockSys: Potential Block Detected");


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
                //print($"BlockSys: YES Match -> BLOCKED : ({myBlocksDirection}) and ({opponentAbilityDir})");
            }
            else
            {
                //print($"BlockSys: NO Match -> HIT : ({myBlocksDirection}) and ({opponentAbilityDir})");
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
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
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
