using System.Collections;
using UnityEngine;

public class BlockTriggerCollider : ModeTriggerGroup
{
    public bool blockUp;

    public virtual AbilityBlock myBlockAbility { get; set; }
    public override Ability ability
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
        if (myBlockAbility.hasBlockUpTime)
            blockUp = false;
        else
            blockUp = true;
        blocking = false;
        blockMissed = false;


        //print($"[{cf.gameObject.name}] Block Collider ENABLED");
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        blockUp = true;
        if (!gameObject.activeInHierarchy) return;

        if (myBlockAbility.hasBlockUpTime)
            StartCoroutine(CounterDownDelayed());
        else
            StartCoroutine(WaitForBlockToStop());

        //print($"[{cf.gameObject.name}] Block Delay COMPLETE");

    }

    protected override void DisableThisTriggerImplementation()
    {
        cf.Controls.BlockedAttack?.Invoke(cf.Controls.lookDir);

        print("Disabling this Block Trigger, Not locally");
        StopAllCoroutines();
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        print("BlockSys: Disabled Block");
        blockUp = false;
        blocking = false;
        blockMissed = false;

    }

    protected override void InitializeSelfImplementation(CombatFunctionality cf, Ability abilty)
    {
        ability = (AbilityBlock)abilty;
    }



    private void OnTriggerEnter(Collider other)
    {
        // Find the attack trigger component on this collider, its parent, or children
        var AT = other.GetComponent<AT_ColliderSingle>();

        if (AT.cf == cf) return;

        if (AT == null)
        {
            Debug.Log($"BlockSys: No AT Detected on {other.gameObject.name}");
            return;
        }

        if (!blockUp && AT.attacking)
        {
            Debug.Log("BlockSys: Can't block");
            return;
        }

        Debug.Log("BlockSys: Potential Block Detected");
        Debug.Log($"BlockSys : New AT Is {AT.name}");
        Debug.Log($"BlockSys : Checking If its a followup, if it is, Basic Block Does nothing");
        if (AT.isLocal)
            if (AT.parentTrigger is MAT_FollowupGroup followup)
            {
                Debug.Log($"BlockSys: HIT (NO BLOCK) Followup Attack Detected on block, Intentionally Do nothing");
                return;
            }


        // Closest contact point from the incoming trigger to me
        Vector3 effectpos = other.ClosestPoint(transform.position);

        // Safely get the attack direction
        if (AT.ability is not AbilityAttack attack)
        {
            Debug.LogWarning("BlockSys: AT has no AbilityAttack");
            return;
        }

        string myBlocksDirection = cf.Controls.lookDir;
        string opponentAbilityDir = attack.Dir.ToString();

        Debug.Log($"BlockSys: CHECKING ({myBlocksDirection}) and ({opponentAbilityDir})");

        if (DidAttackGetBlocked(myBlocksDirection, opponentAbilityDir))
        {
            AT.AttackTriggerBlocked(myBlocksDirection, effectpos);
            AbilityExecutor.ExecuteAbility(myBlockAbility, cf.gameObject, effectpos);

            Debug.Log($"BlockSys: BLOCKED [blocker {cf.gameObject.name}: {myBlocksDirection}] and [ opp {AT.cf.gameObject.name}: {opponentAbilityDir})");
        }
        else
        {
            Debug.Log($"BlockSys: HIT (NO BLOCK) [blocker {cf.gameObject.name}: {myBlocksDirection}] and [ opp {AT.cf.gameObject.name}: {opponentAbilityDir})");

        }

        //Debug.Log("BlockSys: end");
    }


    public static bool DidAttackGetBlocked(string my, string opp)
    {
        bool sideDirsTheSame = IsSideDirsTheSame(my, opp);
        bool upAndDownTheSame = IsUpAndDownDirTheSame(my, opp);

        //Debug.Log($"BlockSys: CHECKING ({myBlocksDirection}) and ({opponentAbilityDir})");

        if (sideDirsTheSame || upAndDownTheSame)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Checks if the dirs are both either left or right, if they are check if they are the same. if they are return true
    /// </summary>
    /// <param name="my"></param>
    /// <param name="opp"></param>
    /// <returns></returns>
    public static bool IsSideDirsTheSame(string my, string opp)
    {
        bool ret = false;

        if (my == "left" && opp == "right")
            ret = true;
        else if (my == "right" && opp == "left")
            ret = true;

        return ret;
    }

    public static bool IsUpAndDownDirTheSame(string my, string opp)
    {
        bool ret = false;
        if (my == "up" && opp == "up")
            ret = true;
        if (my == "down" && opp == "down")
            ret = true;

        return ret;
    }

    void BlockAttack()
    {
        cf.Controls.Mode("Counter");
    }

    void CounterAttack()
    {
        blocking = true;

        if (DebugManager.instance.AttackCollisionDebugsOn)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    IEnumerator CounterDownDelayed()
    {
        yield return new WaitForSeconds(myBlockAbility.blockUpTime);
        CounterDown();
    }
    IEnumerator WaitForBlockToStop()
    {
        while (cf.Controls.Mode("Block").isUsing)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
        CounterDown();
    }

    void CounterDown()
    {
        print("BlockSys: counter collider is down");
        unused = true;
        DisableThisTriggerOnlyLocally();
    }

    protected override void DisableThisTriggerOnDelayImplementation()
    {
    }
}
