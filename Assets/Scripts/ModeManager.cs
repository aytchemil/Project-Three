using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public static ModeManager instance;

    public List<ModeData> modes = new List<ModeData>();
    private void Awake()
    {
        print("initializing mode manager");
        instance = this;
    }
}
