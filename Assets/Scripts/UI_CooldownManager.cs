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

    [SerializeField] private Image[] globalCooldownImages;
    
    
 
    // Update is called once per frame
    void Update()
    {
        fireball.fillAmount = player.FireballTimer/player.FireballCooldown;
        heal.fillAmount = player.HealTimer / player.HealCooldown;
        kick.fillAmount = player.InterruptTimer / player.InterruptCooldown;
        poke.fillAmount = player.InstantCastTimer / player.InstantCooldown;

        foreach (var gcd in globalCooldownImages)
        {
            gcd.fillAmount = player.GlobalCooldownTimer / player.GlobalCooldownDuration; 
        }
    }
}
