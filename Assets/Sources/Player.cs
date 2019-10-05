using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
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
