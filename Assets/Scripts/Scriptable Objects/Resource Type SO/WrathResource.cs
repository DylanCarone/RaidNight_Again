using UnityEngine;

[CreateAssetMenu(fileName = "Wrath", menuName = "Game/Resource Type/Wrath")]
public class WrathResource : ResourceType
{
    public override bool StartFull => false;
    public override bool AutoRegenerate => false;
    public override bool RestoreOnAutoAttack  => true;
    public override bool RestoreResourceOnHit => true;
    public override Color ResourceColor => MyPalette.Wrath;
}