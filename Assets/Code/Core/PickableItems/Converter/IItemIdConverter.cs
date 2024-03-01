using System;

namespace Code.Core.PickableItems.Converter
{
public interface IItemIdConverter : IDisposable
{
    public string ConvertItemIdToResourceId(string itemId);
}
}