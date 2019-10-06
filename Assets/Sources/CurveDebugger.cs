using UnityEngine;

namespace LDJAM45
{
    public class CurveDebugger : MonoBehaviour
    {
        float x = 0;
        float v = 0;
        float t = 0;
        float angle = 0;

        Vector2 origin;
        public Transform movingTarget;

        public float amplitude = 0.5f;
        public float frequency = 4;

        void Start()
        {
            origin = transform.position;
        }

        void Update()
        {
            t += Time.deltaTime;
            v = amplitude * Mathf.Abs(Mathf.Sin(frequency * t));
            x += v / 50;
            transform.position = new Vector2(origin.x, origin.y + v);
            movingTarget.position = new Vector2(origin.x - 10 + x, origin.y);
        }
    }
}