using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public List<WeaponData> weaponsList = new List<WeaponData>();
    public Dictionary<string, WeaponData> weaponsDictionary = new Dictionary<string, WeaponData>();
    public List<WeaponData> unlockedWeapons = new List<WeaponData>();

    private WeaponData selectedWeapon;
    private WeaponInventory weaponInventory = new WeaponInventory();
    private HashSet<string> purchasedWeapons = new HashSet<string>();
    private WeaponData lastSelectedWeaponData;
    public string selectedWeaponId;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeWeaponsDictionary();

            string pistolWeaponId = "Pistol";
            purchasedWeapons.Add(pistolWeaponId);
            UnlockWeapon(pistolWeaponId);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCurrentWeaponData(WeaponData weaponData)
    {
        lastSelectedWeaponData = weaponData;
    }

    public WeaponData GetLastSelectedWeaponData()
    {
        return lastSelectedWeaponData;
    }

    private void InitializeWeaponsDictionary()
    {
        foreach (var weapon in weaponsList)
        {
            if (!weaponsDictionary.ContainsKey(weapon.weaponId))
            {
                weaponsDictionary.Add(weapon.weaponId, weapon);
            }
        }
    }

    public void UpdatePlayerWeapon()
    {
        string weaponId = PlayerPrefs.GetString("SelectedWeaponId", null);
        if (!string.IsNullOrEmpty(weaponId))
        {
            SelectWeaponById(weaponId);
        }
        else
        {
            Debug.LogWarning("No weapon ID found in PlayerPrefs.");
        }
    }

    public void CheckForUnlockedWeapons(int currentDay)
    {
        foreach (var weapon in weaponsList)
        {
            if (weapon.unlockDay <= currentDay && !unlockedWeapons.Contains(weapon) && purchasedWeapons.Contains(weapon.weaponId))
            {
                unlockedWeapons.Add(weapon);
                Debug.Log("Weapon unlocked: " + weapon.weaponName);
            }
        }
    }

    public void SelectWeaponById(string weaponId)
    {
        selectedWeaponId = weaponId;

        WeaponData selectedWeapon = GetWeaponDataById(weaponId);
        if (selectedWeapon != null)
        {
            this.selectedWeapon = selectedWeapon;
            PlayerPrefs.SetString("SelectedWeaponId", weaponId);
            PlayerPrefs.Save();

            if (Weapon.instance != null)
            {
                Weapon.instance.SetWeaponData(selectedWeapon);
                UpdateUpgradeUI();
            }
            UpdateSelectedWeaponUI();
            Debug.Log($"Weapon {weaponId} selected: {selectedWeapon.weaponName}");
        }
        else
        {
            Debug.LogWarning("Selected weapon data is null.");
        }
    }



    public void SelectWeaponByIndex(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count)
        {
            Debug.LogWarning("Invalid weapon index: " + index);
            return;
        }

        selectedWeapon = unlockedWeapons[index];
        Debug.Log("Selected weapon: " + selectedWeapon.weaponName);

        Weapon.instance.InstantiateWeapon(index);
        UpdateSelectedWeaponUI();
    }

    private void UpdateSelectedWeaponUI()
    {
        if (UpgradeManager.instance != null && selectedWeapon != null)
        {
            UpgradeManager.instance.selectedWeaponText.text = $"{selectedWeapon.weaponName} is selected!";
        }
    }

    public void AddWeapon(GameObject weapon)
    {
        weaponInventory.AddWeapon(weapon);
    }

    public void RemoveWeapon(GameObject weapon)
    {
        weaponInventory.RemoveWeapon(weapon);
    }

    public void PurchaseWeapon(string weaponId)
    {
        if (weaponsDictionary.TryGetValue(weaponId, out WeaponData weaponData))
        {
            purchasedWeapons.Add(weaponId);
            Debug.Log($"Weapon {weaponId} purchased.");
        }
    }

    public void UnlockWeapon(string weaponId)
    {
        if (weaponsDictionary.TryGetValue(weaponId, out WeaponData weaponData) && !unlockedWeapons.Contains(weaponData) && purchasedWeapons.Contains(weaponId))
        {
            unlockedWeapons.Add(weaponData);
            Debug.Log($"Weapon {weaponId} unlocked.");
        }
    }

    public GameObject GetWeapon(int index)
    {
        return weaponInventory.GetWeapon(index);
    }

    public void UpgradeWeaponStat(string statType, float valueChange)
    {
        if (selectedWeapon == null)
        {
            Debug.LogWarning("No weapon selected to upgrade.");
            return;
        }

        switch (statType)
        {
            case "Damage":
                selectedWeapon.damage = Mathf.Min(selectedWeapon.damage + valueChange, selectedWeapon.maxDamage);
                Debug.Log("Upgraded damage: " + selectedWeapon.damage);
                break;

            case "ReloadSpeed":
                selectedWeapon.reloadSpeed = Mathf.Max(selectedWeapon.reloadSpeed - valueChange, selectedWeapon.minReloadSpeed);
                Debug.Log("Upgraded reload speed: " + selectedWeapon.reloadSpeed);
                break;

            case "FireSpeed":
                selectedWeapon.fireSpeed = Mathf.Max(selectedWeapon.fireSpeed - valueChange, selectedWeapon.minFireSpeed);
                Debug.Log("Upgraded fire speed: " + selectedWeapon.fireSpeed);
                break;

            default:
                Debug.LogWarning($"Unknown stat type: {statType}");
                break;
        }

        UpdateUpgradeUI();
    }

    private void UpdateUpgradeUI()
    {
        if (UpgradeManager.instance != null && selectedWeapon != null)
        {
            UpgradeManager.instance.upgradeDamageText.text = selectedWeapon.damage >= selectedWeapon.maxDamage ? "Maxed Out!" : $"damage: {selectedWeapon.damage}" + " ($" + selectedWeapon.damageUpgradePrice + ")";
            UpgradeManager.instance.upgradeReloadSpeedText.text = selectedWeapon.reloadSpeed <= selectedWeapon.minReloadSpeed ? "Maxed Out!" : $"reload Speed: {selectedWeapon.reloadSpeed}" + " ($" + selectedWeapon.reloadSpeedUpgradePrice + ")";
            UpgradeManager.instance.upgradeFireSpeedText.text = selectedWeapon.fireSpeed <= selectedWeapon.minFireSpeed ? "Maxed Out!" : $"fire Speed: {selectedWeapon.fireSpeed}" + " ($" + selectedWeapon.fireSpeedUpgradePrice + ")";
        }
    }

    public WeaponData GetWeaponDataById(string weaponId)
    {
        return weaponsList.Find(w => w.weaponId == weaponId);
    }

    public WeaponData GetSelectedWeaponData()
    {
        return selectedWeapon;
    }
}

[System.Serializable]
public class WeaponData
{
    public string weaponId;
    public string weaponName;
    public float damage;
    public float reloadSpeed;
    public float fireSpeed;
    public int unlockDay;
    public Sprite weaponIcon;
    public Sprite lockedIcon;
    public float damageUpgradePrice;
    public float reloadSpeedUpgradePrice;
    public float fireSpeedUpgradePrice;
    public int unlockPrice;

    public float maxDamage;
    public float minReloadSpeed;
    public float minFireSpeed;
}