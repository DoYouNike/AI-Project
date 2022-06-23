using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;
    public AIPlayerControl AIPlayer;
    List<AIPlayerControl> players = new List<AIPlayerControl>();
    public Genetic genetic;
    public Text alivesText;
    public Text levelText;
    public Text highScoreText;
    public Text speedText;

    public int populations = 50;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        genetic = new Genetic(2, 2, 1, populations, 0.2f, 0.1f, 0.2f);
        InitNeuralGenetic();
    }

    void InitNeuralGenetic()
    {
        genetic.PopulateGeneticOpts();

        for (int i = 0; i < genetic.geneticOpts.Count; i++)
        {
            AIPlayerControl player = Instantiate(AIPlayer);
            player.id = i;
            players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        alivesText.text = "Alives: " + Mathf.RoundToInt(GameControl.instance.alives);
        levelText.text = "Level: " + genetic.currentGeneticOpt;

        float highScore = genetic.maxScore;

        if (GameControl.instance.score > genetic.maxScore)
        {
            highScore = GameControl.instance.score;
        }

        highScoreText.text = "High Score: " + Mathf.RoundToInt(highScore);
        speedText.text = "Speed: " + Mathf.RoundToInt(GameControl.instance.speed);

        if (GameControl.instance.gameOver)
        {
            InitNeuralGenetic();
            GameControl.instance.gameOver = false;
            GameControl.instance.Reset();
        }
    }
}
