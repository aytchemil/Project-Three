using System.Collections;
using UnityEngine;
using static CombatEntityController;

public class ModeGeneralFunctionality : MonoBehaviour
{
    /// <summary>
    /// PARENT VIRTUAL STRING for all names of modes (Set in the child class)
    /// </summary>
    public virtual string MODE_NAME { get; }

    /// <summary>
    /// PARENT VIRTUAL FUNCTION for using all mode functionalities
    /// </summary>
   public virtual void UseModeFunctionality()
   {

   }


    /// Optional Functionality to finish an animation
    /// </summary>
    /// <param name="count"></param>
    /// <param name="animName"></param>
    /// <param name="animCont"></param>
    public virtual void FinishedAnAnimation(int count, string animName, CharacterAnimationController animCont)
    {
        print($"[{gameObject.name}] COMBAT ANIMATION [ModeGeneral]: Finished Animation {count} ");
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
    public virtual void AF_StartedAnAnimation()
    {

    }
}
