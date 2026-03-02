using UnityEngine;

[System.Serializable]
public class AbilityInstance
{
    public Ability ability;
    private float currentCooldown;

    private bool canCast;
    public bool CanCast => canCast;

    public AbilityInstance(Ability ability)
    {
        this.ability = ability;
        currentCooldown = 0f;
    }

    public bool IsReady()
    {
        return currentCooldown <= 0f;
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, currentCooldown);
    }

    public float GetCooldownPercent()
    {
        if (ability.cooldown <= 0) return 0f;
        
        return Mathf.Clamp01(currentCooldown / ability.cooldown);
    }

    public void StartCooldown()
    {
        currentCooldown = ability.cooldown;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
        }
    }

    public void ResetCooldown()
    {
        currentCooldown = 0f;
    }
}
