using System.Collections;
using UnityEngine;

public class OnExit : StateMachineBehaviour
{
    [SerializeField] private AnimatorSystem animatorSystem;
    [SerializeField] private bool lockLayer;
    [SerializeField] private float crossfade;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorSystem = animator.gameObject.GetComponent<AnimatorSystem>();
        CombatEntityController cec = animatorSystem.gameObject.GetComponent<CharacterAnimationController>().CEC;

        animatorSystem.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(stateInfo.length - crossfade);
            animatorSystem.SetLocked(false, layerIndex);
            object desiredAnimation = (cec.Mode("Block").ability as AbilityBlock).Block;

            Debug.Log("[AS] Reblocking");

        }
    }

}
