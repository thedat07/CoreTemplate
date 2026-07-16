using System;
using UnityEngine;

/*
 * DataService — Lưu trữ dữ liệu qua Easy Save 3 (ES3) với bảo mật E3:
 *  - Encryption: ES3 AES-128 built-in
 *  - Encoding:   ES3 tự xử lý (nhị phân)
 *  - Error-handling: try-catch toàn bộ, log qua Debug + EventBus
 *
 * Yêu cầu: Easy Save 3 asset từ Asset Store.
 */

public class DataService : IDataService
{
    #region Settings

    private readonly ES3Settings settings;

    public DataService(string password = "Fram_Secure_K3y_2026!")
    {
        settings = new ES3Settings
        {
            encryptionType = ES3.EncryptionType.AES,
            encryptionPassword = password,
            location = ES3.Location.File,
            directory = ES3.Directory.PersistentDataPath,
            path = "FramData.es3"
        };
    }

    #endregion

    #region Public API

    public bool HasKey(string key)
    {
        return ES3.KeyExists(key, settings);
    }

    // ----- Save -----

    public void Save<T>(string key, T data)
    {
        ExecuteSafe("Save", key, () =>
        {
            ES3.Save(key, data, settings);

            EventBus<DataSavedEvent>.Raise(new DataSavedEvent
            {
                Key = key,
                DataType = typeof(T).Name
            });
        });
    }

    public void Save<T>(string key, T data, Action onSuccess, Action<string> onError = null)
    {
        try
        {
            Save(key, data);
            onSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }

    // ----- Load -----

    public T Load<T>(string key)
    {
        return Load(key, default(T));
    }

    public T Load<T>(string key, T defaultValue)
    {
        if (!ES3.KeyExists(key, settings))
            return defaultValue;

        T result = defaultValue;

        ExecuteSafe("Load", key, () =>
        {
            result = ES3.Load(key, defaultValue, settings);

            EventBus<DataLoadedEvent>.Raise(new DataLoadedEvent
            {
                Key = key,
                DataType = typeof(T).Name
            });
        });

        return result;
    }

    public void Load<T>(string key, Action<T> onLoaded, Action<string> onError = null)
    {
        try
        {
            T data = Load<T>(key);
            onLoaded?.Invoke(data);
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }

    // ----- Delete -----

    public void Delete(string key)
    {
        ExecuteSafe("Delete", key, () =>
        {
            ES3.DeleteKey(key, settings);

            EventBus<DataDeletedEvent>.Raise(new DataDeletedEvent
            {
                Key = key
            });
        });
    }

    public void DeleteAll()
    {
        ExecuteSafe("DeleteAll", "*", () =>
        {
            ES3.DeleteFile(settings);

            EventBus<DataDeletedEvent>.Raise(new DataDeletedEvent
            {
                Key = "*"
            });
        });
    }

    // ----- Utility -----

    public void SaveAll()
    {
        // ES3 auto-saves; explicit flush
        ES3.StoreCachedFile();
    }

    public string GetRawJson(string key)
    {
        string result = null;

        ExecuteSafe("GetRawJson", key, () =>
        {
            if (!ES3.KeyExists(key, settings))
                throw new DataNotFoundException($"Key '{key}' not found");

            // ES3 lưu nhị phân, nên ta load object rồi ToJson thủ công
            object obj = ES3.Load<object>(key, settings);
            result = JsonUtility.ToJson(obj, true);
        });

        return result;
    }

    #endregion

    #region Error Handling

    private void ExecuteSafe(string operation, string key, Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            string msg = $"[DataService] {operation} [{key}] failed: {ex.Message}";
            Debug.LogError(msg);

            EventBus<DataErrorEvent>.Raise(new DataErrorEvent
            {
                Key = key,
                Operation = operation,
                ErrorMessage = ex.Message
            });
        }
    }

    #endregion
}

/// <summary>Custom exception for missing data keys.</summary>
public class DataNotFoundException : Exception
{
    public DataNotFoundException(string msg) : base(msg) { }
}