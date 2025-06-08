using System.Collections;
using UnityEngine;
using static CombatEntityController;

public class ModeGeneralFunctionality : MonoBehaviour
{
    /// <summary>
    /// Virtual for all names of modes (Set in the child class)
    /// </summary>
   public virtual string MODE_NAME { get; }

    /// <summary>
    /// The virtual for using all mode functionalities
    /// </summary>
   public virtual void UseModeFunctionality()
   {

   }

    /// <summary>
    /// Individually animates an ability's move
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="delay"></param>
    /// <param name="animCont"></param>
    public void AnimateAblity(string animationName, float delay, CharacterAnimationController animCont)
    {
        Debug.Log($"[{gameObject.name}] [{this.GetType()}] is using Ability [{animationName}] )");
        animCont.UseAnimation?.Invoke(animationName, delay);
    }

    /// <summary>
    /// Animates a parent ability's ability array over time
    /// </summary>
    /// <param name="usingAbility"></param>
    /// <param name="usingTrigger"></param>
    /// <param name="mode"></param>
    /// <param name="animCont"></param>
    /// <returns></returns>
    public IEnumerator AnimateFollowUpAbilities(AbilityWrapper usingAbility, ModeTriggerGroup usingTrigger, CombatEntityModeData mode, CharacterAnimationController animCont)
    {
        print("[Followup] Animation starting");

        //Loop through all triggers and if we are using the mode
        for (int i = 0; i < usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress.Count && mode.isUsing; i++)
        {
            //Setup
            string animName = (usingAbility.Values[i]).AnimName.ToString();
            //print($"[Followup] combo animation on attack [{i}] which is ability [{animName}]");

            //Animates
            AnimateAblity(animName, usingAbility.parentAbility.InitialUseDelay[i], animCont);

            //AdditionalFunctionality Option
            StartedAnAnimation();

            //Checking if the trigger progressed
            while (mode.isUsing && usingAbility.completedAnimation[i] == false)
            {
                //Debug.Log($"[Followup] [Animation] Updating animation, current progress for [{i}] is [{usingAbility.completedAnimation[i]}]");
                print(usingTrigger.gameObject.name);
                yield return new WaitForEndOfFrame();
                usingAbility.completedAnimation = usingTrigger.GetComponent<MAT_FollowupGroup>().triggerProgress;
            }

            //AdditionalFunctionality Option
            FinishedAnAnimation(i, animName, animCont);
        }

        //Waiting an extra frame for the animation to complete because FinishedAnimation will also run on this frame, and they may cancle eachother out
        StartCoroutine(CompletedAnimationSequence(animCont));
        print("[Followup] [Animation] Full Followup Completed");

    }

    /// <summary>
    /// Optional Functionality to finish an animation
    /// </summary>
    /// <param name="count"></param>
    /// <param name="animName"></param>
    /// <param name="animCont"></param>
    public virtual void FinishedAnAnimation(int count, string animName, CharacterAnimationController animCont)
    {
        print($"[Followup] Finished animation on index: [{count}] ");
    }

    /// <summary>
    /// Optional Functionality for completed an animation sequence of a parent ability
    /// </summary>
    /// <param name="animCont"></param>
    /// <returns></returns>
    public virtual IEnumerator CompletedAnimationSequence(CharacterAnimationController animCont)
    {
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Optional Functionality for starting an animation
    /// </summary>
    public virtual void StartedAnAnimation()
    {

    }
}
