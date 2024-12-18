using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    public int upgradePrice;
    public string selectedWeaponId;

    public static UpgradeManager instance;
    public GameObject upgradeDamageButton;
    public GameObject upgradeReloadSpeedButton;
    public GameObject upgradeFireSpeedButton;
    public GameObject weaponSelectionPanel;
    public GameObject upgradeButtonPanel;

    public TextMeshProUGUI availableCurrencyText;
    public TextMeshProUGUI upgradeDamageText;
    public TextMeshProUGUI upgradeReloadSpeedText;
    public TextMeshProUGUI upgradeFireSpeedText;
    public TextMeshProUGUI selectedWeaponText;

    // UI references for weapon icons.
    public Image pistolIcon;
    public Image shotgunIcon;
    public Button pistolButton;
    public Button shotgunButton;
    static bool weaponPurchased = false;

    bool canBuyUpgrade = true;

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
        UpgradeWeapon,
    }

    private void OnEnable()
    {
        ChangeState(UpgradeState.SelectWeapon);
        UpdateAvailableCurrencyText();
    }

    public UpgradeState currentState;

    void Start()
    {
        ValidateUpgrades();

        // Display funds available to player.
        availableCurrencyText.text = "available funds: " + CurrencySystem.bankedCurrency;

        // Ensure the pistol is unlocked by default if not already unlocked.
        UnlockWeaponByIdIfNotUnlocked("Pistol");

        UpdateWeaponIcons();

        // Check if the pistol can be unlocked at the start.
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == "Pistol");
        if (weaponData != null && GameManager.instance.CurrentDay >= weaponData.unlockDay)
        {
            CheckWeaponUnlock(GameManager.instance.CurrentDay, weaponData.unlockDay, "Pistol");
        }

        if (shotgunButton != null)
        {
            shotgunButton.onClick.AddListener(() => UnlockShotgun(2));
        }


        if (pistolButton != null)
        {
            pistolButton.onClick.AddListener(UpgradePistol);
        }
    }

    

    private void SetButtonHighlightColor(Button button, Color highlightColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.highlightedColor = highlightColor;
        button.colors = colorBlock;
    }

    public void IsNextDay()
    {
        ResetWeaponSelection();
    }

    public void ResetWeaponSelection()
    {
        // Logic for resetting weapon selection on new day.
        selectedWeaponId = null;
        WeaponManager.instance.SelectWeaponById(null);
        weaponSelectionPanel.SetActive(true);
        upgradeButtonPanel.SetActive(false);
        UpdateWeaponIcons();
    }

    public void CheckWeaponUnlock(int currentDay, int unlockDay, string weaponId)
    {
        if (currentDay >= unlockDay)
        {
            WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == weaponId);
            if (weaponData != null && weaponData.weaponId != "Pistol")
            {
                int unlockPrice = weaponData.unlockPrice;
                if (CurrencySystem.bankedCurrency >= unlockPrice)
                {
                    CurrencySystem.bankedCurrency -= unlockPrice;
                    WeaponManager.instance.UnlockWeapon(weaponId);
                    Debug.Log($"Weapon {weaponId} unlocked!");
                    availableCurrencyText.text = $"new weapon available: {weaponId}";
                }
                else
                {
                    availableCurrencyText.text = "insufficient funds!";
                }
            }
        }
        else
        {
            availableCurrencyText.text = "insufficient funds!";
        }
    }

    private void UnlockWeaponByIdIfNotUnlocked(string weaponId)
    {
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == weaponId);
        if (weaponData != null && GameManager.instance.CurrentDay >= weaponData.unlockDay)
        {
            int unlockPrice = weaponData.unlockPrice;
            if (CurrencySystem.bankedCurrency >= unlockPrice)
            {
                CurrencySystem.bankedCurrency -= unlockPrice;
                WeaponManager.instance.UnlockWeapon(weaponId);
            }
        }
    }

    private void Update()
    {
        // Keep the selected weapon text update
        if (WeaponManager.instance.GetSelectedWeaponData() == null)
        {
            selectedWeaponText.text = "upgrade: " + selectedWeaponId;
        }
        else
        {
            selectedWeaponText.text = "upgrade: " + WeaponManager.instance.GetSelectedWeaponData().weaponName;
        }

    }

    private void UpdateWeaponIcons()
    {
        UpdateWeaponIcon(pistolIcon, "Pistol", pistolButton);
        UpdateWeaponIcon(shotgunIcon, "Shotgun", shotgunButton);
    }

    private void UpdateWeaponIcon(Image iconImage, string weaponId, Button button)
    {
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == weaponId);
        if (weaponData != null)
        {
            bool isUnlocked = GameManager.instance.CurrentDay >= weaponData.unlockDay;
            bool isPurchased = WeaponManager.instance.unlockedWeapons.Contains(weaponData);
            bool canPurchase = CurrencySystem.bankedCurrency >= weaponData.unlockPrice;

            Debug.Log($"Weapon: {weaponId}, CurrentDay: {GameManager.instance.CurrentDay}, UnlockDay: {weaponData.unlockDay}, IsUnlocked: {isUnlocked}, IsPurchased: {isPurchased}, CanPurchase: {canPurchase}");

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleWeaponButtonClick(weaponData, isUnlocked, isPurchased, canPurchase));

            if (isPurchased)
            {
                iconImage.sprite = weaponData.weaponIcon;
                selectedWeaponText.text = $"{weaponData.weaponName} is purchased!";
            }
            else if (isUnlocked)
            {
                iconImage.sprite = weaponData.weaponIcon;
                selectedWeaponText.text = canPurchase ? $"{weaponData.weaponName} is available for purchase!" : "Insufficient funds!";
            }
            else
            {
                iconImage.sprite = weaponData.lockedIcon;
                selectedWeaponText.text = $"{weaponData.weaponName.ToLower()} will unlock on day {weaponData.unlockDay}";
            }

            button.interactable = true; // Always make the button interactable
        }
    }

    private void HandleWeaponButtonClick(WeaponData weaponData, bool isUnlocked, bool isPurchased, bool canPurchase)
    {
        if (isPurchased)
        {
            DisplayTemporaryMessage($"{weaponData.weaponName} is already purchased!", 2f);
        }
        else if (isUnlocked)
        {
            if (canPurchase)
            {
                CurrencySystem.bankedCurrency -= weaponData.unlockPrice;
                WeaponManager.instance.UnlockWeapon(weaponData.weaponId);
                DisplayTemporaryMessage($"{weaponData.weaponName} purchased!", 2f);
                UpdateWeaponIcons();
            }
            else
            {
                DisplayTemporaryMessage("insufficient funds!", 2f);
            }
        }
        else
        {
            DisplayTemporaryMessage($"{weaponData.weaponName} will unlock on day {weaponData.unlockDay}", 2f);
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
                Time.timeScale = 0;
                break;
            case UpgradeState.UpgradeWeapon:
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
            case UpgradeState.UpgradeWeapon:
                upgradeButtonPanel.SetActive(true);
                break;
        }
    }

    public void SelectWeaponById(string weaponId)
    {
        selectedWeaponId = weaponId;

        if (WeaponManager.instance == null)
        {
            Debug.LogError("WeaponManager instance is null.");
            return;
        }

        WeaponManager.instance.SelectWeaponById(weaponId);

        WeaponData selectedWeapon = WeaponManager.instance.GetSelectedWeaponData();
        if (selectedWeapon == null)
        {
            Debug.LogError("Selected weapon data is null.");
            availableCurrencyText.text = "available funds: " + CurrencySystem.bankedCurrency;
            return;
        }

        Debug.Log($"Selected weapon: {selectedWeapon.weaponName}");
        Debug.Log($"Is weapon unlocked: {WeaponManager.instance.unlockedWeapons.Contains(selectedWeapon)}");

        if (WeaponManager.instance.unlockedWeapons.Contains(selectedWeapon))
        {
            availableCurrencyText.text = "selected: " + selectedWeapon.weaponName;
            ChangeState(UpgradeState.UpgradeWeapon);
            upgradeButtonPanel.SetActive(true);
        }
        else
        {
            availableCurrencyText.text = "weapon locked!";
            StartCoroutine(ResetAvailableText(2f));
        }

        UpdateWeaponIcons();
        UpdateUpgradeText();
    }

    private void UpdateUpgradeText()
    {
        WeaponData selectedWeapon = WeaponManager.instance.GetSelectedWeaponData();

        if (selectedWeapon != null)
        {
            // Upgrade text when a weapon is selected.
            upgradeDamageText.text = "damage: " + selectedWeapon.damage + " ($" + selectedWeapon.damageUpgradePrice + ")";
            upgradeReloadSpeedText.text = "reload speed: " + selectedWeapon.reloadSpeed + " ($" + selectedWeapon.reloadSpeedUpgradePrice + ")";
            upgradeFireSpeedText.text = "fire speed: " + selectedWeapon.fireSpeed + " ($" + selectedWeapon.fireSpeedUpgradePrice + ")";
        }
    }

    public void UpdateAvailableCurrencyText()
    {
        availableCurrencyText.text = "available funds: " + CurrencySystem.bankedCurrency;
    }

    private void DisplayTemporaryMessage(string message, float delay)
    {
        availableCurrencyText.text = message;
        StartCoroutine(ResetAvailableText(delay));
    }

    IEnumerator ResetAvailableText(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        UpdateAvailableCurrencyText();
    }

    

    public void UnlockShotgun(int unlockDay)
    {
        Debug.Log("UnlockShotgun method called."); // Add this line for debugging
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == "Shotgun");
        if (weaponData != null)
        {
            Debug.Log($"Current Day: {GameManager.instance.CurrentDay}, Unlock Day: {unlockDay}");
            Debug.Log($"Banked Currency: {CurrencySystem.bankedCurrency}, Unlock Price: {weaponData.unlockPrice}");

            if (GameManager.instance.CurrentDay >= unlockDay && !WeaponManager.instance.unlockedWeapons.Contains(weaponData))
            {
                if (CurrencySystem.bankedCurrency >= weaponData.unlockPrice)
                {
                    // Deduct the funds
                    CurrencySystem.bankedCurrency -= weaponData.unlockPrice;

                    // Unlock the shotgun
                    WeaponManager.instance.unlockedWeapons.Add(weaponData);
                    UpdateWeaponIcons();

                    // Provide feedback to the user
                    DisplayTemporaryMessage("shotgun unlocked!", 2f);

                    WeaponManager.instance.SelectWeaponById("Shotgun");
                    SetShotgunUpgradeListener();
                    StartCoroutine(UpdateSelectedWeaponTextWithDelay(2f, weaponData.weaponName));
                    ChangeState(UpgradeState.UpgradeWeapon);
                }
                else
                {
                    DisplayTemporaryMessage("insufficient funds!", 2f);
                }
            }
            else if (WeaponManager.instance.unlockedWeapons.Contains(weaponData))
            {
                // Select the shotgun if it is already unlocked
                WeaponManager.instance.SelectWeaponById("Shotgun");

                // Set the button listener to select the shotgun for upgrades
                SetShotgunUpgradeListener();
                availableCurrencyText.text = "selected: " + weaponData.weaponName;
                ChangeState(UpgradeState.UpgradeWeapon);
            }
            else
            {
                Debug.Log("Conditions not met for unlocking the shotgun.");
            }
        }
        else
        {
            Debug.LogError("Shotgun data not found in weapons list.");
        }
    }

    private IEnumerator UpdateSelectedWeaponTextWithDelay(float delay, string weaponName)
    {
        yield return new WaitForSecondsRealtime(delay);
        availableCurrencyText.text = "selected: " + weaponName;
    }

    private void SetShotgunUpgradeListener()
    {
        if (shotgunButton != null)
        {
            shotgunButton.onClick.RemoveAllListeners();
            shotgunButton.onClick.AddListener(() => WeaponManager.instance.SelectWeaponById("Shotgun"));
            selectedWeaponText.text = "Upgrade Shotgun";
        }
    }

    public void UpgradePistol()
    {
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == "Pistol");
        if (weaponData != null)
        {
            WeaponManager.instance.SelectWeaponById("Pistol");
            availableCurrencyText.text = "selected: " + weaponData.weaponName;
            ChangeState(UpgradeState.UpgradeWeapon);
        }

    }

    public void AttemptWeaponPurchase(string weaponId)
    {
        WeaponData weaponData = WeaponManager.instance.weaponsList.Find(w => w.weaponId == weaponId);
        if (weaponData != null && GameManager.instance.CurrentDay >= weaponData.unlockDay)
        {
            if (CurrencySystem.bankedCurrency >= weaponData.unlockPrice)
            {
                CurrencySystem.bankedCurrency -= weaponData.unlockPrice;
                WeaponManager.instance.UnlockWeapon(weaponId);
                availableCurrencyText.text = $"{weaponId} purchased!";
                UpdateWeaponIcons();
            }
            else
            {
                DisplayTemporaryMessage("insufficient funds!", 2f);
            }
        }
    }
    public void AttemptWeaponUnlock(string weaponId, int unlockPrice)
    {
        if (CurrencySystem.bankedCurrency >= unlockPrice)
        {
            CurrencySystem.bankedCurrency -= unlockPrice;
            WeaponManager.instance.UnlockWeapon(weaponId);

            availableCurrencyText.text = $"{weaponId} unlocked!";
            UpdateWeaponIcons();
        }
        else
        {
            availableCurrencyText.text = "you need $" + (unlockPrice - CurrencySystem.bankedCurrency) + " more.";
        }
    }

    public void AttemptUpgrade(string upgradeType, int upgradeCost, float valueChange)
    {
        Debug.Log("Attempting upgrade");
        if (!canBuyUpgrade)
        {
            Debug.Log("Cannot buy upgrade at this time.");
            return;
        }

        if (CurrencySystem.bankedCurrency >= upgradeCost)
        {
            CurrencySystem.bankedCurrency -= upgradeCost;
            UpdateAvailableCurrencyText();

            if (WeaponManager.instance == null)
            {
                Debug.LogError("WeaponManager instance is null.");
                return;
            }

            WeaponManager.instance.UpgradeWeaponStat(upgradeType, valueChange);

            if (Weapon.instance != null)
            {
                Weapon.instance.ApplyUpgrades(WeaponManager.instance.GetSelectedWeaponData());
                ValidateUpgrades();
            }
            else
            {
                Debug.LogError("Weapon instance is null.");
            }
        }
        else
        {
            DisplayTemporaryMessage("insufficient funds!", 1.5f);
        }
    }


    private void ValidateUpgrades()
    {
        WeaponData selectedWeapon = WeaponManager.instance.GetSelectedWeaponData();
        if (selectedWeapon == null) return;

        // Check if the weapon's damage is maxed out
        if (Weapon.instance.damage >= selectedWeapon.maxDamage)
        {
            upgradeDamageText.text = "damage maxed out!";
            upgradeDamageButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            upgradeDamageText.text = "damage: " + Weapon.instance.damage + " ($" + selectedWeapon.damageUpgradePrice + ")";
            upgradeDamageButton.GetComponent<Button>().interactable = true;
        }

        // Check if the weapon's reload speed is minned out
        if (Weapon.instance.reloadSpeed <= selectedWeapon.minReloadSpeed)
        {
            upgradeReloadSpeedText.text = "reload speed maxed out!";
            upgradeReloadSpeedButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            upgradeReloadSpeedText.text = "reload speed: " + Weapon.instance.reloadSpeed + " ($" + selectedWeapon.reloadSpeedUpgradePrice + ")";
            upgradeReloadSpeedButton.GetComponent<Button>().interactable = true;
        }

        // Check if the weapon's fire speed is minned out
        if (Weapon.instance.fireSpeed <= selectedWeapon.minFireSpeed)
        {
            upgradeFireSpeedText.text = "fire speed maxed out!";
            upgradeFireSpeedButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            upgradeFireSpeedText.text = "fire speed: " + Weapon.instance.fireSpeed + " ($" + selectedWeapon.fireSpeedUpgradePrice + ")";
            upgradeFireSpeedButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OnUpgradeDamageButtonClicked()
    {
        WeaponData weaponData = WeaponManager.instance.GetSelectedWeaponData();
        int upgradeCost = (int)weaponData.damageUpgradePrice;
        Debug.Log("Upgrade damage button clicked.");
        AttemptUpgrade("Damage", upgradeCost, 2f);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnUpgradeReloadSpeedButtonClicked()
    {
        WeaponData weaponData = WeaponManager.instance.GetSelectedWeaponData();
        int upgradeCost = (int)weaponData.reloadSpeedUpgradePrice;
        AttemptUpgrade("ReloadSpeed", upgradeCost, 0.2f);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnUpgradeFireSpeedButtonClicked()
    {
        WeaponData weaponData = WeaponManager.instance.GetSelectedWeaponData();
        int upgradeCost = (int)weaponData.fireSpeedUpgradePrice;
        AttemptUpgrade("FireSpeed", upgradeCost, 0.05f);
        EventSystem.current.SetSelectedGameObject(null);
    }
}