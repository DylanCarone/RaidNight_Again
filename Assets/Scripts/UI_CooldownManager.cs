using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownManager : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private PlayerCombatEntity player;
    
    [Header("Images")]
    [SerializeField] private Image fireball;
    [SerializeField] private Image heal;
    [SerializeField] private Image kick;
    [SerializeField] private Image poke;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireball.fillAmount = player.FireballTimer/player.FireballCooldown;
        heal.fillAmount = player.HealTimer / player.HealCooldown;
        kick.fillAmount = player.InterruptTimer / player.InterruptCooldown;
        poke.fillAmount = player.InstantCastTimer / player.InstantCooldown;
    }
}
