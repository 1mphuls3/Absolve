using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerHealth health;
    private void Start()
    {
        fillImage.fillAmount = 1;
    }
    public void UpdateHealthBar()
    {
        fillImage.fillAmount = (float)health.currentHealth / (float)health.maxHealth;
    }
}
