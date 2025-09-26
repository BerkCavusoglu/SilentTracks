using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easehealthSlider;
    private float lerpSpeed = 0.05f;

    [SerializeField]
    private HealthScript targetHealth; // ba�lanacak olan HealthScript

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

        // ger�ek health de�erini al
        float currentHealth = targetHealth.health;

        // anl�k bar (direkt)
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        // yumu�ak ge�i�li bar (ease)
        if (easehealthSlider.value != currentHealth)
        {
            easehealthSlider.value = Mathf.Lerp(easehealthSlider.value, currentHealth, lerpSpeed);
        }
    }
}
