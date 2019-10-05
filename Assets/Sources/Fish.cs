using System.Collections;
using UnityEngine;

namespace LDJAM45
{
    public class Fish : MonoBehaviour
    {
        public Rigidbody2D rb;
        public BoxCollider2D box;

        public bool isGrounded = false;
        private Coroutine coroutine;

        public float squashSteps = 10;
        public float stableSteps = 3;

        IEnumerator Squash()
        {
            transform.localScale = Vector2.one;

            Vector2 stable = Vector2.one;
            Vector2 squash = new Vector2(2f, 0.5f);

            for (float t = 0; t < 1; t += 1 / squashSteps)
            {
                transform.localScale = Vector2.Lerp(stable, squash, t);
                yield return null;
            }

            for (float t = 0; t < 1; t += 1 / stableSteps)
            {
                transform.localScale = Vector2.Lerp(squash, stable, t);
                yield return null;
            }

            transform.localScale = Vector2.one;
        }

        void Start()
        {
            rb.velocity = new Vector2(UnityEngine.Random.insideUnitCircle.x * 2, 10);
            box.enabled = false;
        }

        void Update()
        {
            if (!box.enabled && rb.velocity.y < 0f)
            {
                box.enabled = true;
            }
        }

        void FixedUpdate()
        {
            bool wasGrounded = isGrounded;
            var hit = Physics2D.Raycast(transform.position.Vec2() + Vector2.down * 0.51f, Vector2.down);

            if (!isGrounded && hit.collider != null && !hit.collider.isTrigger && hit.distance < 0.1f)
            {
                isGrounded = true;
                if (!wasGrounded)
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(Squash());
                }
            }
            else if (isGrounded && hit.collider != null && hit.distance >= 0.1f)
            {
                isGrounded = false;
            }
        }
    }
}