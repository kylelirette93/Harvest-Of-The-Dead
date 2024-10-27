using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    // Prefab references.
    public GameObject[] weapons;
    public GameObject bulletPrefab;
    GameObject currentWeapon;

    // Variables.
    float bulletForce = 20f;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SpawnDefaultWeapon();
    }

    void SpawnDefaultWeapon()
    {
        // Instantiate pistol by default.
        GameObject pistolInstance = Instantiate(weapons[0].gameObject);
        // Make it child of the parent.
        pistolInstance.transform.SetParent(transform, false);
        // Keep track of current weapon.
        currentWeapon = pistolInstance;
    }

    public void FireWeapon()
    {
        // Instantiate a bullet at the weapon's position and rotation.
        GameObject bulletInstance = Instantiate(bulletPrefab, currentWeapon.transform.position, currentWeapon.transform.rotation);

        // Apply force to the bullet in the forward facing position.
        bulletInstance.GetComponent<Rigidbody2D>().AddForce(-transform.up * bulletForce, ForceMode2D.Impulse);
    }
}
