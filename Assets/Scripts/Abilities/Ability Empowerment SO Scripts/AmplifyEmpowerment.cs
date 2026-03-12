using UnityEngine;

// Damage amplifier
[CreateAssetMenu(menuName = "Combat/Empowerments/Amplify")]
public class AmplifyEmpowerment : AbilityEmpowerment
{
    [Tooltip("Flat damage to apply to the target. Multiplier is applied after this.")]
    public float flatDamageIncrease = 50f;
    [Tooltip("Multiplier to apply to the damage.")]
    public float multiplier = 1f;
    public override float ModifyDamage(float originalDamage) => (originalDamage + flatDamageIncrease) * multiplier;
}