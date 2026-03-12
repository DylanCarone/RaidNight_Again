using UnityEngine;

[CreateAssetMenu(fileName = "Mana", menuName = "Game/Resource Type/Mana")]
public class ManaResource : ResourceType
{
    
    public override bool StartFull => true;
    public override bool AutoRegenerate => true;
    public override Color ResourceColor => MyPalette.ManaBlue;
}
