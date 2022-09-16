using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int playerHP;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject pauseScreen;
    bool isPaused;

    private void Start()
    {
        TakeDamage(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
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

    private void PauseGame()
    {
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
