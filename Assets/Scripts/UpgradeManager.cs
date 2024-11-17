using TMPro;
using UnityEngine;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    public int upgradePrice;
    public string selectedWeaponId;


    public static UpgradeManager instance;
    public GameObject upgradeDamageButton;
    public GameObject upgradeReloadSpeedButton;
    public GameObject upgradeFireSpeedButton;
    public GameObject weaponSelectionPanel;

    public TextMeshProUGUI availableCurrencyText;
    public TextMeshProUGUI upgradeDamageText;
    public TextMeshProUGUI upgradeReloadSpeedText;
    public TextMeshProUGUI upgradeFireSpeedText;
    public TextMeshProUGUI selectedWeaponText;

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
        UpdateAvailableCurrencyText();
    }

    public UpgradeState currentState;

    void Start()
    {
        ChangeState(UpgradeState.SelectWeapon);
    }

    private void Update()
    {
        upgradeDamageText.text = "damage: " + Weapon.instance.damage;
        upgradeReloadSpeedText.text = "reload speed: " + Weapon.instance.reloadSpeed;
        upgradeFireSpeedText.text = "fire speed: " + Weapon.instance.fireSpeed;

        if (WeaponManager.instance.GetSelectedWeaponData() == null)
        {
            selectedWeaponText.text = "upgrade: " + selectedWeaponId;
        }
        else
        {
            selectedWeaponText.text = "upgrade: " + WeaponManager.instance.GetSelectedWeaponData().weaponName;
        }
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

    IEnumerator ResetAvailableText(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
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

            WeaponManager.instance.UpgradeDamage(2);
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
            StartCoroutine(ResetAvailableText(1.5f));
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
            StartCoroutine(ResetAvailableText(1.5f));
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

            WeaponManager.instance.UpgradeFireSpeed(0.25f);
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
            StartCoroutine(ResetAvailableText(1.5f));
        }
    }
}