using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencySystem
{
    public static int currency = 0;
    public static int bankedCurrency = 0;
    public static int earnedCurrency = 0;

    [RuntimeInitializeOnLoadMethod]
    private static void ResetStatics()
    {
        currency = 3000;
        bankedCurrency = 0;
        earnedCurrency = 0;
        Debug.Log("CurrencySystem static fields reset.");
    }

    public void AddCurrency()
    {
        int earned = Random.Range(10, 100);
        currency += earned;
        earnedCurrency += earned;
        Debug.Log("Currency added. Current currency: " + currency + ", Earned currency: " + earnedCurrency);
    }

    public static void BankCurrency()
    {
        bankedCurrency += currency;
        Debug.Log("Banking currency. Banked currency: " + bankedCurrency);
        ResetCurrency();
    }

    public static void ResetCurrency()
    {
        Debug.Log("Resetting currency. Previous currency: " + currency);
        currency = 0;
    }

    public static void ResetEarnedCurrency()
    {
        // Reset currency earned for the day.
        Debug.Log("Resetting earned currency. Previous earned currency: " + earnedCurrency);
        earnedCurrency = 0;
    }
}