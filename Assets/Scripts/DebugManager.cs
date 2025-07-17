using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    private void Awake()
    {
        instance = this;
    }

    public bool AttackCollisionDebugsOn = true;
    public bool CounterCollisionDebugsOn = true;

}
