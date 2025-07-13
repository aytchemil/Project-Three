using System.Collections;
using UnityEngine;
using static EntityController;

public class ModeGeneralFunctionality : MonoBehaviour
{
    /// <summary>
    /// PARENT VIRTUAL STRING for all names of modes (Set in the child class)
    /// </summary>
    public virtual string MODE { get; }

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

    public virtual System.Enum[] GetAnimEnums(AbilityMulti ability)
    {
        System.Enum[] Enums = new System.Enum[ability.abilities.Length];
        for (int i = 0; i < ability.abilities.Length; i++)
        {
            AbilityAttack abilityi = ((AbilityAttack)ability.abilities[i]);
            System.Enum _enum = abilityi.Attack;
            Enums[i] = _enum;
        }
        return Enums;
    }
}
