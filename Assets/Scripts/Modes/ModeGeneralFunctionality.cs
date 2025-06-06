using System.Collections;
using UnityEngine;
using static CombatEntityController;

public class ModeGeneralFunctionality : MonoBehaviour
{
   public virtual string MODE_NAME { get; }

   public virtual void UseModeFunctionality()
   {

   }

    public void AnimateAblity(string animationName, float delay, CharacterAnimationController animCont)
    {
        Debug.Log($"[{gameObject.name}] [{this.GetType()}] is using Ability [{animationName}] )");
        animCont.UseAnimation?.Invoke(animationName, delay);
    }

    public IEnumerator AnimateFollowUpAbilities(AbilityWrapper usingAbility, ModeTriggerGroup usingTrigger, CombatEntityModeData mode, CharacterAnimationController animCont)
    {
        print("[Followup] Animation starting");
        for (int i = 0; i < usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress.Count && mode.isUsing; i++)
        {
            string animName = (usingAbility.Values[i]).AnimName.ToString();

            print($"[CounterFunctionality] combo animation on attack [{i}] which is ability [{animName}]");
            AnimateAblity(animName, usingAbility.parentAbility.InitialUseDelay[i], animCont);

            //AdditionalFunctionality Option
            StartedAnAnimation();

            while (mode.isUsing && usingAbility.completedAnimation[i] == false)
            {
                Debug.Log($"Updating animation, current progress for [{i}] is [{usingAbility.completedAnimation[i]}]");
                print(usingTrigger.gameObject.name);
                yield return new WaitForEndOfFrame();
                usingAbility.completedAnimation = usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress;
            }

            //AdditionalFunctionality Option
            FinishedAnAnimation(i, animName, animCont);
        }

        StartCoroutine(CompletedAnimationSequence(animCont));
        print("[Followup] Full Followup Completed");

    }

    public virtual void FinishedAnAnimation(int count, string animName, CharacterAnimationController animCont)
    {
        print($"[Followup] Finished animation on index: [{count}] ");
    }

    public virtual IEnumerator CompletedAnimationSequence(CharacterAnimationController animCont)
    {
        yield return new WaitForEndOfFrame();
    }

    public virtual void StartedAnAnimation()
    {

    }
}
