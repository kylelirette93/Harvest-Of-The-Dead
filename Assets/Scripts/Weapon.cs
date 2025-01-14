using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Threading;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;

    int capacity;
    int maxCapacity;
    public float reloadSpeed;
    public float fireSpeed;
    public float damage;
    private float damageUpgrade = 0;
    private float reloadSpeedUpgrade = 0;
    private float fireSpeedUpgrade = 0;
    int bulletSpeed = 50;
    public bool canShoot = true;
    bool isReloading = false;

    public GameObject player;
    public GameObject[] weaponsList;
    public GameObject currentWeapon;
    public GameObject bulletPrefab;

    GameObject muzzleFlashInstance;
    public GameObject muzzleFlashPrefab;
    Transform muzzleFlashPoint;
    string muzzleFlashTag = "MuzzleFlashPoint";

    private Vector3[] weaponOffsets;
    private Quaternion[] localRotations;

    private bool isShotgunPurchased = false;

    private WeaponData currentWeaponData;

    public Dictionary<WeaponType, int> weaponAmmo = new Dictionary<WeaponType, int>();
    public Dictionary<WeaponType, int> maxWeaponAmmo = new Dictionary<WeaponType, int>();

    public TextMeshProUGUI reloadingText;
    public Slider reloadProgressBar;

    public enum WeaponType
    {
        Pistol,
        Shotgun
    }

    public WeaponType weaponType;

    AudioSource shootSound;
    AudioSource reloadSound;

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
        weaponOffsets = new Vector3[weaponsList.Length];
        weaponOffsets[0] = new Vector3(0, -0.5f, 0); // Pistol offset
        weaponOffsets[1] = new Vector3(0, -0.5f, 0); // Shotgun offset

        localRotations = new Quaternion[weaponsList.Length];
        localRotations[0] = Quaternion.Euler(0, 0, 0); // Pistol rotation
        localRotations[1] = Quaternion.Euler(0, 0, -90); // Shotgun rotation

        weaponAmmo[WeaponType.Pistol] = 10; // Initial ammo for Pistol
        maxWeaponAmmo[WeaponType.Pistol] = 10; // Max ammo for Pistol

        weaponAmmo[WeaponType.Shotgun] = 6; // Initial ammo for Shotgun
        maxWeaponAmmo[WeaponType.Shotgun] = 6; // Max ammo for Shotgun

        WeaponManager.instance.AddWeapon(weaponsList[0]);
    }

    public WeaponData GetCurrentWeaponData()
    {
        return currentWeaponData;
    }

    public void SetCurrentWeaponData(WeaponData weaponData)
    {
        currentWeaponData = weaponData;
        ApplyUpgrades(weaponData);
    }

    public void InstantiateWeapon(int index)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && index >= 0 && index < weaponsList.Length)
        {
            Vector3 weaponOffset = weaponOffsets[index];
            Quaternion localRotation = localRotations[index];

            if (currentWeapon != null)
            {
                // Destroy the current weapon if it exists.
                Destroy(currentWeapon);
            }

            // Instantiate the selected weapon at the player's position with an offset
            GameObject weaponInstance = Instantiate(weaponsList[index], player.transform.position, player.transform.rotation);

            // Set the current weapon and parent it to the player
            currentWeapon = weaponInstance;
            currentWeapon.transform.SetParent(player.transform);

            currentWeapon.transform.localPosition = weaponOffset;
            currentWeapon.transform.localRotation = localRotation;

            if (index == 0)
            {
                weaponType = WeaponType.Pistol;
            }
            else if (index == 1)
            {
                weaponType = WeaponType.Shotgun;
            }

            shootSound = currentWeapon.transform.Find("ShootSound").GetComponent<AudioSource>();
            reloadSound = currentWeapon.transform.Find("ReloadSound").GetComponent<AudioSource>();

            // Assign current weapon's muzzle flash point to the muzzle flash point variable.
            muzzleFlashPoint = currentWeapon.transform.Find(muzzleFlashTag);

            // Instantiate muzzle flash prefab at the weapon's muzzle flash point.
            muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.transform.position, muzzleFlashPoint.transform.rotation);
            muzzleFlashInstance.transform.SetParent(currentWeapon.transform);

            // Deactivate the muzzle flash instance initially
            muzzleFlashInstance.SetActive(false);

            WeaponData weaponData = WeaponManager.instance.GetWeaponDataById(weaponsList[index].name);
            if (weaponData != null)
            {
                SetWeaponData(weaponData);
            }
            else
            {
                //Debug.LogWarning("Weapon data not found for: " + weaponsList[index].name);
            }
        }
    }

    public void ResetWeaponData()
    {
        if (currentWeapon != null)
        {
            WeaponData weaponData = WeaponManager.instance.GetWeaponDataById(currentWeapon.name);
            if (weaponData != null)
            {
                //Debug.Log("Resetting weapon data for: " + currentWeapon.name);
                ApplyUpgrades(weaponData);
            }
            else
            {
                //Debug.LogWarning("Weapon data not found for: " + currentWeapon.name);
            }
        }
    }

    public void SetWeaponData(WeaponData weaponData)
    {
        currentWeaponData = weaponData;
        this.damage = weaponData.damage;
        this.reloadSpeed = weaponData.reloadSpeed;
        this.fireSpeed = weaponData.fireSpeed;
        //Debug.Log($"Weapon data set to: {weaponData.weaponName}, fireSpeed: {fireSpeed}");
    }

    public void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex == 1 && !isShotgunPurchased) return;

        if (weaponIndex >= 0 && weaponIndex < weaponsList.Length)
        {
            InstantiateWeapon(weaponIndex);
            weaponType = (WeaponType)weaponIndex; // Set the new weapon type

            // Log the fireSpeed value after switching weapons
            //Debug.Log($"Switched to weapon: {weaponType}, fireSpeed: {fireSpeed}");
        }
        else
        {
            //Debug.LogError("Weapon index out of range");
        }
    }


    public void Shoot()
    {
        if (canShoot && !isReloading)
        {
            if (weaponAmmo[weaponType] > 0)
            {
                shootSound.Play();

                Vector2[] shootDirections = GetShootDirections();
                float adjustedBulletSpeed = weaponType == WeaponType.Pistol ? bulletSpeed * 5f : bulletSpeed;

                foreach (Vector2 shootDirection in shootDirections)
                {
                    Vector3 bulletSpawnPosition = currentWeapon.transform.position + (Vector3)shootDirection.normalized * 1f;
                    GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPosition, player.transform.rotation);
                    bulletInstance.GetComponent<Rigidbody2D>().AddForce(shootDirection * adjustedBulletSpeed, ForceMode2D.Impulse);
                }

                muzzleFlashInstance.SetActive(true);
                //Debug.Log("Muzzle flash effect activated");

                StartCoroutine(DeactivateMuzzleFlash());

                weaponAmmo[weaponType]--; // Decrease ammo for the current weapon
                //Debug.Log("Starting FireDelay coroutine");
                StartCoroutine(FireDelay());

                if (weaponAmmo[weaponType] <= 0)
                {
                    canShoot = false; // Disable shooting when out of ammo
                    StartCoroutine(InitiateReload(0.2f)); // Start the reload process
                }
            }
            else
            {
                //Debug.Log("Out of ammo! Reload required.");
                StartCoroutine(InitiateReload(0.2f)); // Start the reload process if out of ammo
            }
        }
    }

    IEnumerator InitiateReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isReloading && weaponAmmo[weaponType] < maxWeaponAmmo[weaponType])
        {
            StartCoroutine(Reload(reloadSpeed));
        }
    }
    Vector2[] GetShootDirections()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            //Debug.Log("Shooting spread shot.");
            float angleStep = 10f; // Adjust this value to control the spread
            Vector2[] directions = new Vector2[5];

            for (int i = 0; i < directions.Length; i++)
            {
                // Adjust the angle for the spread shot
                float angle = currentWeapon.transform.eulerAngles.z + (angleStep * (i - 2)); // Offset angles for spread

                // Calculate the shoot direction
                directions[i] = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            }

            return directions;
        }
        else if (weaponType == WeaponType.Pistol)
        {
            return new Vector2[] { currentWeapon.transform.up * -0.125f };
        }
        else
        {
            //Debug.Log("No weapon found.");
            return new Vector2[0];
        }
    }
    private IEnumerator FireDelay()
    {
        canShoot = false;
        //Debug.Log("FireDelay started, waiting for " + fireSpeed + " seconds");
        yield return new WaitForSeconds(fireSpeed);
        canShoot = true;
       // Debug.Log("FireDelay ended, canShoot set to true");
    }

    private IEnumerator DeactivateMuzzleFlash()
    {
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        if (muzzleFlashInstance != null)
        {
            //Debug.Log("Deactivating muzzle flash");
            muzzleFlashInstance.SetActive(false);
        }
        else
        {
            //Debug.LogWarning("Muzzle flash instance is null");
        }
    }

    IEnumerator Reload(float delay)
    {
        isReloading = true;
        reloadingText.gameObject.SetActive(true);
        reloadProgressBar.gameObject.SetActive(true);
        reloadProgressBar.value = 0;

        // Access the TextMeshPro material
        Material reloadingTextMaterial = reloadingText.fontMaterial;

        // Define two colors for the ping-pong effect
        Color startColor = Color.grey;
        Color endColor = Color.red;

        if (reloadSound != null)
        {
            reloadSound.pitch = reloadSound.clip.length / delay; // Adjust the pitch based on the reload speed
            reloadSound.Play();
        }

        float elapsedTime = 0;
        while (elapsedTime < delay)
        {
            if (!isReloading)
            {
                reloadingText.gameObject.SetActive(false);
                reloadProgressBar.gameObject.SetActive(false);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            reloadProgressBar.value = Mathf.Clamp01(elapsedTime / delay);

            // Subtle ping-pong effect for the face dilate property
            float faceDilate = Mathf.PingPong(elapsedTime * 0.5f, 0.2f); // Adjust the speed and range for subtle effect
            reloadingTextMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, faceDilate);

            // Ping-pong effect for the color
            float t = Mathf.PingPong(elapsedTime, 0.5f);
            reloadingTextMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.Lerp(startColor, endColor, t));

            yield return null;
        }

        weaponAmmo[weaponType] = maxWeaponAmmo[weaponType]; // Reload only the current weapon's ammo
        isReloading = false;
        canShoot = true;

        reloadingText.gameObject.SetActive(false);
        reloadProgressBar.gameObject.SetActive(false);

        // Reset the face dilate property and color
        reloadingTextMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0);
        reloadingTextMaterial.SetColor(ShaderUtilities.ID_FaceColor, startColor);

        if (reloadSound != null)
        {
            reloadSound.pitch = 1; // Reset the pitch to normal after reloading
        }
    }

    public void StopReload()
    {
        isReloading = false;
    }

    public void UpgradeDamage(int additionalDamage)
    {
        WeaponData weaponData = WeaponManager.instance.GetWeaponDataById(currentWeapon.name);
        if (weaponData != null && damage < weaponData.maxDamage)
        {
            damageUpgrade = Mathf.Min(damageUpgrade + additionalDamage, weaponData.maxDamage - weaponData.damage);
            ApplyUpgrades(weaponData);
        }
        else
        {
            UpgradeManager.instance.upgradeDamageText.text = "Maxed Out!";
        }
    }

    public void UpgradeReloadSpeed()
    {
        WeaponData weaponData = WeaponManager.instance.GetWeaponDataById(currentWeapon.name);
        if (weaponData != null && reloadSpeed > weaponData.minReloadSpeed)
        {
            reloadSpeedUpgrade = Mathf.Min(reloadSpeedUpgrade + 0.2f, weaponData.reloadSpeed - weaponData.minReloadSpeed);
            ApplyUpgrades(weaponData);
            //Debug.Log("Upgraded reload speed: " + reloadSpeed);
        }
        else
        {
            UpgradeManager.instance.upgradeReloadSpeedText.text = "Maxed Out!";
        }
    }

    public void UpgradeFireSpeed(float fireSpeedIncrease)
    {
        WeaponData weaponData = WeaponManager.instance.GetWeaponDataById(currentWeapon.name);
        if (weaponData != null && fireSpeed > weaponData.minFireSpeed)
        {
            fireSpeedUpgrade = Mathf.Min(fireSpeedUpgrade + fireSpeedIncrease, weaponData.fireSpeed - weaponData.minFireSpeed);
            ApplyUpgrades(weaponData);
        }
        else
        {
            UpgradeManager.instance.upgradeFireSpeedText.text = "Maxed Out!";
        }
    }

    public void ApplyUpgrades(WeaponData weaponData)
    {
        if (weaponData != null)
        {
            damage = weaponData.damage + damageUpgrade;
            reloadSpeed = weaponData.reloadSpeed - reloadSpeedUpgrade;
            fireSpeed = weaponData.fireSpeed - fireSpeedUpgrade;
        }
    }
    

    public void PurchaseShotgun()
    {
        isShotgunPurchased = true;
    }
}