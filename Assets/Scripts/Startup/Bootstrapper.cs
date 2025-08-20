using System;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] EnemyData[] enemies;
    [SerializeField] List<ModeData> modes = new List<ModeData>();
    [SerializeReference] Type[] AnimTypes;
    public Weapon wpn;
    [SerializeField] List<AbilitySet> abilitySets;
    [SerializeField] GameObject menu;
    public bool startGame;
    [SerializeField] Button StartGameButton;
    public ModeManager mm;
    public GameManager gm;

    [Serializable]
    public class EnemyData
    {
        public GameObject prefab;
        public Transform spawnLoc;
    }



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

        var sp0 = enemies[0].spawnLoc;
        var sp1 = enemies[1].spawnLoc;

        GameObject newEnemyB = EntityControllerFactory.SpawnEntityPremade(
            enemies[0].prefab, modes, AnimTypes, wpn, abilitySets, sp0.position, sp0.rotation);
        print($"Successfully Created New Enemy {newEnemyB.name}");

        GameObject newEnemyA = EntityControllerFactory.SpawnEntityPremade(
            enemies[1].prefab, modes, AnimTypes, wpn, abilitySets, sp1.position, sp1.rotation);
        print($"Successfully Created New Enemy {newEnemyA.name}");

        Debug.Log($"Spawn0 {sp0.name} world={sp0.position} local={sp0.localPosition}");
        Debug.Log($"Spawn1 {sp1.name} world={sp1.position} local={sp1.localPosition}");
    }

    void StartTheGame()
    {
        SpawnPlayers();
        SpawnEnemies();

        gm.StartGame();
    }
}
