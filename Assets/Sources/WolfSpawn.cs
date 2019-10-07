using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class WolfSpawn : MonoBehaviour
    {
        public GameObject wolfPrefab;

        void OnEnable()
        {
            Pubsub.instance.On(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnDisable()
        {
            Pubsub.instance.Off(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnPeriodUpdate(object args)
        {
            DayPeriod period = (DayPeriod)args;

            if (period == DayPeriod.NIGHT)
            {
                Instantiate(wolfPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}