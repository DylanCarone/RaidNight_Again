using System;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

    [SerializeField] private CombatEntity entity;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Color32 shieldColor;
    private Slider healthBar;
    [SerializeField] private Slider shieldSlider;

    
    Color32 barColor;
    

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    private void Start()
    {
        //Debug.Log(entity.CurrentHealth);
        healthBar.maxValue = entity.MaxHealth;
        healthBar.value = entity.CurrentHealth;
        entity.OnHealthChanged += UpdateHealth;
        //barColor = healthBarImage.color;
        

    }

    private void Update()
    {
        UpdateShieldBar();
    }
    private void UpdateShieldBar()
    {
        if (shieldSlider == null) return;
    
        float totalShield = GetTotalShield();
        bool hasShield = totalShield > 0;
    
        // Update shield bar visibility
        if (hasShield)
        {
            if (!shieldSlider.gameObject.activeSelf)
                shieldSlider.gameObject.SetActive(true);
            
            float combinedValue = (entity.CurrentHealth + totalShield) / entity.MaxHealth;
            //Debug.Log(combinedValue);
            shieldSlider.value = combinedValue;
        }
        else
        {
            if (shieldSlider.gameObject.activeSelf)
                shieldSlider.gameObject.SetActive(false);
        }
    
    }
    private float GetTotalShield()
    {
        if (entity.StatusEffectManager == null)
            return 0f;
        
        float total = 0f;
        
        var shields = entity.StatusEffectManager.GetEffectsOfType<BuffEffect>()
            .FindAll(b => b.BuffType == BuffType.Shield);
        
        foreach (var shield in shields)
        {
            total += shield.CurrentShieldAmount;
        }
        
        return total;
    }


    private void OnDisable()
    {
        entity.OnHealthChanged -= UpdateHealth;
    }


    private void UpdateHealth(float current, float max)
    {
        healthBar.maxValue = max;
        healthBar.value = current;
    }

    private void ShieldUpdate(bool isShield)
    {
        if (isShield)
        {
            healthBarImage.color = shieldColor;
        }
        else
        {
            healthBarImage.color = barColor;
        }

        
    }


}
