using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayCurrency : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    int currentCurrency;
    int bankedCurrency;

    public void UpdateUI()
    {
        currentCurrency = CurrencySystem.earnedCurrency;
        bankedCurrency = CurrencySystem.bankedCurrency;
        displayText.text = "You earned $" + currentCurrency + " today.\n" +
            "$" + bankedCurrency + " is your bank balance.";
    }

}
