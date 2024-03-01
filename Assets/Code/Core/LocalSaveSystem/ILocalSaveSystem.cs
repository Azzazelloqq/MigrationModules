using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Core.LocalSaveSystem
{
public interface ILocalSaveSystem : IDisposable
{
    public void InitializeSaves(ISavable[] savables);
    public UniTask InitializeSavesAsync(ISavable[] savables, CancellationToken cancellationToken);
    public void StartAutoSave();
    public void StopAutoSave();
    public bool IsHaveSaveInt(string id);
    public bool IsHaveSaveFloat(string id);
    public bool IsHaveSaveString(string id);
    public void SaveInt(string id, int data);
    public void SaveFloat(string id, float data);
    public void SaveString(string id, string data);
    public void Save();
    public int LoadInt(string id);
    public float LoadFloat(string id);
    public string LoadString(string id);
    public T Load<T>() where T : ISavable;
    public void ForceUpdateStorageSaves();
    #if DEVELOPMENT_BUILD || UNITY_EDITOR
    void DeleteAllSavesDev();
    #endif
    public void DeleteAllSaves();
}
}