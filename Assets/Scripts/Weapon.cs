using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;

    int capacity;
    int maxCapacity;
    int reloadSpeed;
    public float fireSpeed = 0.25f;
    public int damage;
    int bulletSpeed;
    bool canShoot = true;
    bool isReloading = false;
    
    public GameObject player;
    public GameObject[] weaponsList;
    GameObject currentWeapon;
    public GameObject bulletPrefab;

    private void Awake()
    {
        instance = this;
    }
    public virtual void Start()
    {
        // Initialize variables to default(pistol).
        capacity = 10;
        maxCapacity = capacity;
        reloadSpeed = 2;
        damage = 10;
        bulletSpeed = 20;

        Vector3 weaponOffset = new Vector3(0, 0.5f, 0);
        GameObject weaponInstance = Instantiate(weaponsList[0], player.transform.position - weaponOffset, player.transform.rotation);
        currentWeapon = weaponInstance;
        currentWeapon.transform.SetParent(player.transform);
    }

    public void Shoot()
    {
        if (canShoot && !isReloading)
        {
            // Instantiate a bullet at the weapon's position and rotation.
            GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, transform.rotation);

            // Apply force to the bullet in the forward facing position.
            bulletInstance.GetComponent<Rigidbody2D>().AddForce(-transform.up * bulletSpeed, ForceMode2D.Impulse);

            capacity--;
        }
    }

    IEnumerator Reload(int delay)
    {
        isReloading = true;
        yield return new WaitForSeconds(delay);
        capacity = maxCapacity;
        isReloading = false;
        canShoot = true;
    }

    public virtual void Update()
    {      
        if (capacity <= 0 && !isReloading)
        {
            canShoot = false;
            StartCoroutine(Reload(reloadSpeed));
        }
    }
}
