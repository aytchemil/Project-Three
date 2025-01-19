using UnityEngine;

public class ModeRuntimeData : MonoBehaviour
{
    public ModeData data;
    public Transform parent;
    public GameObject[] triggers = new GameObject[4];

    void AssignMyData(ModeData data)
    {
        this.data = data;
    }
}
