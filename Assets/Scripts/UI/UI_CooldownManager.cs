using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownManager : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private PlayerCombatEntity player;
    [Header("Spell Icons")]
    [SerializeField] private Image squareIcon;
    [SerializeField] private Image triangleIcon;
    [SerializeField] private Image circleIcon;
    [SerializeField] private Image xIcon;
    
    
    [Header("Cooldown Images")]
    [SerializeField] private Image squareCooldown;
    [SerializeField] private Image triangleCooldown;
    [SerializeField] private Image circleCooldown;
    [SerializeField] private Image xCooldown;

    [Header("Global Cooldown Images")]
    [SerializeField] private Image[] globalCooldownImages;
    
    [Header("Out of Range Images")]
    [SerializeField] private Image squareOutOfRangeImage;
    [SerializeField] private Image triOutOfRangeImage;
    [SerializeField] private Image circleOutOfRangeImage;
    [SerializeField] private Image xOutOfRangeImage;

    [Header("Empowerment Indicators")]
    [SerializeField] private Image squareEmpowermentIndicator;
    [SerializeField] private Image triangleEmpowermentIndicator;
    [SerializeField] private Image circleEmpowermentIndicator;
    [SerializeField] private Image xEmpowermentIndicator;

    private bool isInitialized = false;

    public void Initialize(PlayerCombatEntity player)
    {
        this.player = player;
        squareIcon.sprite = player.SquareSpell.ability.icon;
        triangleIcon.sprite = player.TriangleSpell.ability.icon;
        circleIcon.sprite = player.CircleSpell.ability.icon;
        xIcon.sprite = player.XSpell.ability.icon;
        DisableGCDs();
        player.SquareSpell.OnEmpowermentsChanged += (e) => UpdateEmpowermentUI(squareEmpowermentIndicator, e);
        player.TriangleSpell.OnEmpowermentsChanged += (e) => UpdateEmpowermentUI(triangleEmpowermentIndicator, e);
        player.CircleSpell.OnEmpowermentsChanged += (e) => UpdateEmpowermentUI(circleEmpowermentIndicator, e);
        player.XSpell.OnEmpowermentsChanged += (e) => UpdateEmpowermentUI(xEmpowermentIndicator, e);
        isInitialized = true;
        
        
    }

    private void UpdateEmpowermentUI(Image glow, List<AbilityEmpowerment> empowerments)
    {
        bool hasEmpowerments = empowerments.Count > 0;
        glow.gameObject.SetActive(hasEmpowerments);
    }

    void DisableGCDs()
    {
        if(!player.SquareSpell.ability.isOnGDC)
            globalCooldownImages[0].enabled = false;
        if(!player.TriangleSpell.ability.isOnGDC)
            globalCooldownImages[1].enabled = false;
        if(!player.CircleSpell.ability.isOnGDC)
            globalCooldownImages[2].enabled = false;
        if(!player.XSpell.ability.isOnGDC)
            globalCooldownImages[3].enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;
        
        squareCooldown.fillAmount = player.SquareSpell.GetCooldownPercent();
        triangleCooldown.fillAmount = player.TriangleSpell.GetCooldownPercent();
        circleCooldown.fillAmount = player.CircleSpell.GetCooldownPercent();
        xCooldown.fillAmount = player.XSpell.GetCooldownPercent();

        foreach (var gcd in globalCooldownImages)
        {
            gcd.fillAmount = player.GlobalCooldownTimer / player.GlobalCooldownDuration; 
        }

        CalculateOutOfRange();
        CalculateOutOfMana();
    }

    private void CalculateOutOfRange()
    {
        squareOutOfRangeImage.enabled = player.TargetRange > player.SquareSpell.ability.range;
        triOutOfRangeImage.enabled = player.TargetRange > player.TriangleSpell.ability.range;
        circleOutOfRangeImage.enabled = player.TargetRange > player.CircleSpell.ability.range;
        xOutOfRangeImage.enabled = player.TargetRange > player.XSpell.ability.range;
    }
    private void CalculateOutOfMana()
    {
        squareOutOfRangeImage.enabled = player.CurrentResource < player.SquareSpell.ability.resourceCost;
        triOutOfRangeImage.enabled = player.CurrentResource < player.TriangleSpell.ability.resourceCost;
        circleOutOfRangeImage.enabled = player.CurrentResource < player.CircleSpell.ability.resourceCost;
        xOutOfRangeImage.enabled = player.CurrentResource < player.XSpell.ability.resourceCost;
    }
}
