using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownManager : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private PlayerCombatEntity player;
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

    [SerializeField] private Image[] globalCooldownImages;
    [SerializeField] private Image squareOutOfRangeImage;
    [SerializeField] private Image triOutOfRangeImage;
    [SerializeField] private Image circleOutOfRangeImage;
    [SerializeField] private Image xOutOfRangeImage;


    private void Start()
    {
        DisableGCDs();
        squareIcon.sprite = player.SquareSpell.ability.icon;
        triangleIcon.sprite = player.TriangleSpell.ability.icon;
        circleIcon.sprite = player.CircleSpell.ability.icon;
        xIcon.sprite = player.XSpell.ability.icon;
        
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
        squareCooldown.fillAmount = player.SquareSpell.GetCooldownPercent();
        triangleCooldown.fillAmount = player.TriangleSpell.GetCooldownPercent();
        circleCooldown.fillAmount = player.CircleSpell.GetCooldownPercent();
        xCooldown.fillAmount = player.XSpell.GetCooldownPercent();

        foreach (var gcd in globalCooldownImages)
        {
            gcd.fillAmount = player.GlobalCooldownTimer / player.GlobalCooldownDuration; 
        }

        squareOutOfRangeImage.enabled = player.TargetRange > player.SquareSpell.ability.range;
        triOutOfRangeImage.enabled = player.TargetRange > player.TriangleSpell.ability.range;
        circleOutOfRangeImage.enabled = player.TargetRange > player.CircleSpell.ability.range;
        xOutOfRangeImage.enabled = player.TargetRange > player.XSpell.ability.range;
    }
}
