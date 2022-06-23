using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    void Update()
    {
        if (!GameControl.instance.gameOver)
        {
            transform.position += Vector3.left * GameControl.instance.speed * Time.deltaTime;
        }

        if (transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }
}
