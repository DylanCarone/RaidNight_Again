using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CooldownManager : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private PlayerCombatEntity player;

    [Header("Spells")] 
    [SerializeField] private SpellUI squareSpell;
    [SerializeField] private SpellUI triangleSpell;
    [SerializeField] private SpellUI circleSpell;
    [SerializeField] private SpellUI xSpell;

    private SpellUI[] spells;
    private AbilityInstance[] spellInstances;

    private bool isInitialized = false;

    public void Initialize(PlayerCombatEntity player)
    {
        this.player = player;

        spells = new[] { squareSpell, triangleSpell, circleSpell, xSpell };
        spellInstances = new[] { player.SquareSpell, player.TriangleSpell, player.CircleSpell, player.XSpell };
        for (int i = 0; i < spells.Length; i++)
        {
            spells[i].Initialize(spellInstances[i], spellInstances);
        }

        player.SquareSpell.OnEmpowermentsChanged += (e) => squareSpell.UpdateEmpowermentUI(e);
        player.TriangleSpell.OnEmpowermentsChanged += (e) => triangleSpell.UpdateEmpowermentUI(e);
        player.CircleSpell.OnEmpowermentsChanged += (e) => circleSpell.UpdateEmpowermentUI(e);
        player.XSpell.OnEmpowermentsChanged += (e) => xSpell.UpdateEmpowermentUI(e);
        isInitialized = true;

    }



    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;

        for (int i = 0; i < spells.Length; i++)
        {
            spells[i].UpdateCooldown(spellInstances[i]);
            spells[i].UpdateOutOfRange(player);
            spells[i].UpdateOutOfMana(player);
        }

        UpdateGlobalCooldown();
    }

    private void UpdateGlobalCooldown()
    {
        float gcdPercent = player.GlobalCooldownTimer / player.GlobalCooldownDuration;
        foreach (var spell in spells)
        {
            if (spell.globalCooldownOverlay != null && spell.globalCooldownOverlay.enabled)
                spell.globalCooldownOverlay.fillAmount = gcdPercent;
        }
    }
}


[Serializable]
    public class SpellUI
    {
        [Header("References")] public Image icon;

        public Image cooldownOverlay;
        public Image globalCooldownOverlay;
        public Image outOfRangeOverlay;
        public Image outOfManaOverlay;
        public TextMeshProUGUI stackText;
        public Image empowermentIndicator;

        [HideInInspector] public int maxStacks;

        private AbilityInstance currentSpell;

        public void Initialize(AbilityInstance spell, AbilityInstance[] allSpells)
        {
            currentSpell = spell;
            icon.sprite = spell.ability.icon;
            maxStacks = 0;
            foreach (var s in allSpells)
            {
                if (s.ability is IStackingAbility stackAbility && stackAbility.AbilityToBuff == spell.ability)
                {
                    maxStacks = stackAbility.MaxStacks;
                    break;
                }
            }
            
            UpdateEmpowermentUI(new List<AbilityEmpowerment>());


        }

        public void UpdateCooldown(AbilityInstance spell)
        {
            cooldownOverlay.fillAmount = spell.GetCooldownPercent();
        }

        public void UpdateOutOfRange(PlayerCombatEntity player)
        {
            if (outOfRangeOverlay != null)
                outOfRangeOverlay.enabled = player.TargetRange > currentSpell.ability.range;
        }



        public void UpdateOutOfMana(PlayerCombatEntity player)
        {
            if (outOfManaOverlay != null)
                outOfManaOverlay.enabled = player.CurrentResource < currentSpell.GetModifiedResourceCost();
        }

        public void UpdateEmpowermentUI(List<AbilityEmpowerment> empowerments)
        {
            if (maxStacks <= 0)
            {
                empowermentIndicator.gameObject.SetActive(false);
                if (stackText != null) stackText.gameObject.SetActive(false);
                return;
            }

            int currentStacks = empowerments.Count > 0 ? Enumerable.Count(empowerments,e => e == empowerments[0]) : 0;
            bool hasStacks = currentStacks > 0;
            bool isMaxStacks = currentStacks >= maxStacks;

            if (stackText != null)
            {
                stackText.gameObject.SetActive(hasStacks);
                stackText.text = currentStacks.ToString();
                stackText.color = isMaxStacks ? Color.red : Color.white;
            }

            empowermentIndicator.gameObject.SetActive(isMaxStacks);
        }
    }

