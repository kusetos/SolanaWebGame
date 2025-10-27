using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static Health Instance { get; private set; }

    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Settings")]
    public Slider healthSlider; // optional (assign in Inspector)
    // public Image healthFillImage; // optional for color change
    // public Gradient healthGradient; // optional gradient color (green→red)

    private void Awake()
    {
                if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

    }
    bool isDead = false;

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        SoundManager.Instance.Play("damage");

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentHealth -= amount;

        UpdateHealthUI();
        //Debug.Log(currentHealth);
        if (currentHealth <= 1)
        {
            Die();
            isDead = true;
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider)
        {
            healthSlider.value = currentHealth / maxHealth;

            // if (healthFillImage && healthGradient.colorKeys.Length > 0)
            //     healthFillImage.color = healthGradient.Evaluate(healthSlider.value);
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        LevelManager.Instance.RestartLevel();
    }

}
