using System.Collections.Generic;
using Code.Core.LocalSaveSystem;
using Newtonsoft.Json.Linq;

namespace Code.Core.MoneyPack.Save
{
public class CashPackGlobalSave : ISavable
{
    private const string CashItemsCountSaveId = "CurrentCashItemsCountByHolderId";
    public string SaveId => "CashPack:GlobalSave";
    public Dictionary<string, CashPackSave> CurrentCashItemsCountByHolderId;

    public void InitializeAsNewSave()
    {
        CurrentCashItemsCountByHolderId = new Dictionary<string, CashPackSave>();
    }

    public void Parse(JObject jObject)
    {
        var currentCashJToken = jObject.GetValue(CashItemsCountSaveId);
        CurrentCashItemsCountByHolderId = currentCashJToken != null
            ? currentCashJToken.ToObject<Dictionary<string, CashPackSave>>()
            : new Dictionary<string, CashPackSave>();
    }

    public bool IsHaveSave()
    {
        return CurrentCashItemsCountByHolderId.Keys.Count != 0;
    }
}
}