using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    public GameObject studioNamePanel;
    public VideoPlayer studioNameVideoPlayer;
    public GameObject mainMenuPanel;
    public GameObject fileSelectionPanel;
    public GameObject instructionsPanel;
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
    private int currentCurrency;

    List<GameObject> tempDeactivatedHealthBars = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        SelectFile,
        Instructions,
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
                break;
            case GameState.SelectFile:
                fileSelectionPanel.SetActive(false);
                break;
                case GameState.Instructions:
                    instructionsPanel.SetActive(false);
                break;
            case GameState.Init:
                break;
            case GameState.Playing:
                gunPanel.SetActive(false);
                statsPanel.SetActive(false);
                StopMusic();
                EnableCursor();
                break;
            case GameState.NewDay:
                EnableCursor();
                break;
            case GameState.Death:
                deathPanel.SetActive(false);
                StopMusic();
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
                Time.timeScale = 1;
                PlayMusic(menuMusic);
                break;
            case GameState.SelectFile:
                fileSelectionPanel.SetActive(true);
                break;
            case GameState.Instructions:
                                    instructionsPanel.SetActive(true);
                break;
            case GameState.Init:
                StopMusic();
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
                DisableCursor();
                Invoke("SpawnEnemies", 0.1f);
                Time.timeScale = 1;
                PlayMusic(gamePlayMusic);
                break;
            case GameState.NewDay:
                DisableCursor();
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
                DeactivateHealthBar();
                DeactivatePlayer();
                DespawnEnemies();
                DeactivateZombies("Zombie");
                deathPanel.SetActive(true);
                Time.timeScale = 0;
                PlayMusic(menuMusic);
                break;
            case GameState.Retreat:
                PlayMusic(upgradeMusic);
                Weapon.instance.StopReload();
                DeactivateHealthBar();
                DeactivatePlayer();
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

    void DisableCursor()
    {
        Cursor.visible = false; // Hides the cursor
        Cursor.lockState = CursorLockMode.Confined; // Locks the cursor to the center of the screen
    }

    void EnableCursor()
    {
        Cursor.visible = true; // Shows the cursor
        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
    }
    public void LoadData(GameData data)
    {
        this.CurrentDay = data.currentDay;
    }

    public void SaveData(ref GameData data)
    {
        data.currentDay = CurrentDay;
    }

    void TransitionToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    void DisableBlackScreen()
    {

       blackScreen.SetActive(false);
    }

    public void InstructionsButtonClicked()     {
        ChangeState(GameState.Instructions);
    }
    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying)
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
        if (playerInstance != null)
        {
            playerInstance.GetComponent<Player>().healthBarFill.enabled = false;
        }
    }

    void ActivateHealthBar()
    {
        if (playerInstance != null)
        {
            playerInstance.GetComponent<Player>().healthBarFill.enabled = true;
        }
    }

    void DeactivateHealthBars()
    {
        tempDeactivatedHealthBars.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("HealthBar"))
        {
            tempDeactivatedHealthBars.Add(obj);
            obj.SetActive(false);
        }
    }

    void ReactivateHealthBars()     {
        foreach (GameObject obj in tempDeactivatedHealthBars)
        {

            obj.SetActive(true);
        }
        tempDeactivatedHealthBars.Clear();
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

    public void FileSelection()     {
        ChangeState(GameState.SelectFile);
    }

    public void PlayGame()
    {
        ChangeState(GameState.Init);
    }

    public void Pause()
    {
        Player.instance.DisableCrosshair();
        EnableCursor();
        Weapon.instance.canShoot = false;
        DeactivateHealthBars();
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Player.instance.EnableCrosshair();
        DisableCursor();
        Weapon.instance.canShoot = true;
        ReactivateHealthBars();
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
        if (playerInstance != null)
        {
            //Debug.Log("Activating player instance");
            playerInstance.SetActive(true);
            Player playerScript = playerInstance.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.enabled = true;
                playerInstance.transform.position = originalPosition;
            }
            else
            {
                //Debug.LogError("Player component not found on playerInstance");
            }
        }
        else
        {
            //Debug.LogError("playerInstance is null, cannot activate player");
        }
    }

    public void DeactivatePlayer()
    {
        if (playerInstance != null)
        {
            //Debug.Log("Deactivating player instance");
            playerInstance.SetActive(false);
            playerInstance.GetComponent<Player>().enabled = false;
        }
        else
        {
            //Debug.LogWarning("Player instance is already null or destroyed");
        }
    }

    public void StartNewDay()
    {
        WeaponManager.instance.SelectWeaponById(Player.instance.savedWeaponData.weaponId);
        ChangeState(GameState.NewDay);
    }

    public void BackToMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ContinueToUpgrades()
    {
        ChangeState(GameState.Upgrade);
    }

    void SpawnPlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            originalPosition = playerInstance.transform.position;
        }
        else
        {
           // Debug.LogWarning("Player instance already exists. Skipping spawn.");
        }
    }

    void UpdateCurrencyText()
    {
        currentCurrency = CurrencySystem.GetCurrency();
        if (playerInstance != null)
        {
            Player player = playerInstance.GetComponent<Player>();
            player.UpdateUI(currentCurrency);
        }
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
