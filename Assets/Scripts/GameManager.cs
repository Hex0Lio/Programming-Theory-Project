using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int playerHP;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Start()
    {
        TakeDamage(0);
    }

    public void TakeDamage(int amount)
    {
        playerHP -= amount;
        if (playerHP <= 0) GameOver();
        hpText.SetText("Player HP: " + playerHP);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
