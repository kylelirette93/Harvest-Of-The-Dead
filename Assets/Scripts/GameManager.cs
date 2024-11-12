using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject mainMenuPanel;
    public GameObject gunPanel;
    public GameObject graveyardPrefab;
    public GameObject playerPrefab;
    public SpawnManager spawnManagerScript;
    public GameObject deathPanel;
    public GameObject retreatPanel;
    public GameObject statsPanel;
    public DisplayCurrency displayCurrencyScript;

    GameObject playerInstance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum GameState
    {
        MainMenu,
        Init,
        Playing,
        NewDay,
        Death,
        Retreat,
    }

    public GameState currentState;

    void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        ExitState(currentState);

        currentState = newState;

        EnterState(currentState);
    }

    public void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                mainMenuPanel.SetActive(false);
                break;
            case GameState.Init:
                break;
            case GameState.Playing:
                gunPanel.SetActive(false);
                statsPanel.SetActive(false);
                break;
            case GameState.NewDay:
                gunPanel.SetActive(false);
                SpawnPlayer();
                SpawnEnemies();
                break;
            case GameState.Death:
                deathPanel.SetActive(false);
                break;
            case GameState.Retreat:
                retreatPanel.SetActive(false);
                break;
        }
    }

    public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Init:
                gunPanel.SetActive(true);
                statsPanel.SetActive(true);
                SpawnPlayer();
                ActivatePlayer();
                Time.timeScale = 1;
                ChangeState(GameState.Playing);
                break;
            case GameState.Playing:          
                SpawnEnemies();
                Time.timeScale = 1;
                break;
            case GameState.NewDay:
                gunPanel.SetActive(true);
                statsPanel.SetActive(true);
                ActivatePlayer();
                SpawnEnemies();
                CurrencySystem.ResetCurrency();
                CurrencySystem.ResetEarnedCurrency();
                UpdateCurrencyText();
                Time.timeScale = 1;
                break;
            case GameState.Death:
                DeactivateHealthBar();
                DeactivatePlayer();
                DespawnEnemies();
                DeactivateZombies("Zombie");
                deathPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Retreat:
                DeactivateHealthBar();
                DeactivatePlayer();
                DespawnEnemies();
                DeactivateZombies("Zombie");
                CurrencySystem.BankCurrency();
                retreatPanel.SetActive(true);
                displayCurrencyScript.UpdateUI();
                Time.timeScale = 0;
                break;
        }
    }

    void DeactivateHealthBar()
    {
        playerInstance.GetComponent<Player>().healthBarFill.enabled = false;
    }

    void DeactivateZombies(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Zombie zombie = obj.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.DeactivateZombie();
            }
        }
    }

    public void PlayGame()
    {
        ChangeState(GameState.Init);
    }

    public void ActivatePlayer()
    {
        playerInstance.SetActive(true);
    }

    public void DeactivatePlayer()
    {
        playerInstance.SetActive(false);
    }

    public void StartNewDay()
    {
        ChangeState(GameState.NewDay);
    }

    void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab);
    }

    void UpdateCurrencyText()
    {
        Player player = playerInstance.GetComponent<Player>();
        player.UpdateUI();
    }
    

    void SpawnEnemies()
    {
        spawnManagerScript.enabled = true;
        spawnManagerScript.globalChaseSpeed = 1.5f;
        spawnManagerScript.spawnDelay = 5f;
    }

    void DespawnEnemies()
    {
        spawnManagerScript.enabled = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
