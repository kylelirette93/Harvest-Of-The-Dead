using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    public List<WeaponData> weaponsList = new List<WeaponData>();
    private WeaponData selectedWeapon;
    private WeaponInventory weaponInventory = new WeaponInventory();

    private void Awake()
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
    }

    public void SelectWeaponById(string weaponId)
    {
        selectedWeapon = weaponsList.Find(weapon => weapon.weaponId == weaponId);
        if (selectedWeapon != null)
        {
            Debug.Log("Weapon selected: " + selectedWeapon.weaponName);
        }
        else
        {
            Debug.LogError("Weapon not found: " + weaponId);
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

    public GameObject GetWeapon(int index)
    {
        return weaponInventory.GetWeapon(index);
    }

    public void UpgradeDamage(int additionalDamage)
    {
        if (selectedWeapon != null)
        {
            selectedWeapon.damage += additionalDamage;
            Debug.Log("Upgraded damage: " + selectedWeapon.damage);
        }
        else
        {
            // Set pistol as default weapon if no weapon is selected.
            selectedWeapon = weaponsList[0];
        }
    }

    public void UpgradeReloadSpeed(float reloadSpeedMultiplier)
    {
        if (selectedWeapon != null)
        {
            selectedWeapon.reloadSpeed = Mathf.Max(1, selectedWeapon.reloadSpeed - 0.2f);
            Debug.Log("Upgraded reload speed: " + selectedWeapon.reloadSpeed);
        }
        else
        {
            // Set pistol as default weapon if no weapon is selected.
            selectedWeapon = weaponsList[0];
        }
    }

    public void UpgradeFireSpeed(float fireSpeedIncrease)
    {
        if (selectedWeapon != null)
        {
            selectedWeapon.fireSpeed += fireSpeedIncrease;
            Debug.Log("Upgraded fire speed: " + selectedWeapon.fireSpeed);
        }
        else
        {
            // Set pistol as default weapon if no weapon is selected.
            selectedWeapon = weaponsList[0];
        }
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
    public int damage;
    public float reloadSpeed;
    public float fireSpeed;
}