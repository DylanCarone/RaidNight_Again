using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DPSCalculator : MonoBehaviour
{
    [SerializeField] private CombatEntity boss;
    [SerializeField] private TextMeshProUGUI dpsText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float currentBossHealth;
    
    private float startingHealth;
    private float startTime;
    private bool isTracking = false;

    private float currentDPS;
    private float finalDPS;
    private float gametimer = 0f;
    
    
    struct DamageEntry
    {
        public float amount;
        public float time;
    }

    private Queue<DamageEntry> damageWindow = new Queue<DamageEntry>();
    private float windowDuration = 3.0f;
    
    private void Start()
    {
        currentBossHealth = boss.MaxHealth;
        boss.OnHealthChanged += UpdateBossLife;
        dpsText.text = $"{0:F2}";
        timerText.text= $"{gametimer}";
    }

    private void Update()
    {
        if (currentBossHealth > 0)
        {
            gametimer += Time.deltaTime;
        }

        timerText.text= $"{gametimer:F1}";
        if (currentBossHealth <= 0)
        {
            finalDPS = currentDPS;
            dpsText.text = $"{finalDPS:F0}";
            return;
        }
        
        if(!isTracking && currentBossHealth < boss.MaxHealth)
            StartTracking();

        if (isTracking)
        {
            currentDPS = CalculateDPS();
            dpsText.text = $"{currentDPS:F0}";
        }


    }



    void UpdateBossLife(float current, float max)
    {
        currentBossHealth = current;
    }

    void StartTracking()
    {
        isTracking = true;
        startTime = Time.time;
        startingHealth = boss.MaxHealth;
    }

    float CalculateDPS()
    {
        float timeElapsed = Time.time - startTime;

        if (timeElapsed <= 0) return 0;

        float totalDamageDealt = startingHealth - currentBossHealth;
        return totalDamageDealt / timeElapsed;
    }
}
