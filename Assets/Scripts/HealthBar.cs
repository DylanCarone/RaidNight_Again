using System;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

    [SerializeField] private CombatEntity entity;
    private Slider healthBar;


    private void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    private void Start()
    {
        Debug.Log(entity.CurrentHealth);
        healthBar.maxValue = entity.MaxHealth;
        healthBar.value = entity.CurrentHealth;
        entity.OnHealthChanged += UpdateHealth;
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


}
