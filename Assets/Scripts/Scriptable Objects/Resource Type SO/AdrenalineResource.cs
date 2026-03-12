using UnityEngine;

[CreateAssetMenu(fileName = "Adrenaline", menuName = "Game/Resource Type/Adrenaline")]
public class AdrenalineResource : ResourceType
{
    [Range(0, 1)] public float frenzyThreshold = 0.6f;
    public float frenzyGCD = 1.5f;
    public override Color ResourceColor => MyPalette.EnergyYellow;

    public override bool IsThresholdActive(float current, float max)
    {
        // at 60% of max, player should enter a state of frenzy,
        // which reduces the GCD to 1.5s and increase auto attack speed
        return current >= max * frenzyThreshold;
    }

    public override float GetEffectiveGCD(float baseGCD, bool thresholdMet)
    {
        if (thresholdMet)
            return frenzyGCD;
        return baseGCD;

    }
}