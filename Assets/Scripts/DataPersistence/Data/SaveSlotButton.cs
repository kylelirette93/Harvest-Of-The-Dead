using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] private int slotId;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        UpdateButtonText();
    }

    private void OnButtonClick()
    {
        DataPersistenceManager.instance.LoadGame(slotId);
    }

    private void UpdateButtonText()
    {
        string status = DataPersistenceManager.instance.GetSlotStatus(slotId);
        buttonText.text = status;
    }
}