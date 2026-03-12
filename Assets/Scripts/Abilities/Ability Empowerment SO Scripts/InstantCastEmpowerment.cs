using UnityEngine;

// Instant cast
[CreateAssetMenu(menuName = "Combat/Empowerments/Instant Cast")]
public class InstantCastEmpowerment : AbilityEmpowerment
{
    public override float ModifyCastTime(float originalCastTime) => 0f;
}