using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

namespace LDJAM45
{
    public enum DayPeriod
    {
        NIGHT,
        DAY,
        DUSK,
        DAWN
    }

    public class NightDay : MonoBehaviour
    {
        float time = 0;
        float relativeTime = 0;

        public float timeSpeed = 1;
        public DayPeriod period = DayPeriod.DAWN;

        [Range(0.5f, 1)]
        public float dayIntensity;
        public int dayLength = 1;
        public Color dayTint;
        [Range(0, 0.5f)]
        public float nightIntensity;
        public int nightLength = 1;
        public Color nightTint;

        public TMPro.TextMeshProUGUI clockField;
        public Sprite sprite;
        public Light2D skylight;

        void Update()
        {
            time += Time.deltaTime * timeSpeed;

            relativeTime = time;

            float dayBegins = Mathf.PI / 2;
            float dayEnds = dayBegins + 2 * Mathf.PI * dayLength;

            float nightBegins = dayEnds + Mathf.PI;
            float nightEnds = nightBegins + 2 * Mathf.PI * nightLength;

            float boundTime = time % (2 * Mathf.PI * (dayLength + nightLength + 1));

            if (boundTime >= dayBegins && boundTime <= dayEnds)
            {
                relativeTime = 1;
                if (period == DayPeriod.DAWN)
                {
                    period = DayPeriod.DAY;
                    Pubsub.instance.Publish(EventName.PeriodUpdate, period);
                }
            }
            else if (boundTime >= nightBegins && boundTime <= nightEnds)
            {
                relativeTime = 0;
                if (period == DayPeriod.DUSK)
                {
                    period = DayPeriod.NIGHT;
                    Pubsub.instance.Publish(EventName.PeriodUpdate, period);
                }
            }
            else
            {
                relativeTime = Mathf.Sin(time) / 2 + 0.5f;
                if (period == DayPeriod.DAY)
                {
                    period = DayPeriod.DUSK;
                    Pubsub.instance.Publish(EventName.PeriodUpdate, period);
                }
                else if (period == DayPeriod.NIGHT)
                {
                    period = DayPeriod.DAWN;
                    Pubsub.instance.Publish(EventName.PeriodUpdate, period);
                }
            }

            skylight.intensity = Mathf.LerpAngle(nightIntensity, dayIntensity, relativeTime);
            skylight.color = Color.Lerp(nightTint, dayTint, relativeTime);
        }
    }
}

// void PlotFish(Color color, float x, float y)
// {
//     float debugX = (x % 10) - 5;
//     GameObject timeGo = new GameObject("time_plot");
//     var sp = timeGo.AddComponent<SpriteRenderer>();
//     sp.color = color;
//     sp.sprite = sprite;
//     sp.sortingLayerName = "UI";
//     timeGo.transform.position = new Vector2(debugX, 5 + y);
//     timeGo.transform.localScale = Vector2.one * 0.25f;
//     Destroy(timeGo, 1.5f);
// }
// print("Relative Time " + relativeTime);

// PlotFish(new Color(256, 256, 256, 0.25f), time, 1);
// PlotFish(new Color(256, 256, 256, 0.25f), time, 0);

// PlotFish(new Color(256, 0, 256, 0.25f), time, Mathf.Sin(4 * time) / 2 + 0.5f);
// PlotFish(Color.red, time, relativeTime);