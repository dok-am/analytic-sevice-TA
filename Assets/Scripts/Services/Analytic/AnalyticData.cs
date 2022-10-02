
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Analytic
{
    [Serializable]
    public struct AnalyticsEvent
    {
        public string type;
        public string data;
    }

    [Serializable]
    public class AnalyticsData
    {
        public List<AnalyticsEvent> events;
        public AnalyticsData() => events = new List<AnalyticsEvent>();

        public AnalyticsEvent AddEvent(string type, string data)
        {
            AnalyticsEvent analyticsEvent = new AnalyticsEvent() { type = type, data = data };
            events.Add(analyticsEvent);

            return analyticsEvent;
        }

        public void Clear()
        {
            events.Clear();
        }

        public void AddEventsFromString(string eventsString)
        {
            AnalyticsData otherData = JsonUtility.FromJson<AnalyticsData>(eventsString);

            if (otherData != null)
            {
                events.AddRange(otherData.events);
            }
        }
    }
}