using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerLobbyController : MonoBehaviour
{
    [Header("Player Info")] public int playerIndex;
    public string playerName;

    [Header("UI References")] public Text playerNameText;
    public Button readyButton;
    public Image playerAvatar;
    public GameObject playerUIPanel;

    [Header("Input Settings")] public InputActionReference readyAction;
    public InputActionReference cancelAction;

    private PlayerInput playerInput;
    private bool isReady = false;
    private LobbyManager lobbyManager;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    void OnEnable()
    {
        // Subscribe to input actions
        if (readyAction != null)
        {
            readyAction.action.performed += OnReadyPressed;
            readyAction.action.Enable();
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed += OnCancelPressed;
            cancelAction.action.Enable();
        }
    }

    void OnDisable()
    {
        // Unsubscribe from input actions
        if (readyAction != null)
        {
            readyAction.action.performed -= OnReadyPressed;
            readyAction.action.Disable();
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed -= OnCancelPressed;
            cancelAction.action.Disable();
        }
    }

    public void Initialize(int index)
    {
        playerIndex = index;
        playerName = $"Player {index + 1}";

        // Update UI elements
        if (playerNameText != null)
            playerNameText.text = playerName;

        if (readyButton != null)
        {
            readyButton.onClick.AddListener(ToggleReady);
            UpdateReadyButton();
        }

        // Set up UI navigation for this player's input
        SetupUINavigation();

        Debug.Log($"Initialized {playerName}");
    }

    private void SetupUINavigation()
    {
        // Get or add InputSystemUIInputModule for this player
        var uiModule = GetComponent<InputSystemUIInputModule>();
        if (uiModule == null)
        {
            uiModule = gameObject.AddComponent<InputSystemUIInputModule>();
        }

        // Configure UI input for this player
        uiModule.actionsAsset = playerInput.actions;
    }

    private void OnReadyPressed(InputAction.CallbackContext context)
    {
        ToggleReady();
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (isReady)
        {
            SetReady(false);
        }
        else
        {
            // Leave the lobby
            LeaveLobby();
        }
    }

    public void ToggleReady()
    {
        SetReady(!isReady);
    }

    public void SetReady(bool ready)
    {
        isReady = ready;
        UpdateReadyButton();

        Debug.Log($"{playerName} is {(isReady ? "ready" : "not ready")}");

        // Notify lobby manager or other systems about ready state change
        // You can add events here for other systems to listen to
    }

    private void UpdateReadyButton()
    {
        if (readyButton != null)
        {
            var buttonText = readyButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isReady ? "Ready!" : "Press to Ready";
            }

            // Change button color based on ready state
            var colors = readyButton.colors;
            colors.normalColor = isReady ? Color.green : Color.white;
            readyButton.colors = colors;
        }
    }

    public void LeaveLobby()
    {
        // Remove this player from the input manager
        if (playerInput != null)
        {
            Destroy(playerInput.gameObject);
        }
    }

    public bool IsReady => isReady;
    public int PlayerIndex => playerIndex;
    public string PlayerName => playerName;
}