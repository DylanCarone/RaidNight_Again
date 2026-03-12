using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Empowerments/Cast Reduction")]
public class CastReductionEmpowerment : AbilityEmpowerment
{
    public float flatReduction = 0f;
    public override float ModifyCastTime(float originalCastTime) => originalCastTime - flatReduction;
}
