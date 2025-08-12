using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using static EntityController;

[CreateAssetMenu(fileName = "ComboAbility", menuName = "ScriptableObjects/Abilities/Multi/Combo Ability")]
public class AbilityCombo : AbilityMulti
{
    public float reattackTimeUntilReset;

    [HideInInspector]
    public override float maxInitialUseDelay => reattackTimeUntilReset;

    public enum ComboType
    {
        Linear = 0,
    }
    public ComboType comboType;

    public override void Use(ICombatMode mode, CombatFunctionality cf, EntityController.RuntimeModeData Mode)
    {
        AbilityCombo ability = (AbilityCombo)Mode.ability;
        ModeTriggerGroup trigger = Mode.trigger;
        EntityController.RuntimeModeData AtkMode = cf.Controls.Mode("Attack");


        AtkMode.functionality.Starting();

        //Desired Mutations
        switch (ability.comboType)
        {
            case AbilityCombo.ComboType.Linear:

                //Setup
                cf.StartCoroutine(WaitForComboToFinish(trigger));

                //Actuall Attack
                UseCurrentCombo().Use(ability.initialUseDelay[0]);

                //Animation
                AM.FollowUpPackage FollowUpPackage = new AM.FollowUpPackage(
                    trigger,
                    Mode,
                    cf.GetAnimEnums(ability),
                    typeof(AM.AtkAnims),
                    typeof(AM.AtkAnims.Anims),
                    CharacterAnimationController.UPPERBODY,
                    false,
                    false,
                    0.2f,
                    ability.initialUseDelay
                    );
                cf.StartCoroutine(FollowUpPackage.PlayFollowUp(cf.Controls.animController.Play));
                break;

        }





        /// <summary>
        /// +Sets all trigers to false
        /// +Sets a trigger active
        /// +returns that trigger
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        CombotTriggerGroup UseCurrentCombo()
        {
            string dir = cf.Controls.lookDir;
            //print($"[MODE] [COMBO] : Direction Chosen ({dir})");

            RuntimeModeData combo = Mode;
            int trigerIndx = cf.GetDirIndex(dir);

            //Errors
            if (combo == null || combo.triggers == null) throw new NullReferenceException("Mode 'Combo' or its triggers are null.");
            if (dir == null) throw new Exception($"direction: [ {dir} ] is null");

            foreach (GameObject trigger in combo.triggers)
                trigger.SetActive(false);
            combo.triggers[trigerIndx].SetActive(true);
            return combo.triggers[trigerIndx].GetComponent<CombotTriggerGroup>();

        }



        /// <summary>
        /// Waits for the combo to finish (either reach final trigger proggress or attack.isusing = false)
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        IEnumerator WaitForComboToFinish(ModeTriggerGroup trigger)
        {
            CombotTriggerGroup comboTrigger = trigger.GetComponent<CombotTriggerGroup>();

            while (Mode.isUsing == true)
            {
                //print($"[MODE] [COMBO] waiting for combo to finish");
                yield return new WaitForEndOfFrame();

                if (comboTrigger.triggerProgress[comboTrigger.triggerProgress.Length - 1] == true || AtkMode.isUsing == false)
                    Mode.isUsing = false;
            }
        }

    }



}
