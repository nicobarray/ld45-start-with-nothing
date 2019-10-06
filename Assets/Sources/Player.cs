using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class Player : MonoBehaviour
    {
        [Header("Components")]
        public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;

        [Header("Player Configuration")]
        public float speed = 5;

        void Update()
        {
            float boost = 1;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                boost = 10;
            }

            float horizontalInput = Input.GetAxisRaw("Horizontal");

            spriteRenderer.flipX = horizontalInput < 0;

            if (horizontalInput == 0)
            {
                return;
            }

            if (transform.position.x < GameManager.instance.leftShore.position.x && horizontalInput < 0)
            {
                return;
            }

            if (transform.position.x > GameManager.instance.rightShore.position.x && horizontalInput > 0)
            {
                return;
            }

            transform.Translate(Vector2.right * horizontalInput * speed * boost * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var fish = other.GetComponent<Fish>();

            if (fish != null)
            {
                Destroy(fish.gameObject);
                GameManager.instance.fishCount++;
            }
        }
    }
}
