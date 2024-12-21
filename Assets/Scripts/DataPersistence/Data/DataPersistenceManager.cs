using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
   public static DataPersistenceManager instance {  get; private set; }

    private int currentSlotId = 0; // Default slot id.
    private Dictionary<int, string> slotStatuses = new Dictionary<int, string>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one data persistence manager in scene");  
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        UpdateSlotStatuses();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame(int slotId)
    {
        currentSlotId = slotId;
        // Load any saved data from a file using data handler.
        this.gameData = dataHandler.Load(slotId);
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }

        Debug.Log("Loaded Current day = " + gameData.currentDay);
        GameManager.instance.PlayGame();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in this.dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        dataHandler.Save(gameData, currentSlotId);
        UpdateSlotStatuses();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void UpdateSlotStatuses()
    {
        for (int i = 0; i < 3; i++)
        {
            GameData data = dataHandler.Load(i);
            if (data == null)
            {
                slotStatuses[i] = "Empty";
            }
            else
            {
                slotStatuses[i] = "Day: " + data.currentDay;
            }
        }
    }

    public string GetSlotStatus(int slotId)
    {
        if (slotStatuses.ContainsKey(slotId))
        {
            return slotStatuses[slotId];
        }
        return "Empty";
    }

}
