using UnityEngine;

namespace LDJAM45
{
    public class Wolf : MonoBehaviour
    {
        [Header("Read-Only (debug)")]
        public Vector2 origin;
        public Vector2 destination;

        [Header("Gameplay parameters")]
        public float speedAmplitude = 1f;
        public float speedFrequency = 2;

        public float jumpAmplitude = 0.5f;
        public float jumpRequency = 1f;
        public float groundHeight = 2;
        public float jumpHeight = 3;

        void Start()
        {
            origin = transform.position;
            destination = GameManager.instance.camp.position;
            groundHeight = origin.y;

            if ((destination - origin).x < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
        }

        float t = 0;
        float velocity = 0;
        float lerpT = 0;

        float GetX()
        {
            float deltaX = Vector2.Distance(origin, destination);

            t += Time.deltaTime;
            velocity = speedAmplitude * Mathf.Abs(Mathf.Sin(speedFrequency * t));
            // x += v / 50;
            lerpT += velocity / 50;

            float x = Mathf.Lerp(origin.x, destination.x, lerpT / deltaX);

            return x;
        }

        float jumpVelocity = 0;

        float GetY()
        {
            jumpVelocity = jumpAmplitude * Mathf.Abs(Mathf.Sin(speedFrequency * t));
            // x += v / 50;
            // lerpT += jumpVelocity;

            return Mathf.Lerp(groundHeight, groundHeight + jumpHeight, jumpVelocity);
        }

        void Update()
        {
            transform.position = new Vector2(GetX(), GetY());
            transform.rotation = Quaternion.Euler(0, 0, 20 + Mathf.LerpAngle(320, 20, jumpVelocity));

            if (Vector2.Distance(transform.position, destination) <= 0.5f)
            {
                if (GameManager.instance.fishCount > 0)
                {
                    GameManager.instance.RemoveFish();
                }

                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Arrow arrow = other.GetComponent<Arrow>();

            if (arrow != null)
            {
                Destroy(arrow.gameObject);
                Destroy(gameObject);
            }
        }
    }
}