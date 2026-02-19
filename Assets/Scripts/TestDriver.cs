using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestDriver : MonoBehaviour
{
    public CombatEntity player;
    public CombatEntity boss;

    private void Start()
    {
        player.StartAutoAttacking(boss);
    }
}
