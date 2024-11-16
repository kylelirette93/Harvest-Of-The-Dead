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
    }

    public void InstantiateWeapon()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 weaponOffset = new Vector3(0, 0.5f, 0);

            // Instantiate the weapon at the player's position with an offset
            GameObject weaponInstance = Instantiate(weaponsList[0], player.transform.position - weaponOffset, player.transform.rotation);

            // Set the current weapon and parent it to the player
            currentWeapon = weaponInstance;
            currentWeapon.transform.SetParent(player.transform);
        }
    }

    public void Shoot()
    {
        Debug.Log("Shooting!");
        if (canShoot && !isReloading)
        {
            // Instantiate a bullet at the weapon's position and rotation.
            GameObject bulletInstance = Instantiate(bulletPrefab, currentWeapon.transform.position, 
                player.transform.rotation);

            // Apply force to the bullet in the forward facing position.
            Vector2 shootDirection = (player.transform.up  * -0.15f).normalized;
            bulletInstance.GetComponent<Rigidbody2D>().AddForce(shootDirection * bulletSpeed, ForceMode2D.Impulse);

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
