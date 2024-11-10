using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject mainMenuPanel;
    public GameObject gunPanel;
    public GameObject graveyardPrefab;
    public GameObject playerPrefab;
    public SpawnManager spawnManagerScript;
    public GameObject healthPanel;
    public GameObject deathPanel;
    public GameObject upgradePanel;

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
        Playing,
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

    void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                mainMenuPanel.SetActive(false);
                break;
            case GameState.Playing:
                gunPanel.SetActive(false);
                break;
            case GameState.Death:
                deathPanel.SetActive(false);
                break;
            case GameState.Retreat:
                upgradePanel.SetActive(false);
                break;
        }
    }

    void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Playing:
                gunPanel.SetActive(true);
                healthPanel.SetActive(true);
                SpawnPlayer();
                SpawnEnemies();
                Time.timeScale = 1;
                break;
            case GameState.Death:
                deathPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Retreat:
                upgradePanel.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    public void PlayGame()
    {
        ChangeState(GameState.Playing);
    }

    void SpawnPlayer()
    {
        GameObject playerInstance = Instantiate(playerPrefab);
    }

    

    void SpawnEnemies()
    {
        spawnManagerScript.enabled = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
