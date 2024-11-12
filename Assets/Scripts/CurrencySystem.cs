using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencySystem
{
    public static int currency;
    public static int bankedCurrency = 0;
    public static int earnedCurrency = 0;

    public void AddCurrency()
    {
        int earned = Random.Range(10, 100);
        currency += earned;
        earnedCurrency += earned;
    }

    public static void BankCurrency()
    {
        bankedCurrency += currency;
        ResetCurrency();
    }

    public static void ResetCurrency()
    {
        currency = 0;
    }

    public static void ResetEarnedCurrency()
    {
        // Reset currency earned for the day.
        earnedCurrency = 0;
    }
}
