using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        

    }
}
