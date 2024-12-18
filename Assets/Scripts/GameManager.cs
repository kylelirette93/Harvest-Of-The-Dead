using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject studioNamePanel;
    public VideoPlayer studioNameVideoPlayer;
    public GameObject mainMenuPanel;
    public GameObject gunPanel;
    public GameObject graveyardPrefab;
    public GameObject playerPrefab;
    public SpawnManager spawnManagerScript;
    public GameObject deathPanel;
    public GameObject retreatPanel;
    public GameObject upgradePanel;
    public GameObject statsPanel;
    public GameObject pausePanel;
    public GameObject blackScreen;
    public DisplayCurrency displayCurrencyScript;
    Vector3 originalPosition;
    public int CurrentDay { get; private set; } = 1;

    GameObject playerInstance;

    public AudioClip menuMusic;
    public AudioClip gamePlayMusic;
    public AudioClip upgradeMusic;
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
    }

    public enum GameState
    {
        StudioName,
        MainMenu,
        Init,
        Playing,
        NewDay,
        Death,
        Retreat,
        Upgrade,
    }

    public GameState currentState;

    void Start()
    {
        CurrentDay = 1;
        ChangeState(GameState.StudioName);
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
            case GameState.StudioName:
                studioNamePanel.SetActive(false);
                break;
            case GameState.MainMenu:
                mainMenuPanel.SetActive(false);
                StopMusic();
                break;
            case GameState.Init:
                break;
            case GameState.Playing:
                gunPanel.SetActive(false);
                statsPanel.SetActive(false);
                StopMusic();
                break;
            case GameState.NewDay:
                break;
            case GameState.Death:
                deathPanel.SetActive(false);
                break;
            case GameState.Retreat:
                retreatPanel.SetActive(false);
                break;
            case GameState.Upgrade:
                upgradePanel.SetActive(false);
                StopMusic();
                break;
        }
    }

    public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.StudioName:
                studioNamePanel.SetActive(true);
                studioNameVideoPlayer.Play();
                Invoke("TransitionToMainMenu", (float)studioNameVideoPlayer.clip.length);
                Invoke("DisableBlackScreen", 0.4f);
                break;
            case GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                Time.timeScale = 0;
                PlayMusic(menuMusic);
                break;
            case GameState.Init:
                gunPanel.SetActive(true);
                statsPanel.SetActive(true);
                SpawnPlayer();
                ActivatePlayer();
                Weapon.instance.InstantiateWeapon(0);
                Invoke("ActivateHealthBar", 0.2f);
                Time.timeScale = 1;
                ChangeState(GameState.Playing);
                break;
            case GameState.Playing:
                SpawnEnemies();
                Time.timeScale = 1;
                PlayMusic(gamePlayMusic);
                break;
            case GameState.NewDay:
                AdvanceDay();
                RefillAmmunition();
                gunPanel.SetActive(true);
                statsPanel.SetActive(true);
                ActivatePlayer();
                ActivateHealthBar();
                DeactivateZombies("Zombie");
                SpawnEnemies();
                CurrencySystem.ResetCurrency();
                CurrencySystem.ResetEarnedCurrency();
                UpdateCurrencyText();
                WeaponManager.instance.UpdatePlayerWeapon();
                Time.timeScale = 1;
                PlayMusic(gamePlayMusic);
                break;
            case GameState.Death:
                statsPanel.SetActive(false);
                DeactivatePlayer();
                DeactivateHealthBar();
                DespawnEnemies();
                DeactivateZombies("Zombie");
                deathPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Retreat:
                PlayMusic(upgradeMusic);
                Weapon.instance.StopReload();
                DeactivatePlayer();
                DeactivateHealthBar();
                DespawnEnemies();
                DeactivateZombies("Zombie");
                CurrencySystem.BankCurrency();
                retreatPanel.SetActive(true);
                displayCurrencyScript.UpdateUI();
                statsPanel.SetActive(false);
                Time.timeScale = 0;
                break;
            case GameState.Upgrade:
                upgradePanel.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    void TransitionToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    void DisableBlackScreen()
    {

       blackScreen.SetActive(false);
    }
    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void StopMusic()
    {
        audioSource.Stop();
    }

    void RefillAmmunition()
    {
        Weapon.instance.weaponAmmo[Weapon.WeaponType.Pistol] = 10;
        Weapon.instance.weaponAmmo[Weapon.WeaponType.Shotgun] = 6;
    }

    void DeactivateHealthBar()
    {
        playerInstance.GetComponent<Player>().healthBarFill.enabled = false;
    }

    void ActivateHealthBar()
    {
        playerInstance.GetComponent<Player>().healthBarFill.enabled = true;
    }

    void DeactivateZombies(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Zombie zombie = obj.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.chaseSpeed = 1.5f;
                zombie.DeactivateZombie();
            }
        }
    }


    public void PlayGame()
    {
        ChangeState(GameState.Init);
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void AdvanceDay()
    {
        CurrentDay++;
        WeaponManager.instance.CheckForUnlockedWeapons(CurrentDay);
    }


    public void ActivatePlayer()
    {
        playerInstance.SetActive(true);
        playerInstance.GetComponent<Player>().enabled = true;
        playerInstance.transform.position = originalPosition;
    }

    public void DeactivatePlayer()
    {
        playerInstance.SetActive(false);
        playerInstance.GetComponent<Player>().enabled = false;
        playerInstance.SetActive(false);
    }

    public void StartNewDay()
    {
        WeaponManager.instance.SelectWeaponById(Player.instance.savedWeaponData.weaponId);
        ChangeState(GameState.NewDay);
    }

    public void ContinueToUpgrades()
    {
        ChangeState(GameState.Upgrade);
    }

    void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab);
        originalPosition = playerInstance.transform.position;
    }

    void UpdateCurrencyText()
    {
        Player player = playerInstance.GetComponent<Player>();
        player.UpdateUI();
    }
    

    void SpawnEnemies()
    {
        spawnManagerScript.enabled = true;
    }

    void DespawnEnemies()
    {
        spawnManagerScript.globalChaseSpeed = 1.5f;
        spawnManagerScript.spawnDelay = 5f;
        spawnManagerScript.enabled = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
