using System.Collections.Generic;
using UnityEngine;

/*
 * TriggerService — Hệ thống event giống Firebase Analytics.
 * - Log event với parameters
 * - Fire EventBus cho hệ thống khác lắng nghe
 * - Sau này tích hợp Firebase: chỉ cần thêm call FirebaseAnalytics.LogEvent() vào đây
 */

public class LogEventService : ILogEventService
{
    public void LogEvent(string eventName)
    {
        LogEvent(eventName, (IDictionary<string, object>)null);
    }

    public void LogEvent(string eventName, IDictionary<string, object> parameters)
    {
        // EventBus
        EventBus<AnalyticsEvent>.Raise(new AnalyticsEvent
        {
            EventName = eventName,
            Parameters = parameters
        });

        Debug.Log($"[TriggerService] {eventName}");
    }

    public void LogEvent(string eventName, params (string key, object value)[] parameters)
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < parameters.Length; i++)
            dict[parameters[i].key] = parameters[i].value;

        LogEvent(eventName, dict);
    }
}