
using System;
using System.Collections.Generic;

namespace Systems.Analytic
{
    [Serializable]
    public struct AnalyticEvent
    {
        public string type;
        public string data;
    }

    [Serializable]
    public class AnalyticsData
    {
        public List<AnalyticEvent> events;
        public AnalyticsData() => events = new List<AnalyticEvent>();

        public AnalyticEvent AddEvent(string type, string data)
        {
            AnalyticEvent analyticEvent = new AnalyticEvent() { type = type, data = data };
            events.Add(analyticEvent);

            return analyticEvent;
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}