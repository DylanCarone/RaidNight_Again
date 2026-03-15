using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_GameManager : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] PlayerCombatEntity player1;
    [SerializeField] Transform player1Spawn;
    [SerializeField] private UI_CooldownManager player1UI;
    [SerializeField] private HealthBar player1HealthUI;
    [Header("Player 2")]
    [SerializeField] PlayerCombatEntity player2;
    [SerializeField] Transform player2Spawn;
    [SerializeField] private UI_CooldownManager player2UI;
    [SerializeField] private bool debugPlayer2 = false;
    [Header("Player 3")]
    [SerializeField] PlayerCombatEntity player3;
    [SerializeField] Transform player3Spawn;
    [SerializeField] private UI_CooldownManager player3UI;
    [SerializeField] private bool debugPlayer3 = false;
    [Header("Player 4")]
    [SerializeField] PlayerCombatEntity player4;
    [SerializeField] Transform player4Spawn;
    [SerializeField] private UI_CooldownManager player4UI;
    [SerializeField] private bool debugPlayer4 = false;

    [Header("Boss")] [SerializeField] private BossCombatEntity boss; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var allPlayerData = FindObjectsByType<PlayerData>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var data in allPlayerData)
        {
            GameObject model = null;
            switch (data.PlayerIndex)
            {
                case 0:
                    player1.gameObject.SetActive(true);
                    model = Instantiate(data.characterData.playerModel, player1Spawn.position, player1Spawn.rotation, player1Spawn);
                    player1.Initialize(data.characterData, data.GetComponent<PlayerInput>(), model);
                    player1UI.Initialize(player1);
                    break;
                case 1:
                    player2.gameObject.SetActive(true);
                    model = Instantiate(data.characterData.playerModel, player2Spawn.position, player2Spawn.rotation, player2Spawn);
                    player2.Initialize(data.characterData, data.GetComponent<PlayerInput>(), model);
                    player2UI.Initialize(player2);
                    break;
                case 2:
                    player3.gameObject.SetActive(true);
                    model = Instantiate(data.characterData.playerModel, player3Spawn.position, player3Spawn.rotation, player3Spawn);
                    player3.Initialize(data.characterData, data.GetComponent<PlayerInput>(), model);
                    player3UI.Initialize(player3);
                    break;
                case 3:
                    player4.gameObject.SetActive(true);
                    model = Instantiate(data.characterData.playerModel, player4Spawn.position, player4Spawn.rotation, player4Spawn);
                    player4.Initialize(data.characterData, data.GetComponent<PlayerInput>(), model);
                    player4UI.Initialize(player4);
                    break;
                default:
                    Debug.LogError("Too many players");
                    break;
                    
            }
        }

        if (debugPlayer2)
        {
            player2UI.Initialize(player2);
        }
        
        
        boss.Initialize();
    }
    
}
