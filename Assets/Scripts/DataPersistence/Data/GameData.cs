using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentDay;
    public int earnedCurrency;
    public int bankedCurrency;
    public int currency;
    public List<WeaponData> storedWeaponsData; // Changed to a list of WeaponData

    public GameData()
    {
        this.currentDay = 1;
        this.earnedCurrency = 0;
        this.bankedCurrency = 0;
        this.currency = 0;

        // Initialize the list and add two weapons
        this.storedWeaponsData = new List<WeaponData>
        {
            new WeaponData
            {
                weaponId = "Pistol",
                weaponName = "Pistol",
                damage = 10,
                reloadSpeed = 2,
                fireSpeed = 0.4f,
                unlockDay = 1,
                damageUpgradePrice = 300,
                reloadSpeedUpgradePrice = 200,
                fireSpeedUpgradePrice = 250,
                unlockPrice = 0,
                maxDamage = 20,
                minReloadSpeed = 1,
                minFireSpeed = 0.15f
            },
            new WeaponData
            {
                weaponId = "Shotgun",
                weaponName = "Shotgun",
                damage = 20,
                reloadSpeed = 3f,
                fireSpeed = 1.5f,
                unlockDay = 10,
                damageUpgradePrice = 500,
                reloadSpeedUpgradePrice = 400,
                fireSpeedUpgradePrice = 450,
                unlockPrice = 2000,
                maxDamage = 30,
                minReloadSpeed = 2,
                minFireSpeed = 1.25f
            }
        };
    }
}