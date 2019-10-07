using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

namespace LDJAM45
{
    public class Firelight : MonoBehaviour
    {
        [Header("References")]
        public Light2D firelight;
        public Transform flame;

        public float defaultRadius = 0;

        float t = 0;
        float duration = 0.2f;

        float prevRadius = 0;
        float nextRadius = 0;

        bool disableLight = false;

        void OnEnable()
        {
            Pubsub.instance.On(EventName.PeriodUpdate, OnPeriodUpdate);
            DisableLight();
        }

        void OnDisable()
        {
            Pubsub.instance.Off(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnPeriodUpdate(object args)
        {
            DayPeriod period = (DayPeriod)args;

            if (period == DayPeriod.DAWN)
            {
                DisableLight();
            }
            else if (period == DayPeriod.DUSK)
            {
                EnableLight();
            }
        }

        void ComputeNextRadius()
        {
            prevRadius = firelight.pointLightOuterRadius;
            nextRadius = UnityEngine.Random.value * 2f + defaultRadius;
        }

        void Start()
        {
            ComputeNextRadius();
        }

        void DisableLight()
        {
            disableLight = true;
            firelight.pointLightOuterRadius = 0;
            flame.gameObject.SetActive(false);
        }

        void EnableLight()
        {
            disableLight = false;
            firelight.pointLightOuterRadius = defaultRadius;
            flame.gameObject.SetActive(true);
        }

        void Update()
        {
            if (disableLight)
            {
                return;
            }

            t += Time.deltaTime;
            if (t > duration)
            {
                t -= duration;
                ComputeNextRadius();
            }

            firelight.pointLightOuterRadius = Mathf.Lerp(prevRadius, nextRadius, t / duration);
        }
    }
}