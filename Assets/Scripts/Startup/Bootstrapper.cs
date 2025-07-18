using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AM;

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
    [SerializeReference] Type[] AnimTypes;
    public Weapon wpn;
    [SerializeField] List<AbilitySet> abilitySets;
    [SerializeField] GameObject menu;
    public bool startGame;
    [SerializeField] Button StartGameButton;
    public ModeManager mm;
    public GameManager gm;

    public void StartGame()
    {
        startGame = true;
    }

    void Awake()
    {
        mm = new ModeManager(modes);

        AnimTypes = new Type[]
        {
            typeof(MoveAnims),
            typeof(AtkAnims),
            typeof(BlkAnims)
        };

        SetupUI();
    }



    public void SetupUI()
    {
        StartGameButton.GetComponent<Button>().onClick.AddListener(StartTheGame);
    }
    public void SpawnPlayers()
    {
        Debug.Log("Spawning Players");

        GameObject newPlayer = EntityControllerFactory.SpawnEntityPremade(playerPrefab, modes, AnimTypes, wpn, abilitySets, playerSpawnLoc.position, Quaternion.identity);
        print($"Successfully Created New Player {newPlayer.name}");
        gm = new GameManager(menu, newPlayer);

    }

    public void SpawnEnemies()
    {
        Debug.Log("Spawning Enemies");

        GameObject newPlayer = EntityControllerFactory.SpawnEntityPremade(enemyPrefab, modes, AnimTypes, wpn, abilitySets, playerSpawnLoc.position, Quaternion.identity);
        print($"Successfully Created New Enemy {newPlayer.name}");
    }

    void StartTheGame()
    {
        SpawnPlayers();
        SpawnEnemies();

        gm.StartGame();
    }
}
