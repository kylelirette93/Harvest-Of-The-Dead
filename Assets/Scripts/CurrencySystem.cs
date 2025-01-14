using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencySystem : IDataPersistence
{
    public static int currency;
    public static int bankedCurrency;
    public static int earnedCurrency;

   
    public void LoadData(GameData data)
    {
        currency = data.currency;
        bankedCurrency = data.bankedCurrency;
        earnedCurrency = data.earnedCurrency;
    }

    public void SaveData(ref GameData data)
    {
        data.currency = currency;
        data.earnedCurrency = earnedCurrency;
        data.bankedCurrency = bankedCurrency;
    }

    public static void AddCurrency()
    {
        int earned = Random.Range(10, 100);
        currency += earned;
        earnedCurrency += earned;
        Debug.Log("Currency added. Current currency: " + currency + ", Earned currency: " + earnedCurrency);
    }

    public static void BankCurrency()
    {
        bankedCurrency += currency;
        //Debug.Log("Banking currency. Banked currency: " + bankedCurrency);
        ResetCurrency();
    }

    public static int GetCurrency()
    {
        //Debug.Log("Getting currency. Current currency: " + currency);
        return currency;
    }

    public static void ResetCurrency()
    {
        //Debug.Log("Resetting currency. Previous currency: " + currency);
        currency = 0;
    }

    public static void ResetEarnedCurrency()
    {
        // Reset currency earned for the day.
        //Debug.Log("Resetting earned currency. Previous earned currency: " + earnedCurrency);
        earnedCurrency = 0;
    }
}