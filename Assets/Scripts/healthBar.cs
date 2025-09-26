using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easehealthSlider;
    private float lerpSpeed = 0.05f;

    [SerializeField]
    private HealthScript targetHealth; // baðlanacak olan HealthScript

    void Start()
    {
        if (targetHealth != null)
        {
            healthSlider.maxValue = targetHealth.health;
            easehealthSlider.maxValue = targetHealth.health;
        }
    }

    void Update()
    {
        if (targetHealth == null) return;

        // gerçek health deðerini al
        float currentHealth = targetHealth.health;

        // anlýk bar (direkt)
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        // yumuþak geçiþli bar (ease)
        if (easehealthSlider.value != currentHealth)
        {
            easehealthSlider.value = Mathf.Lerp(easehealthSlider.value, currentHealth, lerpSpeed);
        }
    }
}
