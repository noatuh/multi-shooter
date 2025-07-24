using UnityEngine;
using TMPro;
using Mirror;

public class HealthDisplay : NetworkBehaviour
{
    private TextMeshProUGUI healthText;
    private PlayerHealth playerHealth;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        playerHealth = GetComponent<PlayerHealth>();
        
        GameObject healthTextObject = GameObject.FindGameObjectWithTag("HealthText");
        if (healthTextObject != null)
        {
            healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
            UpdateHealthDisplay(); // Immediately update the health display
        }
        else
        {
            Debug.LogError("HealthText object not found in scene. Make sure to tag your health text UI element with 'HealthText'");
        }
    }

    void Update()
    {
        if (isLocalPlayer && playerHealth != null && healthText != null)
        {
            UpdateHealthDisplay();
        }
    }

    void UpdateHealthDisplay()
    {
        int currentHealth = playerHealth.GetCurrentHealth();
        if (currentHealth >= 100)
        {
            healthText.text = "Health: Full";
        }
        else
        {
            healthText.text = $"Health: {currentHealth}%";
        }
    }
}
