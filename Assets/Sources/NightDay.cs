using UnityEngine;

namespace LDJAM45
{
    public class NightDay : MonoBehaviour
    {
        float time = 0;
        float relativeTime = 0;

        public Color dayTint;
        public Color nightTint;
        public float timeSpeed = 1;
        public int dayLength = 1;
        public int nightLength = 1;

        public TMPro.TextMeshProUGUI clockField;
        public Sprite sprite;

        void PlotFish(Color color, float x, float y)
        {
            float debugX = (x % 10) - 5;
            GameObject timeGo = new GameObject("time_plot");
            var sp = timeGo.AddComponent<SpriteRenderer>();
            sp.color = color;
            sp.sprite = sprite;
            sp.sortingLayerName = "UI";
            timeGo.transform.position = new Vector2(debugX, 5 + y);
            timeGo.transform.localScale = Vector2.one * 0.25f;
            Destroy(timeGo, 1.5f);
        }

        void Update()
        {
            time += Time.deltaTime * timeSpeed;

            var sprites = FindObjectsOfType<SpriteRenderer>();

            relativeTime = time;

            float dayBegins = Mathf.PI / 2;
            float dayEnds = dayBegins + 2 * Mathf.PI * dayLength;

            float nightBegins = dayEnds + Mathf.PI;
            float nightEnds = nightBegins + 2 * Mathf.PI * nightLength;

            float boundTime = time % (2 * Mathf.PI * (dayLength + nightLength + 1));

            // relativeTime = (Mathf.Sin(time) / 2) + 0.5f;
            if (boundTime >= dayBegins && boundTime <= dayEnds)
            {
                relativeTime = 1;
            }
            else if (boundTime >= nightBegins && boundTime <= nightEnds)
            {
                relativeTime = 0;
            }
            else
            {
                relativeTime = Mathf.Sin(time) / 2 + 0.5f;
                // relativeTime = 0.5f;
            }

            // print("Relative Time " + relativeTime);

            // PlotFish(new Color(256, 256, 256, 0.25f), time, 1);
            // PlotFish(new Color(256, 256, 256, 0.25f), time, 0);

            // PlotFish(new Color(256, 0, 256, 0.25f), time, Mathf.Sin(4 * time) / 2 + 0.5f);
            // PlotFish(Color.red, time, relativeTime);

            foreach (var item in sprites)
            {
                item.color = Color.Lerp(nightTint, dayTint, relativeTime);
            }
        }
    }
}