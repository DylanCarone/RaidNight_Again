using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestDriver : MonoBehaviour
{
    
    public PlayerInputManager inputManager;
    public PlayerCombatEntity testPlayer;
    public PlayerCombatEntity testPlayer2;

    public GameObject player1Highlight;
    public GameObject player2Highlight;
    
    private Dictionary<int, int> selections = new Dictionary<int, int>();

    void Update()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad gamepad)
            {
                if (gamepad.buttonWest.wasPressedThisFrame)
                {
                    TrySelect(gamepad.deviceId, 1);
                }
                if (gamepad.buttonNorth.wasPressedThisFrame)
                {
                    TrySelect(gamepad.deviceId, 2);
                }

                if (gamepad.selectButton.wasPressedThisFrame)
                {
                    TryDeselect(gamepad.deviceId);
                }
            }
        }
    }


    private void TrySelect(int deviceId, int slot)
    {
        if (selections.ContainsValue(slot) && (!selections.ContainsKey(deviceId) || selections[deviceId] != slot))
        {
            Debug.Log($"Slot {slot} is taken!");
            return;
        }
        
        // 2. Remove old selection if this device is switching slots
        if (selections.ContainsKey(deviceId))
        {
            int oldSlot = selections[deviceId];
            SetHighlight(oldSlot, false);
            selections.Remove(deviceId);
        }

        // 3. Assign new selection
        selections.Add(deviceId, slot);
        SetHighlight(slot, true);
    }
    
    void TryDeselect(int deviceId)
    {
        if (selections.ContainsKey(deviceId))
        {
            int currentSlot = selections[deviceId];
            SetHighlight(currentSlot, false); // Turn off the UI
            selections.Remove(deviceId);      // Forget the device
            Debug.Log($"Device {deviceId} left slot {currentSlot}");
        }
    }
    
    void SetHighlight(int slot, bool active)
    {
        if (slot == 1) player1Highlight.SetActive(active);
        if (slot == 2) player2Highlight.SetActive(active);
    }
    
    
    
}
