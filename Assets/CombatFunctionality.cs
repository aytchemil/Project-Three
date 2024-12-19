using UnityEngine;

[RequireComponent(typeof(ControllsHandler))]
public class CombatFunctionality : MonoBehaviour
{
    ControllsHandler c;

    private void Awake()
    {
        c = GetComponent<ControllsHandler>();
        c.UtilizeAbility += EnableAbility;
    }

    void EnableAbility(string dir)
    {
        switch (dir)
        {
            case "right":
                 c.a_current= c.a_right;
                    break;
            case "left":
                c.a_current = c.a_left;
                break;
            case "up":
                c.a_current = c.a_up;
                break;
            case "down":
                c.a_current = c.a_right;
                break;
        }
    }




}
