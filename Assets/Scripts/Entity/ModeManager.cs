using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public static ModeManager instance;
    public enum Modes
    {
        Attack = 0,
        Counter = 1,
        Blocking = 2,
        Combo = 3,
    }
    public List<ModeData> modes = new List<ModeData>();
    private void Awake()
    {
        //print("initializing mode manager");
        instance = this;
    }
}
