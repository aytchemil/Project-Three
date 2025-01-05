using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttackTrigger : AttackTriggerGroup
{
    public List<AttackTriggerGroup> triggers;
    bool hasTriggers = false;

    private void OnEnable()
    {
        if(!hasTriggers)
            TakeOnChildrenAttackTriggers();
    }

    void TakeOnChildrenAttackTriggers()
    {
        print("taking on children");


        for(int i = 0; i < transform.childCount; i++)
        {
            triggers.Add(transform.GetChild(i).GetComponent<AttackTriggerGroup>());
            print("Adding to triggers: " + transform.GetChild(i).name);
        }

        hasTriggers = true;
    }


}
