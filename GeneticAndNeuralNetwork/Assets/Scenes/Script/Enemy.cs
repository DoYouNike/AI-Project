using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyObject;

    void Start()
    {
        StartCoroutine("Spawn");
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            if (!GameControl.instance.gameOver)
            {
                Instantiate(enemyObject);
            }

            yield return new WaitForSeconds(Random.Range(GameControl.instance.minEnemySpawnInterval, GameControl.instance.maxEnemySpawnInterval));
        }

    }
}
