using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAOE : MonoBehaviour
{
    [SerializeField] private float damagePerTick;
    [SerializeField] private float tickRate;
    List<PlayerCombatEntity> playersEffected = new List<PlayerCombatEntity>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TickCoroutine());
    }

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCombatEntity>() == null) return;
        
        playersEffected.Add(other.gameObject.GetComponent<PlayerCombatEntity>());
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCombatEntity>() == null) return;
        
        playersEffected.Remove(other.gameObject.GetComponent<PlayerCombatEntity>());
    }

    private void OnDestroy()
    {
        playersEffected.Clear();
    }
    
    private IEnumerator TickCoroutine()
    {
        float timeSinceLastTick = 0f;

        while (true)
        {
            yield return null;

            float deltaTime = Time.deltaTime;
            timeSinceLastTick += deltaTime;

            if (timeSinceLastTick >= tickRate)
            {
                OnTick();
                timeSinceLastTick = 0f;
            }
        }
        yield return null;
    }

    private void OnTick()
    {
        foreach (var player in playersEffected)
        {
            player.TakeDamage(damagePerTick);
        }
    }
}
