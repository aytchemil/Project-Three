using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Just for now
    public GameObject player;
    ControllsHandler controlls;
    public GameObject menu;

    public DropdownMenu dd;

    private void Awake()
    {
        controlls = player.GetComponent<ControllsHandler>();
        player.gameObject.SetActive(false);
    }

    private void Start()
    {
        player.SetActive(false);
        menu.SetActive(true);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void StartGame()
    {
        player.SetActive(true);
        menu.SetActive(false);
    }

    void UpdateAbility(string dir)
    {
        switch (dir)
        {
            case "right":

                break;
            case "left":

                break;
            case "up":

                break;
            case "down":

                break;


        }
    }


}
