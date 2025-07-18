using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager 
{
    //Just for now
    public GameObject player;
    public GameObject menu;
    PlayerController PCE;


    public GameManager(GameObject menu, GameObject player)
    {
        this.player = player;
        this.menu = menu;
        PCE = player.GetComponent<PlayerController>();
        Init(player);
    }

    public void Init(GameObject playerObject)
    {
        Debug.Log($"init player {playerObject}");
        player = playerObject;
        menu.SetActive(true);
        player.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        Debug.Log($"Registered Game Manager. player is {player}");

    }

    public void StartGame()
    {
        Debug.Log($"Setting player {player} active");
        player.SetActive(true);
        menu.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }


}
