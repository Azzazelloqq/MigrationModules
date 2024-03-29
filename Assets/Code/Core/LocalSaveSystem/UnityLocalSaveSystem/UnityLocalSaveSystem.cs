using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Code.Core.Logger;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Code.Core.LocalSaveSystem.UnityLocalSaveSystem
{
public class UnityLocalSaveSystem : ILocalSaveSystem
{
    private const string FileName = "Saves.json";
    private ISavable[] _savesCash;
    private Dictionary<string, JObject> _loadedJsonSave;
    private bool _needSaveToStorage;
    private readonly string _storagePath;
    private readonly string _filePath;
    private readonly IInGameLogger _logger;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly int _autoSavePeriodPerSeconds;

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private static ISavable[] SavesCashDev;

    private static string SaveDirectoryPathDev =>
        Path.Combine(Application.persistentDataPath, "DeliveryBoy", "SaveData");
    private static string SaveFilePath => 
        Path.Combine(SaveDirectoryPathDev, FileName);
    #endif

    public UnityLocalSaveSystem(string storagePath, IInGameLogger logger, int autoSavePeriodPerSeconds = 3)
    {
        _storagePath = storagePath;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _filePath = Path.Combine(_storagePath, FileName);
        _autoSavePeriodPerSeconds = autoSavePeriodPerSeconds;
        SubscribeOnEvents();
    }

    public void InitializeSaves(ISavable[] savables)
    {
        _savesCash = savables;
        _loadedJsonSave = LoadJsonSave();
        ParseSavesFromStorage();
    }

    public async UniTask InitializeSavesAsync(ISavable[] savables, CancellationToken cancellationToken)
    {
        _savesCash = savables;
        _loadedJsonSave = await LoadJsonSave(cancellationToken);
        ParseSavesFromStorage();
    }
    
    public void Dispose()
    {
        CancelSave();
        _cancellationTokenSource?.Dispose();
    }

    public void StartAutoSave()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        StartAutoSaveData(_autoSavePeriodPerSeconds, _cancellationTokenSource.Token);
    }

    public void StopAutoSave()
    {
        CancelSave();
    }

    public bool IsHaveSaveInt(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        if (!PlayerPrefs.HasKey(id))
        {
            return false;
        }

        return true;
    }

    public bool IsHaveSaveFloat(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        if (!PlayerPrefs.HasKey(id))
        {
            return false;
        }

        return true;
    }

    public bool IsHaveSaveString(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        if (!PlayerPrefs.HasKey(id))
        {
            return false;
        }

        return true;
    }

    public void SaveInt(string id, int data)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        PlayerPrefs.SetInt(id, data);

        _needSaveToStorage = true;
    }

    public void SaveFloat(string id, float data)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        PlayerPrefs.SetFloat(id, data);

        _needSaveToStorage = true;
    }

    public void SaveString(string id, string data)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }


        PlayerPrefs.SetString(id, data);

        _needSaveToStorage = true;
    }

    public void Save()
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
            return;
        }

        _needSaveToStorage = true;
    }

    public int LoadInt(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        var data = PlayerPrefs.GetInt(id);
        return data;
    }

    public float LoadFloat(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }


        if (!PlayerPrefs.HasKey(id))
        {
            _logger.LogError($"Can't find data by id {id}");
        }

        var data = PlayerPrefs.GetFloat(id);
        return data;
    }

    public string LoadString(string id)
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
        }

        if (!PlayerPrefs.HasKey(id))
        {
            _logger.LogError($"Can't find data by id {id}");
        }

        var data = PlayerPrefs.GetString(id);
        return data;
    }

    public T Load<T>() where T : ISavable
    {
        if (_savesCash == null)
        {
            _logger.LogError("Initialize the saves before using");
            return default;
        }

        foreach (var savablesCash in _savesCash)
        {
            if (savablesCash is T savedData)
            {
                return savedData;
            }
        }

        _logger.LogError(
            $"Can't find {typeof(T)}. Need add savable in {nameof(ISavable)}[] from {nameof(InitializeSaves)} method");
        return default;
    }

    public void ForceUpdateStorageSaves()
    {
        CancelSave();
        
        SaveAll();
    }
    
    #if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void DeleteAllSavesDev()
    {
        UnsubscribeOnEvents();
        CancelSave();
        PlayerPrefs.DeleteAll();

        if (!Directory.Exists(SaveDirectoryPathDev))
        {
            return;
        }

        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
        
        Directory.Delete(SaveDirectoryPathDev);
    }
    
    #endif
    
    public void DeleteAllSaves()
    {
        PlayerPrefs.DeleteAll();

        if (!Directory.Exists(_storagePath))
        {
            return;
        }

        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
        
        Directory.Delete(_storagePath);

        foreach (var savesCash in _savesCash)
        {
            savesCash.InitializeAsNewSave();
        }

    }
    
    private void CancelSave()
    {
        _cancellationTokenSource.Cancel();
    }

    private void ParseSavesFromStorage()
    {
        if (_loadedJsonSave.Count == 0)
        {
            foreach (var savesCash in _savesCash)
            {
                savesCash.InitializeAsNewSave();
            }
        }

        foreach (var savesCash in _savesCash)
        {
            if (_loadedJsonSave.TryGetValue(savesCash.SaveId, out var saveJObject))
            {
                savesCash.Parse(saveJObject);
            }
            else
            {
                savesCash.InitializeAsNewSave();
            }
        }
    }

    private void SubscribeOnEvents()
    {
        Application.quitting += OnApplicationQuitting;
    }

    private void UnsubscribeOnEvents()
    {
        Application.quitting -= OnApplicationQuitting;
    }

    private void OnApplicationQuitting()
    {
        StopAutoSave();
        
        SaveAll();

        UnsubscribeOnEvents();
    }

    private async void StartAutoSaveData(int periodPerSeconds, CancellationToken token)
    {
        try
        {
            var periodPerMillisecond = periodPerSeconds * 1000;
            await UniTask.RunOnThreadPool(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    
                    await UniTask.Delay(periodPerMillisecond, cancellationToken: token);

                    if (!_needSaveToStorage)
                    {
                        continue;
                    }

                    await SaveAllAsync(token);

                    _needSaveToStorage = false;
                }
            }, cancellationToken: token);
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException)
            {
                _logger.LogException(e);
            }
        }
    }

    private void SaveAll()
    {
        CreateDirectoryIfNeed();
        var json = JsonConvert.SerializeObject(_savesCash);
        File.WriteAllText(_filePath, json);
    }

    private async UniTask SaveAllAsync(CancellationToken token)
    {
        try
        {
            CreateDirectoryIfNeed();

            var json = JsonConvert.SerializeObject(_savesCash);
            await File.WriteAllTextAsync(_filePath, json, token);
        }
        catch (Exception e)
        {
            _logger.LogException(e);
        }
        
        _logger.Log("[Save system] all data saved");
    }

    private Dictionary<string, JObject> LoadJsonSave()
    {
        if (!File.Exists(_filePath))
        {
            return new Dictionary<string, JObject>();
        }

        var json = File.ReadAllText(_filePath);

        return JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
    }

    private async UniTask<Dictionary<string, JObject>> LoadJsonSave(CancellationToken token)
    {
        try
        {
            var deserializeObject = new Dictionary<string, JObject>();

            if (!File.Exists(_filePath))
            {
                return deserializeObject;
            }

            var json = await File.ReadAllTextAsync(_filePath, token);

            var jsonArray = JArray.Parse(json);

            foreach (var jToken in jsonArray)
            {
                var item = (JObject)jToken;
                var saveId = item.GetValue("SaveId")!.ToString();
                deserializeObject[saveId] = item;
            }

            return deserializeObject;
        }
        catch (Exception e)
        {
            _logger.LogException(e);
            return new Dictionary<string, JObject>();
        }
    }

    private void CreateDirectoryIfNeed()
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    #if UNITY_EDITOR
    
    [MenuItem("SaveSystem/DeleteAllSaves")]
    private static void DeleteSavesDev()
    {
        PlayerPrefs.DeleteAll();

        if (!Directory.Exists(SaveDirectoryPathDev))
        {
            return;
        }

        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
        
        Directory.Delete(SaveDirectoryPathDev);
    }
    
    public static void SaveDev()
    {
        if (!Directory.Exists(SaveDirectoryPathDev))
        {
            Directory.CreateDirectory(SaveDirectoryPathDev);
        }
        
        if (!File.Exists(SaveFilePath))
        {
            using (var fileStream = File.Create(SaveFilePath))
            {
                
            }
        }
        
        var json = JsonConvert.SerializeObject(SavesCashDev);
        File.WriteAllText(SaveFilePath, json);
    }

    public static TSavable GetSave<TSavable>(ISavable[] allSavables) where TSavable : ISavable
    {
        SavesCashDev = allSavables;
        var deserializeObject = new Dictionary<string, JObject>();

        if(File.Exists(SaveFilePath))
        {
            var json = File.ReadAllText(SaveFilePath);

            var jsonArray = JArray.Parse(json);

            foreach (var jToken in jsonArray)
            {
                var item = (JObject)jToken;
                var saveId = item.GetValue("SaveId")!.ToString();
                deserializeObject[saveId] = item;
            }
        }

        if (deserializeObject.Count == 0)
        {
            foreach (var savesCash in SavesCashDev)
            {
                savesCash.InitializeAsNewSave();
            }
        }

        foreach (var savesCash in SavesCashDev)
        {
            if (deserializeObject.TryGetValue(savesCash.SaveId, out var saveJObject))
            {
                savesCash.Parse(saveJObject);
            }
            else
            {
                savesCash.Parse(new JObject());
            }
        }
        
        foreach (var savable in SavesCashDev)
        {
            if (savable is TSavable foundSave)
            {
                return foundSave;
            }
        }

        return default;
    }
    #endif
}
}