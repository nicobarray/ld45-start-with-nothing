using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;

    [Header("Player Configuration")]
    public float speed = 5;


    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput == 0)
        {
            return;
        }

        transform.Translate(Vector2.right * horizontalInput * speed * Time.deltaTime);
    }
}
