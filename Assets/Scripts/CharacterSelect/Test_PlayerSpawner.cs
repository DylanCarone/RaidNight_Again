using System;
using UnityEngine;

public class Test_PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        var allPlayerData = FindObjectsByType<PlayerData>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var data in allPlayerData)
        {
            var spawnPoint = spawnPoints[data.PlayerIndex];
            var character = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
            var renderer = character.GetComponent<SpriteRenderer>();
            if(renderer!=null) 
                renderer.color = data.PlayerColor;
            Debug.Log($"Player {data.PlayerIndex} is actually a {data.characterData.className}");
        }
    }
}
