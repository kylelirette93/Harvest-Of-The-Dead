using UnityEngine;

public class WeaponCycler : MonoBehaviour
{
    public KeyCode nextWeaponKey = KeyCode.E;
    public KeyCode previousWeaponKey = KeyCode.Q;

    private int currentWeaponIndex = 0;

    void Update()
    {
        int unlockedWeaponCount = WeaponManager.instance.unlockedWeapons.Count;

        if (unlockedWeaponCount <= 1) return;
        if (Input.GetKeyDown(nextWeaponKey))
        {
            CycleWeapon(1);
        }
        else if (Input.GetKeyDown(previousWeaponKey))
        {
            CycleWeapon(-1);
        }
    }

    void CycleWeapon(int direction)
    {
        // Get the total number of weapons from WeaponManager
        int unlockedWeaponCount = WeaponManager.instance.unlockedWeapons.Count;

        if (unlockedWeaponCount == 0) return;

        // Update the current index
        currentWeaponIndex = (currentWeaponIndex + direction + unlockedWeaponCount) % unlockedWeaponCount;

        // Select the weapon by index
        WeaponManager.instance.SelectWeaponByIndex(currentWeaponIndex);
    }
}