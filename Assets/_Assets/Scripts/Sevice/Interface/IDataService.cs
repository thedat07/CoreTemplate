using System;

public interface IDataService
{
    // --- Kiểm tra ---
    bool HasKey(string key);

    // --- Save ---
    void Save<T>(string key, T data);
    void Save<T>(string key, T data, Action onSuccess, Action<string> onError = null);

    // --- Load ---
    T Load<T>(string key);
    T Load<T>(string key, T defaultValue);
    void Load<T>(string key, Action<T> onLoaded, Action<string> onError = null);

    // --- Xoá ---
    void Delete(string key);
    void DeleteAll();

    // --- Utility ---
    void SaveAll();
    string GetRawJson(string key);
}