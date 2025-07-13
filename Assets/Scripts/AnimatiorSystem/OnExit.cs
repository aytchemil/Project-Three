using System.Collections;
using UnityEngine;

public class OnExit : StateMachineBehaviour
{
    [SerializeField] private AnimatorSystem animatorSystem;
    [SerializeField] private bool lockLayer;
    [SerializeField] private float crossfade;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorSystem = animator.gameObject.GetComponent<AnimatorSystem>() ?? throw new System.ArgumentNullException("Animator system not set");
        EntityController cec = animatorSystem.GetComponent<CharacterAnimationController>().CEC ?? throw new System.ArgumentNullException("CEC not set");

        animatorSystem.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(stateInfo.length - crossfade);

            animatorSystem.SetLocked(false, layerIndex);

            if (cec.Mode("Attack").isUsing)
            {
                animatorSystem.StartCoroutine(WaitUntilAttackClears());
                yield break;
            }

            NextAnim();
        }

        IEnumerator WaitUntilAttackClears()
        {
            while (cec.Mode("Attack").isUsing)
                yield return new WaitForEndOfFrame();

             NextAnim();
        }


        void NextAnim()
        {
            AM.BlkAnims.Anims desiredAnim = (cec.Mode("Block").ability as AbilityBlock).Block;
            animatorSystem.Play(typeof(AM.BlkAnims), (int)desiredAnim, layerIndex, false, false);
        }
        //Debug.Log("[AS] ANIMATION EXITING");
    }

}
