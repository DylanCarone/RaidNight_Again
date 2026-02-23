using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeaveHandler : MonoBehaviour
{
    private PlayerInput _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    // Wire this to PlayerInput -> Events -> UI -> Cancel
    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlayerInputManager.instance.playerJoinedEvent.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}
