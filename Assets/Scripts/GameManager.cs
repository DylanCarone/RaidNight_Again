using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject startMenu;

    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject youWinScreen;
    [SerializeField] private GameObject youLoseScreen;

    [SerializeField] private GameObject countDownUI;
    [SerializeField] private TextMeshProUGUI countDownText;

    [SerializeField] private CombatEntity player;
    [SerializeField] private CombatEntity boss;


    private bool gameStarted = false;

    private bool gameOver = false;

    private bool canRestart = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame) && canRestart)
            {
                SceneManager.LoadScene(0);
            }
            return; 
        }

        
        CheckWinner();
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame) && !gameStarted)
        {
            StartCoroutine(StartGame());
        }
    }


    IEnumerator StartGame()
    {
        gameStarted = true;
        countDownUI.SetActive(true);
        startMenu.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 0;
        yield return StartCoroutine(BeginCountDown());
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        countDownUI.SetActive(false);

    }

    IEnumerator BeginCountDown()
    {
        countDownText.text = "3";
        yield return new WaitForSecondsRealtime(1);
        countDownText.text = "2";
        yield return new WaitForSecondsRealtime(1);
        countDownText.text = "1";
        yield return new WaitForSecondsRealtime(1);
        countDownText.text = "GO!";
    }

    IEnumerator BeginRestartTimer()
    {
        yield return new WaitForSeconds(2);
        canRestart = true;
    }
    void CheckWinner()
    {
        if (boss.IsDead)
        {
            youWinScreen.SetActive(true);
            gameOver = true;
            StartCoroutine(BeginRestartTimer());
        }

        if (player.IsDead)
        {
            youLoseScreen.SetActive(true);
            gameOver = true;
            StartCoroutine(BeginRestartTimer());
        }
    }
}
