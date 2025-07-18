using Sirenix.OdinInspector;
using UnityEngine;
using System;
using static EntityController;
using System.Collections;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class AbilityBlock : Ability, IAbilityAnims, IAbilityDirectional
{
    public Type type { get => typeof(AM.BlkAnims); }
    public int Enum { get => (int)Block; }
    public IAbilityDirectional.Direction Dir { get => dir; }

    [SerializeField] private IAbilityDirectional.Direction dir;

    public AM.BlkAnims.Anims Block;

    public enum Collision
    {
        Regular,
    }
    public Collision collision;


    public GameObject blockTriggerCollider;
    public override GameObject ColliderPrefab
    {
        get => blockTriggerCollider;
        set => blockTriggerCollider = value;
    }

    public bool hasBlockUpTime = false;
    [ShowIf("hasBlockUpTime")]
    public float blockUpTime;
    public float damagePercentageBlocked;
    public float slowdownMovementPercentage;
    public string animationUnblock;
    public float defaultBlockTimeToBlocking = 0.5f;

    public override void Use(ICombatMode combatMode, CombatFunctionality cf, RuntimeModeData Mode)
    {
        BlockMode Block = (BlockMode)combatMode;
        AbilityBlock ability = (AbilityBlock)Mode.ability;
        CharacterAnimationController animCont = cf.Controls.animController;
        ModeTriggerGroup trigger = Mode.trigger;

        //Initial Mutations
        // + SETS flag value for start blocking
        // + SETS the ability in modedata
        // + SETS the trigger as the one we chose
        // + COROUTINE to wait for an unblock input

        cf.StartCoroutine(WaitToStartBlockingToUnblock(ability));


        //Execution
        animCont.Play(typeof(AM.BlkAnims), (int)ability.Block, CharacterAnimationController.UPPERBODY, false, false);

        //Trigger
        trigger.Use(ability.InitialUseDelay[0]);


        IEnumerator WaitToStartBlockingToUnblock(AbilityBlock ability)
        {
            Block.readyToUnblock = false;
            yield return new WaitForSeconds(ability.defaultBlockTimeToBlocking);
            Block.readyToUnblock = true;
        }
    }

}
