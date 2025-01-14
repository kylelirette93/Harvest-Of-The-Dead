using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Player : Actor
{
    public static Player instance;
    public TextMeshProUGUI currencyText;
    int currency;
    public TextMeshProUGUI bankedCurrencyText;
    public TextMeshProUGUI dayText;

    // References.
    Rigidbody2D rb;
    public Image healthBarFill;
    public static GameObject healthBarInstance;
    public GameObject crosshairPrefab; // Reference to the crosshair prefab
    private GameObject crosshairInstance; // Instance of the crosshair

    // Variables.
    public float moveSpeed = 5f;
    float timeSinceLastShot = 0;
    float fireCooldown;
    Vector2 moveDirection;
    Vector2 mousePosition;
    int currentHealth;
    int mapMinX = -15;
    int mapMaxX = 15;
    int mapMinY = -7;
    int mapMaxY = 7;

    // New variables for crosshair scaling
    Vector3 normalScale = Vector3.one;
    Vector3 enlargedScale = new Vector3(1.5f, 1.5f, 1.5f);
    float scaleSpeed = 10f;
    bool isShooting = false;

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

    public WeaponData savedWeaponData;

    private void OnDisable()
    {
        // Save the current weapon data when the player is disabled
        if (Weapon.instance != null)
        {
            savedWeaponData = Weapon.instance.GetCurrentWeaponData();
        }

        // Destroy the crosshair instance when the player is disabled
        if (crosshairInstance != null)
        {
            Destroy(crosshairInstance);
        }
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        if (healthSystem != null)
        {
            healthSystem.Heal(100);
        }
        if (savedWeaponData != null && Weapon.instance != null)
        {
            Weapon.instance.SetWeaponData(savedWeaponData);
        }

        if (dayText != null && GameManager.instance != null)
        {
            dayText.text = "Day: " + GameManager.instance.CurrentDay;
        }

        // Instantiate the crosshair prefab and make it a child of the canvas
        Canvas healthCanvas = FindObjectOfType<Canvas>();
        if (crosshairPrefab != null && healthCanvas != null)
        {
            crosshairInstance = Instantiate(crosshairPrefab, healthCanvas.transform);
            crosshairInstance.transform.localScale = normalScale;
        }
    }

    void OnDestroy()
    {
        //Debug.Log("Player instance destroyed.");
        //Debug.Log($"Stack trace: {System.Environment.StackTrace}");
    }

    public override void Start()
    {
        base.Start();

        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        Transform healthBarTransform = healthBarInstance.transform;
        currencyText = GameObject.Find("currencyText").GetComponent<TextMeshProUGUI>();
        bankedCurrencyText = GameObject.Find("bankedCurrencyText").GetComponent<TextMeshProUGUI>();
        dayText = GameObject.Find("dayText").GetComponent<TextMeshProUGUI>();
        dayText.text = "Day: " + GameManager.instance.CurrentDay;
        currency = CurrencySystem.GetCurrency();
        UpdateUI(currency);

        healthBarFill = healthBarInstance.transform.Find("Fill").GetComponent<Image>();

        Canvas healthCanvas = FindObjectOfType<Canvas>();
        if (healthCanvas != null)
        {
            healthBarTransform.SetParent(healthCanvas.transform, false);
        }
        healthBarTransform.localScale = normalScale;

        fireCooldown = Weapon.instance.fireSpeed;
        currentHealth = healthSystem.currentHealth;
    }

    public override void Update()
    {
        base.Update();

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        healthBarFill.transform.position = screenPosition;

        healthBarFill.fillAmount = healthSystem.currentHealth / (float)healthSystem.maxHealth;

        if (healthSystem.currentHealth <= healthSystem.maxHealth * 0.25f)
        {
            // Health is 25% or lower
            healthBarFill.color = Color.red;
        }
        else if (healthSystem.currentHealth <= healthSystem.maxHealth * 0.5f)
        {
            // Health is 50% or lower
            healthBarFill.color = Color.yellow;
        }
        else
        {
            // Health is above 50%
            healthBarFill.color = Color.green;
        }

        timeSinceLastShot += Time.deltaTime;
        // Get player input.
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Fire weapon on mouse click.
        if (Input.GetMouseButton(0) && timeSinceLastShot >= fireCooldown)
        {
            Weapon.instance.Shoot();
            timeSinceLastShot = 0f;
            isShooting = true;
        }

        // Gradually scale the crosshair
        if (isShooting)
        {
            crosshairInstance.transform.localScale = Vector3.Lerp(crosshairInstance.transform.localScale, enlargedScale, Time.deltaTime * scaleSpeed);
            if (Vector3.Distance(crosshairInstance.transform.localScale, enlargedScale) < 0.01f)
            {
                isShooting = false;
            }
        }
        else
        {
            crosshairInstance.transform.localScale = Vector3.Lerp(crosshairInstance.transform.localScale, normalScale, Time.deltaTime * scaleSpeed);
        }

        // Direction is based on input.
        moveDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

        // Get current mouse position and convert to world space.
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 currentPosition = transform.position;
        currentPosition.x = Mathf.Clamp(currentPosition.x, mapMinX, mapMaxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, mapMinY, mapMaxY);

        // Check for pause button press.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.Pause();
        }

        // Update crosshair position
        if (crosshairInstance != null)
        {
            crosshairInstance.transform.position = Input.mousePosition;
        }
    }

    void FixedUpdate()
    {
        // Move the player using direction.
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        // Aim direction is from the mouse's position to the player.
        Vector2 aimDirection = mousePosition - rb.position;

        // Calculate angle between player and mouse position.
        float aimAngle = Mathf.Atan2(aimDirection.x, -aimDirection.y) * Mathf.Rad2Deg;

        // Apply angle to rotate rigidbody.
        rb.rotation = aimAngle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            healthSystem.TakeDamage(10);
        }
    }

    public void SpawnHealthBar()
    {
        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        Transform healthBarTransform = healthBarInstance.transform;
        currencyText = GameObject.Find("currencyText").GetComponent<TextMeshProUGUI>();

        healthBarFill = healthBarInstance.transform.Find("Fill").GetComponent<Image>();

        Canvas healthCanvas = FindObjectOfType<Canvas>();
        if (healthCanvas != null)
        {
            healthBarTransform.SetParent(healthCanvas.transform, false);
        }
        healthBarTransform.localScale = normalScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Home"))
        {
            GameManager.instance.ChangeState(GameManager.GameState.Retreat);
        }

        if (other.gameObject.CompareTag("Money"))
        {
            CurrencySystem.AddCurrency();
            currency = CurrencySystem.GetCurrency();
            UpdateUI(currency);
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Health"))
        {
            healthSystem.Heal(10);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Home"))
        {
            GameManager.instance.ChangeState(GameManager.GameState.Retreat);
        }
    }

    public void UpdateUI(int currency)
    {
        currencyText.text = "Cash: $" + currency;
        bankedCurrencyText.text = "Bank: $" + CurrencySystem.bankedCurrency;
        dayText.text = "Day: " + GameManager.instance.CurrentDay;
    }


    public override void Die()
    {
        healthSystem.Heal(100);
        GameManager.instance.ChangeState(GameManager.GameState.Death);
    }

    public void DisableCrosshair()
    {
        if (crosshairInstance != null)
        {
            crosshairInstance.SetActive(false);
        }
    }

    public void EnableCrosshair()
    {
        if (crosshairInstance != null)
        {
            crosshairInstance.SetActive(true);
        }
    }
}