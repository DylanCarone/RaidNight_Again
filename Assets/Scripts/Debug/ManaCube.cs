using System;
using UnityEngine;

public class ManaCube : MonoBehaviour
{
    public float manaAmount = 50;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCombatEntity>(out var player))
        {
            player.RestoreResource(manaAmount);
            Destroy(gameObject);
        }
    }
}
