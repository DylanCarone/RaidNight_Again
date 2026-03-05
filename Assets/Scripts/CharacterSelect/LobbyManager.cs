
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private CharacterData[] classes;
    [SerializeField] private CharacterPanel[] characterPanels;
    [SerializeField] private PlayerInputManager inputManager;
    private int playerIndex = 0;
    private int numPlayers = 0;
    private void Start()
    {
        AddCharactersToPanels();
        inputManager.onPlayerJoined += OnPlayerJoined;
        inputManager.onPlayerLeft += OnPlayerLeft;

    }

    private void AddCharactersToPanels()
    {
        foreach (var panel in characterPanels)
        {
            panel.OnCharacterSelected += OnCharacterSelected;
            panel.OnCharacterDeselected += AddCharactersToOtherPanels;
            foreach (var character in classes)
            {
                panel.AddCharacter(character);
            }
        }
    }

    private void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
        inputManager.onPlayerLeft -= OnPlayerLeft;
        foreach (var panel in characterPanels)
        {
            panel.OnCharacterSelected -= OnCharacterSelected;
            panel.OnCharacterDeselected -= AddCharactersToOtherPanels;
            
        }
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame && numPlayers >= 1)
        {
            SceneManager.LoadScene("Scenes/TrainingRoom");
        }
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        numPlayers--;
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        
        characterPanels[playerInput.playerIndex].JoinedPlayer(playerInput);
        var data =playerInput.GetComponent<PlayerData>();
        data.PlayerIndex = playerInput.playerIndex;
        

        switch (playerInput.playerIndex)
        {
            case 0:
                data.PlayerColor = Color.red;
                break;
            case 1:
                data.PlayerColor = Color.blue;
                break;
            case 2:
                data.PlayerColor = Color.yellow;
                break;
            case 3:
                data.PlayerColor = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            
        }
        
    }

    void OnCharacterSelected(CharacterData selectedCharacter)
    {
        CharacterPanel selectingPanel = null;
        foreach (var panel in characterPanels)
        {
            if (panel.GetCurrentCharacter() == selectedCharacter && panel.IsPlayerSelected)
            {
                selectingPanel = panel;
                break;
            }
        }

        if (selectingPanel != null && selectingPanel.PlayerInput != null)
        {
            // Set the character data on the PlayerInput's PlayerData component
            var playerData = selectingPanel.PlayerInput.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.characterData = selectedCharacter; // Assuming PlayerData has a CharacterData property
            }

            RemoveCharacterFromOtherPanels(selectedCharacter);

        }
    }


    void RemoveCharacterFromOtherPanels(CharacterData selectedCharacter)
    {
        CharacterPanel selectingPanel = null;
        foreach (var panel in characterPanels)
        {
            if (panel.GetCurrentCharacter() == selectedCharacter && panel.IsPlayerSelected)
            {
                selectingPanel = panel;
                break;
            }
        }

        foreach (var panel in characterPanels)
        {
            if (panel != selectingPanel)
            {
                panel.RemoveCharacter(selectedCharacter);
                if (panel.GetCurrentCharacter() == selectedCharacter)
                {
                    panel.SelectNextCharacter();
                }
            }
        }
        numPlayers--;
    }
    
    void AddCharactersToOtherPanels(CharacterData selectedCharacter)
    {
        CharacterPanel selectingPanel = null;
        foreach (var panel in characterPanels)
        {
            if (panel.GetCurrentCharacter() == selectedCharacter && panel.IsPlayerSelected)
            {
                selectingPanel = panel;
                break;
            }
        }

        foreach (var panel in characterPanels)
        {
            if (panel != selectingPanel)
            {
                panel.AddCharacter(selectedCharacter);
            }
        }
        numPlayers++;
    }

    
}