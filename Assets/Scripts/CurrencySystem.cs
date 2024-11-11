using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencySystem
{
    public int currency;

    public void AddCurrency()
    {
        currency += Random.Range(10, 100);
    }
}
