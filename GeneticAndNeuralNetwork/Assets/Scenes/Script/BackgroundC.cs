using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundC : MonoBehaviour
{
    // method to make the background realistic and always in the scenes.
    float Startposition, length;
    // Start is called before the first frame update
    void Start()
    {
        Startposition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        //check if the the game is over or not if not transform in the x direction using the player speed.
        if (!GameControl.instance.gameOver)
        {
            transform.position += Vector3.left * (GameControl.instance.speed/10) * Time.deltaTime;
        }

        // move to the original position
        if (transform.position.x < -length)
        {
            transform.position = new Vector2(length, transform.position.y);
        }

    }
}
