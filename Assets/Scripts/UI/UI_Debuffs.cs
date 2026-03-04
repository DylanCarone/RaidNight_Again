using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Debuffs : MonoBehaviour
{
    [SerializeField] CombatEntity entity;
    [SerializeField] private GameObject effectIconPrefab;
    [SerializeField] private Transform iconContainer;

    private StatusEffectManager manager;
    private Dictionary<StatusEffect, GameObject> activeIcons = new Dictionary<StatusEffect, GameObject>(); 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = entity.StatusEffectManager;
        manager.OnEffectApplied += AddIcon;
        manager.OnEffectRemoved += RemoveIcon;
    }

    private void OnDisable()
    {
        manager.OnEffectApplied -= AddIcon;
        manager.OnEffectRemoved -= RemoveIcon;
    }


    private void AddIcon(StatusEffect effect)
    {
        var icon = Instantiate(effectIconPrefab, iconContainer);
        icon.GetComponent<UI_EffectIcon>().Initialize(effect);
        activeIcons.Add(effect, icon);
    }
    
    private void RemoveIcon(StatusEffect effect)
    {
        if (activeIcons.TryGetValue(effect, out var icon))
        {
            Destroy(icon);
            activeIcons.Remove(effect);
        }
    }
    
    
}
