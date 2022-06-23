using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    float Startposition, length;
    // Start is called before the first frame update
    void Start()
    {
        Startposition = transform.position.x;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x*12;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameControl.instance.gameOver)
        {
            transform.position += Vector3.left * GameControl.instance.speed * Time.deltaTime;
        }

        if (transform.position.x < -length)
        {
            transform.position = new Vector2(length, transform.position.y);
        }

    }

}
