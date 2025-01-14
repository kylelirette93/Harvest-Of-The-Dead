using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponInventory
{
    public List<GameObject> weapons = new List<GameObject>();
    public void AddWeapon(GameObject weapon)
    {
        if (!weapons.Contains(weapon))
        {
            //Debug.Log("Weapon added to inventory: " + weapon.name);
            weapons.Add(weapon);
        }
    }

    public void RemoveWeapon(GameObject weapon)
    {
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
        }
    }

    public GameObject GetWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            return weapons[index];
        }
        return null;
    }
}
