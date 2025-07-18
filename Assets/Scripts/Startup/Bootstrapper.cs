using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] bool spawnPlayers;
    [SerializeField] bool spawnEnemies;
    [SerializeField] int playerCount;
    [SerializeField] int enemyCount;
    [SerializeField] Transform playerSpawnLoc;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemySpawnLoc;
    [SerializeField] List<ModeData> modes = new List<ModeData>();
    public Weapon wpn;
    [SerializeField] List<AbilitySet> abilitySets;
    [SerializeField] GameObject menu;
    [SerializeField] Button StartGameButton;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        ServiceLocator.Register(new ModeManager(modes));

        ServiceLocator.Register(new GameManager(menu, this));

        print("Boostrap Complete");

        SetupUI();
    }

    public void SetupUI()
    {
        StartGameButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ServiceLocator.Get<GameManager>().StartGame();
        });
    }
    public void SpawnPlayers()
    {
        Debug.Log("Spawning Players");

        for (int i = 0; i < playerCount; i++)
        {
            GameObject newPlayer = EntityControllerFactory.SpawnEntityPremade(playerPrefab, modes, wpn, abilitySets, playerSpawnLoc.position, Quaternion.identity);
            print($"Successfully Created New Player {newPlayer.name}");
            ServiceLocator.Get<GameManager>().Init(newPlayer);
            newPlayer.SetActive(false);
        }
    }

    public void SpawnEnemies()
    {
        Debug.Log("Spawning Enemies");

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject newPlayer = EntityControllerFactory.SpawnEntityPremade(enemyPrefab, modes, wpn, abilitySets, playerSpawnLoc.position, Quaternion.identity);
            print($"Successfully Created New Enemy {newPlayer.name}");
        }
    }

    public void SetAbilitySet(ModeManager.Modes mode)
    {

    }
}
