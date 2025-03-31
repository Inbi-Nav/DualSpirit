using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    public GameObject gameOverScreen;
    public TMP_Text survivedText;

    private int survivedLevelsCount;

    public static event Action OnReset;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;

        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOverScreen;

        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) survivedText.text += "S";

        Time.timeScale = 1;

        int userId = PlayerPrefs.GetInt("userId", -1);
        if (userId != -1)
        {
            StartCoroutine(RegistrarPartida(userId));
        }

        Time.timeScale = 0; // Pausa el juego
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        Loadlevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    public void GoToProfile()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayerProfile");
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            loadCanvas.SetActive(true);
            Debug.Log("Level Complete");
        }
    }

    void Loadlevel(int level, bool wantSurvivedIncrease)
    {
        loadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;

        if (wantSurvivedIncrease)
            survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        Loadlevel(nextLevelIndex, true);

        if (currentLevelIndex == levels.Count - 1)
        {
            int userId = PlayerPrefs.GetInt("userId", -1);
            if (userId != -1)
            {
                StartCoroutine(RegistrarPartida(userId));
            }
        }
    }

    IEnumerator RegistrarPartida(int userId)
    {
        string url = $"http://localhost:3000/users/{userId}/gamesPlayed";
        UnityWebRequest www = UnityWebRequest.Put(url, new byte[0]);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Partida registrada");
        }
        else
        {
            Debug.LogError("Error al registrar partida: " + www.error);
        }
    }
}
