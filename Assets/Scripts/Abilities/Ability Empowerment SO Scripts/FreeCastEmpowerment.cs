using UnityEngine;

// Free cast
[CreateAssetMenu(menuName = "Combat/Empowerments/Free Cast")]
public class FreeCastEmpowerment : AbilityEmpowerment
{
    public override float ModifyResourceCost(float originalCost) => 0f;
}