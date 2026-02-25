using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Settings")]
    public GameObject playerModel;
    public PlayerRole role;
    public string className;
    [Header("Combat")]
    public float maxHealth;
    public float attackSpeed;
    public float attackDamage;
    public float attackRange;
    public Ability[] abilities;
    [Header("Resources")]
    public ResourceType resourceType;
    public float maxResource;
    public float regenPerSecond;
    public bool startFull = true;
    public bool autoRegenerate = true;
    public bool restoreOnAutoAttack = false;
    public bool restoreResourceOnHit = false;
}


public enum ResourceType
{
    Mana,
    Rage,
    Energy
}