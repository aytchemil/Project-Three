using System;
using UnityEditor.ShaderGraph;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability Set", menuName = "ScriptableObjects/Ability Set")]
public class AbilitySet : ScriptableObject
{
    public ModeManager.Modes mode;
    public Ability right;
    public Ability left;
    public Ability up;
    public Ability down;

    public static int MAX_ABILITIES = 4;
    public Ability this[int index]
    {
        get
        {
            return index switch
            {
                0 => right,
                1 => left,
                2 => up,
                3 => down,
                _ => throw new IndexOutOfRangeException("Indexor for AbilitySet, past 3 (down)")
            };
        }

        set
        {
            if (index < 0 || index >= MAX_ABILITIES)
                throw new IndexOutOfRangeException("Indexor for AbilitySet, past 3 (down)");

            switch (index) 
            {
                case 0: 
                    right = value; break;
                case 1:
                    left = value; break;
                case 2:
                    up = value; break;
                case 3:
                    down = value; break;
            };
        }
    }

    public static string GetLookDir(int abilitySetIndex)
    {
        if (abilitySetIndex == 0)
            return "right";
        else if (abilitySetIndex == 1)
            return "left";
        else if (abilitySetIndex == 2)
            return "up";
        else if (abilitySetIndex == 3)
            return "down";

        throw new Exception($"[AbilitySet] +GetLookDir: abilitySetIndex not found [{abilitySetIndex}] should be between 0 and 3");
    }
}
