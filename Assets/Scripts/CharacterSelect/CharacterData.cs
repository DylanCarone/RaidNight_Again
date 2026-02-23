using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public GameObject playerModel;
    public PlayerRole role;
    public string className;
    public Ability[] abilities;
    
}
