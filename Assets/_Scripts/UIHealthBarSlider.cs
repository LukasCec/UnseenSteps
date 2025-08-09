using UnityEngine;
using UnityEngine.UI;

public class UIHealthBarSlider : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Slider slider;
    public bool smooth = true;
    public float lerpTime = 0.15f;

    void Start()
    {
        if (playerHealth == null) return;
        slider.minValue = 0;
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.health;
    }

    void Update()
    {
        if (playerHealth == null) return;

        
        if ((int)slider.maxValue != playerHealth.maxHealth)
            slider.maxValue = playerHealth.maxHealth;

        if (smooth)
            slider.value = Mathf.Lerp(slider.value, playerHealth.health, Time.unscaledDeltaTime / Mathf.Max(0.0001f, lerpTime));
        else
            slider.value = playerHealth.health;
    }
}
