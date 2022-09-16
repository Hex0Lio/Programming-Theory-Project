using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playerHP;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject buttons;
    bool isPaused;
    bool gameOver;

    private void Start()
    {
        TakeDamage(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver) {
            if (!isPaused) {
                PauseGame();
            } else {
                ResumeGame();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        playerHP -= amount;
        if (playerHP <= 0) GameOver();
        hpText.SetText("Player HP: " + playerHP);
    }

    void PauseGame()
    {
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        buttons.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        buttons.SetActive(false);
    }

    void GameOver()
    {
        gameOver = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        buttons.SetActive(true);
        hpText.gameObject.SetActive(false);
    }
}
