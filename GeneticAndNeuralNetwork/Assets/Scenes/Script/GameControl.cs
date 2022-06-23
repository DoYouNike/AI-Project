using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;
    public Text highScoreText;
    public AudioSource backgroundMusic;
    public float speed = 10f;
    public float maxSpeed = 16f;
    public float score = 0;
    public bool gameOver = false;
    public float minEnemySpawnInterval = 0.8f;
    public float maxEnemySpawnInterval = 1.2f;
    public int alives = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        if (highScoreText)
        {
            UpdateHighScore();
        }
    }

    void UpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null && highScore > 0)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "HIGH SCORE: " + highScore;
        }
    }

    void Update()
    {
        if (alives <= 0)
        {
            gameOver = true;  
        }

        if (!gameOver)
        {
            score += 0.05f;
            if (speed < maxSpeed)
            {
                speed += 0.001f;
            }

            if (highScoreText != null)
            {
                if (score > PlayerPrefs.GetInt("HighScore", 0))
                {
                    PlayerPrefs.SetInt("HighScore", Mathf.RoundToInt(score));
                }

                UpdateHighScore();
            }

        }
       
    }

    public void Reset()
    {
        gameOver = false;
        speed = 10f;

        score = 0;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            for (var i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
    }
}
