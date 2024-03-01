using UnityEngine;

namespace Code.Core.DynamicBatcher.Base
{
public interface IBatchable
{
    public string InstanceToBatchId { get; }
    public bool IsVisible { get; }
    public Matrix4x4 GetLocalToWorldMatrix();
}
}