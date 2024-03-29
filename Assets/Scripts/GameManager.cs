﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // UI GameObjects 
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject playerOneHealthBar;
    private Image playerOneHealthBarImage;
    [SerializeField] GameObject playerTwoHealthBar;
    private Image playerTwoHealthBarImage;

    [SerializeField] List<GameObject> playerOneRounds;
    [SerializeField] List<GameObject> playerTwoRounds;

    // Class Chosen 
    [SerializeField] List<GameObject> characters;
    [SerializeField] float initialXDistanceToOrigin = 7.5f;
    [SerializeField] float initialYDistanceToOrigin = -1.5f;
    private GameObject playerOne;
    private GameObject playerTwo;

    private GameObject PauseMenu;
    private GameObject gameOverUI;
    private GameObject winnerTextUI;

    // Round info
    [SerializeField] int maxRound = 3;
    public int round = 1;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;

    // Health info
    [SerializeField] int maxHealth = 100;
    public int playerOneHealth;
    public int playerTwoHealth;

    // Events for decreasing players healths
    private class IntEvent : UnityEvent<int> { };
    private IntEvent playerOneDamagedEvent;
    private IntEvent playerTwoDamagedEvent;

    private bool playersInstantiated = false;

    private void Awake()
    {
        if (!playersInstantiated)
        {
            InstantiatePlayers();
        }
        playerOneHealthBarImage = playerOneHealthBar.GetComponent<Image>();
        playerTwoHealthBarImage = playerTwoHealthBar.GetComponent<Image>();

        PauseMenu = canvas.GetComponent<Transform>().GetChild(0).gameObject;
        gameOverUI = canvas.GetComponent<Transform>().GetChild(1).gameObject;
        winnerTextUI = gameOverUI.GetComponent<Transform>().GetChild(0).gameObject;

        if (playerOneDamagedEvent == null)
            playerOneDamagedEvent = new IntEvent();
        if (playerTwoDamagedEvent == null)
            playerTwoDamagedEvent = new IntEvent();

        playerOneDamagedEvent.AddListener(DecPlayerOneHealth);
        playerTwoDamagedEvent.AddListener(DecPlayerTwoHealth);

        playerOneHealth = maxHealth;
        playerTwoHealth = maxHealth;
    }

    private void Update()
    {
        //print(playersInstantiated);

        UpdateHealthBar();
        CheckWinner();
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void InstantiatePlayers()
    {
        print("INSTANTIATE");
        int p1 = PlayerPrefs.GetInt("PlayerOneChar");
        int p2 = PlayerPrefs.GetInt("PlayerTwoChar");

        playerOne = Instantiate(characters[p1], new Vector2(-initialXDistanceToOrigin, initialYDistanceToOrigin),
                                           Quaternion.identity);
        playerTwo = Instantiate(characters[p2], new Vector2(initialXDistanceToOrigin, initialYDistanceToOrigin), 
                                           Quaternion.identity);
        playerTwo.GetComponent<SpriteRenderer>().flipX = true;

        AttachPlayers();
    }

    private void AttachPlayers()
    {
        Player_Input inputScript = GetComponent<Player_Input>();
        inputScript.Player1 = playerOne;
        inputScript.Player2 = playerTwo;
        inputScript.Player1_rb = playerOne.GetComponent<Rigidbody2D>();
        inputScript.Player2_rb = playerTwo.GetComponent<Rigidbody2D>();

        playersInstantiated = true;
    }
    private void UpdateHealthBar()
    {
        playerOneHealthBarImage.fillAmount = ((float)playerOneHealth / (float)maxHealth);
        playerTwoHealthBarImage.fillAmount = ((float)playerTwoHealth / (float)maxHealth);
    }

    private void TogglePause()
    {
        if (PauseMenu.activeSelf)
            PauseMenu.SetActive(false);
        else
            PauseMenu.SetActive(true);
    }

    private void CheckGameOver()
    {
        ResetHealth();

        int scoreToWin = (int)Mathf.Ceil(maxRound / 2f);

        if (playerOneScore == scoreToWin)
            GameOver(1);
        else if (playerTwoScore == scoreToWin)
            GameOver(2);
        else
            StartNewRound();
    }

    private void CheckWinner()
    {
        if (playerOneHealth == 0)
        {
            playerTwoRounds[playerTwoScore].SetActive(true);
            playerTwoScore++;
            CheckGameOver();
        }
        if (playerTwoHealth == 0)
        {
            playerOneRounds[playerOneScore].SetActive(true);
            playerOneScore++;
            CheckGameOver();
        }
    }

    private void StartNewRound()
    {
        round++;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvas);
        playersInstantiated = false;
        SceneManager.LoadScene("MainScene");
    }

    private void GameOver(int winner)
    {
        Time.timeScale = 0;

        gameOverUI.SetActive(true);
        TextMeshProUGUI winText = winnerTextUI.GetComponent<TextMeshProUGUI>();
        winText.text = "Player " + winner.ToString() + " Wins!";
    }

    private void ResetHealth()
    {
        playerOneHealth = maxHealth;
        playerTwoHealth = maxHealth;
    }

    private void DecPlayerOneHealth(int damage)
    {
        playerOneHealth -= damage;
    }

    private void DecPlayerTwoHealth(int damage)
    {
        playerTwoHealth -= damage;
    }

    // Setters
    public void SetPlayerOneDamaged(int damage)
    {
        playerOneDamagedEvent.Invoke(damage);
    }

    public void SetPlayerTwoDamaged(int damage)
    {
        playerTwoDamagedEvent.Invoke(damage);
    }

    // Getters
    public int GetPlayerOneHealth()
    {
        return playerOneHealth;
    }

    public int GetPlayerTwoHealth()
    {
        return playerTwoHealth;
    }
}
