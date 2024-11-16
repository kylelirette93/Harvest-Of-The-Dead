using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;
    public GameObject upgradeDamageButton;
    public GameObject upgradeReloadSpeedButton;
    public GameObject upgradeFireSpeedButton;
    public string selectedWeaponId;
    public GameObject weaponSelectionPanel;
    public int upgradePrice;
    public TextMeshProUGUI availableCurrencyText;

    private void Awake()
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

    

    public enum UpgradeState
    {
        SelectWeapon,
        IncreaseHealth,
    }

    private void OnEnable()
    {
        ChangeState(UpgradeState.SelectWeapon);
    }

    public UpgradeState currentState;

    void Start()
    {
        ChangeState(UpgradeState.SelectWeapon);
        UpdateAvailableCurrencyText();
    }

    

    public void ChangeState(UpgradeState newState)
    {
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    public void ExitState(UpgradeState currentState)
    {
        switch (currentState)
        {
            case UpgradeState.SelectWeapon:
                weaponSelectionPanel.SetActive(false);
                break;
            case UpgradeState.IncreaseHealth:
                break;
        }
    }

    public void EnterState(UpgradeState newState)
    {
        switch (newState)
        {
            case UpgradeState.SelectWeapon:
                weaponSelectionPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case UpgradeState.IncreaseHealth:
                break;
        }
    }

    public void SelectWeaponById(string weaponId)
    {
        selectedWeaponId = weaponId;
        WeaponManager.instance.SelectWeaponById(weaponId);
        weaponSelectionPanel.SetActive(false);
        Time.timeScale = 0;
    }

    public void UpdateAvailableCurrencyText()
    {
        availableCurrencyText.text = "Available Funds: " + CurrencySystem.bankedCurrency;
    }

    public void OnUpgradeDamageButtonClicked(int upgradeCost)
    {
        upgradePrice = upgradeCost;
        Debug.Log("Attempting to upgrade damage. Current currency: " + CurrencySystem.bankedCurrency);
        if (CurrencySystem.bankedCurrency >= upgradePrice)
        {
            CurrencySystem.bankedCurrency -= upgradePrice;
            UpdateAvailableCurrencyText();
            Debug.Log("Currency deducted. New currency: " + CurrencySystem.bankedCurrency);

            WeaponManager.instance.UpgradeDamage(1);
            if (Weapon.instance != null)
            {
                Weapon.instance.ApplyUpgrades(WeaponManager.instance.GetSelectedWeaponData());
                Debug.Log("Weapon upgraded. New damage: " + Weapon.instance.damage);
            }
            else
            {
                Debug.LogError("Weapon instance is null.");
            }
        }
        else
        {
            availableCurrencyText.text = "Insufficient funds!";
            Debug.LogWarning("Insufficient funds for upgrade.");
        }
    }

    public void OnUpgradeReloadSpeedButtonClicked(int upgradeCost)
    {
        upgradePrice = upgradeCost;
        Debug.Log("Attempting to upgrade reload speed. Current currency: " + CurrencySystem.bankedCurrency);
        if (CurrencySystem.bankedCurrency >= upgradePrice)
        {
            CurrencySystem.bankedCurrency -= upgradePrice;
            UpdateAvailableCurrencyText();
            Debug.Log("Currency deducted. New currency: " + CurrencySystem.bankedCurrency);

            WeaponManager.instance.UpgradeReloadSpeed(0.8f);
            if (Weapon.instance != null)
            {
                Weapon.instance.ApplyUpgrades(WeaponManager.instance.GetSelectedWeaponData());
                Debug.Log("Weapon upgraded. New reload speed: " + Weapon.instance.reloadSpeed);
            }
            else
            {
                Debug.LogError("Weapon instance is null.");
            }
        }
        else
        {
            availableCurrencyText.text = "Insufficient funds!";
            Debug.LogWarning("Insufficient funds for upgrade.");
        }
    }

    public void OnUpgradeFireSpeedButtonClicked(int upgradeCost)
    {
        upgradePrice = upgradeCost;
        Debug.Log("Attempting to upgrade fire speed. Current currency: " + CurrencySystem.bankedCurrency);
        if (CurrencySystem.bankedCurrency >= upgradePrice)
        {
            CurrencySystem.bankedCurrency -= upgradePrice;
            UpdateAvailableCurrencyText();
            Debug.Log("Currency deducted. New currency: " + CurrencySystem.bankedCurrency);

            WeaponManager.instance.UpgradeFireSpeed(0.9f);
            if (Weapon.instance != null)
            {
                Weapon.instance.ApplyUpgrades(WeaponManager.instance.GetSelectedWeaponData());
                Debug.Log("Weapon upgraded. New fire speed: " + Weapon.instance.fireSpeed);
            }
            else
            {
                Debug.LogError("Weapon instance is null.");
            }
        }
        else
        {
            availableCurrencyText.text = "Insufficient funds!";
            Debug.LogWarning("Insufficient funds for upgrade.");
        }
    }
}