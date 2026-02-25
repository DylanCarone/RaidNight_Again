using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] AbilityPanelDisplay[] abilityPanels;
    
    private List<CharacterData> availableCharacters = new List<CharacterData>();
    private CharacterData currentCharacter;

    private GameObject spawnedPlayer;
    
    [SerializeField] PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;
    private bool playerSelected = false;
    
    public Action<CharacterData> OnCharacterSelected;
    public Action<CharacterData> OnCharacterDeselected;

    private void Start()
    {
        if (availableCharacters.Count == 0) return;
        currentCharacter = availableCharacters[0];
        int spellIndex = 0;
        spawnedPlayer = Instantiate(currentCharacter.playerModel, playerModel.transform.position, playerModel.transform.rotation);
        foreach (var panel in abilityPanels)
        {
            panel.Initialize(currentCharacter.abilities[spellIndex]);
            spellIndex++;
        }
        titleText.text = currentCharacter.className;
        
    }

    private void Update()
    {
        if (playerInput == null) return;

        if (playerSelected && playerInput.actions.FindAction("Cancel").WasPressedThisFrame())
        {
            DeselectCharacter(currentCharacter);
            return;
        }

        if (playerSelected) return;

        if (!playerSelected && playerInput.actions.FindAction("Cancel").WasPressedThisFrame())
        {
            startPanel.SetActive(true);
            Destroy(playerInput.gameObject);
            playerInput = null;
            return;
        }

        if (playerInput.actions.FindAction("Move Left").WasPressedThisFrame())
        {
            SelectPreviousCharacter();
        }
        if (playerInput.actions.FindAction("Move Right").WasPressedThisFrame())
        {
            SelectNextCharacter();
        }
        if (playerInput.actions.FindAction("Submit").WasPressedThisFrame())
        {
            SelectCharacter(currentCharacter);
        }
        

    }

    public void AddCharacter(CharacterData character)
    {
        if (availableCharacters.Contains(character)) return;
        availableCharacters.Add(character);
    }

    public void RemoveCharacter(CharacterData character)
    {
        if (!availableCharacters.Contains(character)) return;
        availableCharacters.Remove(character);
    }
    

    [ContextMenu("Select next")]
    public void SelectNextCharacter()
    {
        int index = availableCharacters.IndexOf(currentCharacter) + 1;
        currentCharacter = availableCharacters[index % availableCharacters.Count];
        titleText.text = currentCharacter.className;
        Destroy(spawnedPlayer);
        spawnedPlayer = Instantiate(currentCharacter.playerModel, playerModel.transform.position, playerModel.transform.rotation);
        int spellIndex = 0;
        foreach (var panel in abilityPanels)
        {
            panel.Initialize(currentCharacter.abilities[spellIndex]);
            spellIndex++;
        }
    }
    
    [ContextMenu("Select prev")]
    public void SelectPreviousCharacter()
    {
        int index = availableCharacters.IndexOf(currentCharacter) - 1;
        if(index < 0)
            index = availableCharacters.Count - 1;
        
        currentCharacter = availableCharacters[index];
        titleText.text = currentCharacter.className;
        Destroy(spawnedPlayer);
        spawnedPlayer = Instantiate(currentCharacter.playerModel, playerModel.transform.position, playerModel.transform.rotation);
        int spellIndex = 0;
        foreach (var panel in abilityPanels)
        {
            panel.Initialize(currentCharacter.abilities[spellIndex]);
            spellIndex++;
        }
    }

    public void SelectCharacter(CharacterData selectedCharacter)
    {
        playerSelected = true;
        // send this info to the lobby manager
        // play the select animation
        spawnedPlayer.GetComponent<Animator>().SetBool("IsCasting", playerSelected);
        // disallow other players from selected the same character
        OnCharacterSelected?.Invoke(selectedCharacter);
        
    }

    public void DeselectCharacter(CharacterData selectedCharacter)
    {
        playerSelected = false;
        spawnedPlayer.GetComponent<Animator>().SetBool("IsCasting", playerSelected);
        OnCharacterDeselected?.Invoke(selectedCharacter);
    }

    public void JoinedPlayer(PlayerInput player)
    {
        playerInput = player;
        startPanel.SetActive(false);
    }

    public CharacterData GetCurrentCharacter()=>currentCharacter;
    public bool IsPlayerSelected => playerSelected;
}

