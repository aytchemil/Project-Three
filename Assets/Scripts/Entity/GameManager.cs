using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager 
{
    //Just for now
    public GameObject player;
    public GameObject menu;
    public Bootstrapper bootstrapper;


    public GameManager(GameObject menu, Bootstrapper bs)
    {
        this.menu = menu;
        bootstrapper = bs;
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
        SpawnPlayers(1);
        SpawnEnemies(1);

        player.SetActive(true);
        menu.SetActive(false);

        player.GetComponent<EntityController>().animController.animator.Rebind();
    }


    IEnumerator DisablePlayer()
    {
        yield return new WaitForEndOfFrame();
        player.SetActive(false);
    }

    public void SpawnPlayers(int count)
    {
        bootstrapper.SpawnPlayers();
    }

    public void SpawnEnemies(int count)
    {
        bootstrapper.SpawnEnemies();
    }

}
