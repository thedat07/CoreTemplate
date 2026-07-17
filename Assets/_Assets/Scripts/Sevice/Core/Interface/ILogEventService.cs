using System.Collections.Generic;

public interface ILogEventService
{
    /// <summary>Log một event đơn giản (giống Firebase Analytics.LogEvent)</summary>
    void LogEvent(string eventName);

    /// <summary>Log event kèm parameters</summary>
    void LogEvent(string eventName, IDictionary<string, object> parameters);

    /// <summary>Log event kèm params dạng tuple (tiện cho code nhanh)</summary>
    void LogEvent(string eventName, params (string key, object value)[] parameters);
}