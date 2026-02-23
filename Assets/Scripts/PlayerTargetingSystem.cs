using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetingSystem : MonoBehaviour
{
    [SerializeField] private PlayerCombatEntity player;


    private void Update()
    {
        if (player.Role != PlayerRole.Healer)
        {
            player.SetTarget(player.FindNearestEnemy());
            return;
        }
        
        if (player.Inputs.actions.FindAction("Target North").WasPressedThisFrame())
        {
            player.SetTarget(TargetPlayerRole(PlayerRole.Tank));
        }
        if (player.Inputs.actions.FindAction("Target East").WasPressedThisFrame())
        {
            player.SetTarget(TargetPlayerRole(PlayerRole.Ranged));
        }
        if (player.Inputs.actions.FindAction("Target South").WasPressedThisFrame())
        {
            player.SetTarget(TargetPlayerRole(PlayerRole.Healer));
        }
        if (player.Inputs.actions.FindAction("Target West").WasPressedThisFrame())
        {
            player.SetTarget(TargetPlayerRole(PlayerRole.Melee));
        }
    }


    PlayerCombatEntity TargetPlayerRole(PlayerRole role)
    {
        PlayerCombatEntity[] players = FindObjectsByType<PlayerCombatEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var player in players )
        {
            if (player.Role == role)
            {
                return player;
            }
        }

        return null;
    }
}
