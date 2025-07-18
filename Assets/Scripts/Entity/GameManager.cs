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
        playerObject.GetComponent<MonoBehaviour>().StartCoroutine(DisablePlayer());
        menu.SetActive(true);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void StartGame()
    {
        player.SetActive(true);
        menu.SetActive(false);
    }


    IEnumerator DisablePlayer()
    {
        yield return new WaitForEndOfFrame();
        player.SetActive(false);
    }

}
