using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Just for now
    public GameObject player;
    PlayerController controlls;
    public GameObject menu;

    public DropdownMenu dd;

    private void Awake()
    {
        controlls = player.GetComponent<PlayerController>();
    }

    private void Start()
    {
        StartCoroutine(DisablePlayer());
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


    IEnumerator DisablePlayer()
    {
        yield return new WaitForEndOfFrame();
        player.SetActive(false);
    }

}
