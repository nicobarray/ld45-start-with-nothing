using System;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public enum EventName
    {
        PeriodUpdate
    }

    public class Pubsub
    {
        public static Pubsub instance = new Pubsub();

        Dictionary<EventName, Action<object>> events = new Dictionary<EventName, Action<object>>();
        Dictionary<EventName, int> eventsCount = new Dictionary<EventName, int>();

        public void Publish(EventName eventName, object args)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName](args);
            }
        }

        public void On(EventName eventName, Action<object> callback)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName] += callback;
                eventsCount[eventName]++;
            }
            else
            {
                events.Add(eventName, callback);
                eventsCount.Add(eventName, 1);
            }
        }

        public void Off(EventName eventName, Action<object> callback)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName] -= callback;
                eventsCount[eventName]--;

                if (eventsCount[eventName] == 0)
                {
                    eventsCount.Remove(eventName);
                    events.Remove(eventName);
                }
            }
        }
    }
}