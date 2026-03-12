using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Empowerments/Cost Reduction Cast")]
public class CostReductionEmpowerment : AbilityEmpowerment
{
    [Tooltip("Percentage of the original cost to reduce to.")]
    [Range(0,100)] public float reductionPercentage = 0f;
    public override float ModifyResourceCost(float originalCost) => originalCost * (1f - reductionPercentage / 100f);
}
