using UnityEngine;

namespace LDJAM45
{
    public class CurveDebugger : MonoBehaviour
    {
        float t = 0;

        Vector2 origin;
        public Transform movingTarget;

        public float amplitude = 0.5f;
        public float frequency = 4;

        public Sprite pointSprite;

        void PlotFish(Color color, float x, float y)
        {
            GameObject timeGo = new GameObject("time_plot");
            var sp = timeGo.AddComponent<SpriteRenderer>();
            sp.color = color;
            sp.sprite = pointSprite;
            sp.sortingLayerName = "UI";
            timeGo.transform.position = new Vector2(x, y);
            timeGo.transform.localScale = Vector2.one * 0.25f;
            Destroy(timeGo, 5f);
        }

        void Start()
        {
            origin = Vector2.zero;
        }

        Vector2 nextPosition;

        public float destinationDistance = 1;

        void Update()
        {
            // t += Time.deltaTime;
            // v = amplitude * Mathf.Abs(Mathf.Sin(frequency * t));
            // x += v / 50;
            // transform.position = new Vector2(origin.x, origin.y + v);
            // movingTarget.position = new Vector2(origin.x - 10 + x, origin.y);

            t += Time.deltaTime * 5;

            if (t > 10)
            {
                t -= 10;
            }

            Vector2 destination = origin + Vector2.right * destinationDistance;
            float distance = Mathf.Abs(origin.x - destination.x);
            float xOrientation = destination.x - origin.x > 0 ? 1 : -1;
            float x = xOrientation * t;
            float y = -Mathf.Pow(t, 2) / (distance * 2) + distance * t / (distance * 2);

            nextPosition = new Vector2(x, y);
            Vector2 direction = (nextPosition - transform.position.Vec2()).normalized;
            bool isFalling = direction.y < 0;

            transform.position = nextPosition;
            transform.rotation = Quaternion.Euler(0, 0, (isFalling ? -1 : 1) * Vector2.Angle(Vector2.right, direction));

            for (int i = -20; i < 20; i++)
            {
                PlotFish(new Color(0, 0, 0, 0.2f), t, i);
                PlotFish(new Color(0, 0, 0, 0.2f), i, t);
            }

            PlotFish(Color.red, nextPosition.x, y);
        }
    }
}