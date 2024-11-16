using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;

    string weaponId;
    int capacity;
    int maxCapacity;
    public int reloadSpeed;
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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void Start()
    {
        // Initialize variables to default(pistol).
        capacity = 10;
        maxCapacity = capacity;
        reloadSpeed = 2;
        damage = 10;
        bulletSpeed = 20;

        // Add the pistol to weapon inventory by default.
        WeaponManager.instance.AddWeapon(weaponsList[0]);
    }

    public void InstantiateDefaultWeapon(int index, GameObject player)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && index >= 0 && index < weaponsList.Length)
        {
            Vector3 weaponOffset = new Vector3(0, 0.5f, 0);

            if (currentWeapon != null)
            {
                // Destroy the current weapon if it exists.
                Destroy(currentWeapon);
            }

            // Instantiate the weapon at the player's position with an offset
            GameObject weaponInstance = Instantiate(weaponsList[index], player.transform.position - weaponOffset, player.transform.rotation);

            // Set the current weapon and parent it to the player
            currentWeapon = weaponInstance;
            currentWeapon.transform.SetParent(player.transform);
            weaponId = "1";
        }
    }

    public void Shoot()
    {
        Debug.Log("Shooting!");
        if (canShoot && !isReloading)
        {
            // Instantiate a bullet at the weapon's position and rotation.
            GameObject bulletInstance = Instantiate(bulletPrefab, currentWeapon.transform.position, player.transform.rotation);

            // Apply force to the bullet in the forward facing position.
            Vector2 shootDirection = (currentWeapon.transform.up * -0.125f).normalized;
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

    public void UpgradeDamage(int additionalDamage)
    {
        damage += additionalDamage;
    }

    public void UpgradeReloadSpeed(float reloadSpeedMultiplier)
    {
        reloadSpeed = Mathf.Max(1, (int)(reloadSpeed * reloadSpeedMultiplier));
    }

    public void UpgradeFireSpeed(float fireSpeedMultiplier)
    {
        fireSpeed *= fireSpeedMultiplier;
    }

    public void ApplyUpgrades(WeaponData weaponData)
    {
        if (weaponData != null)
        {
           damage = weaponData.damage;
           reloadSpeed = weaponData.reloadSpeed;
           fireSpeed = weaponData.fireSpeed;
        }
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