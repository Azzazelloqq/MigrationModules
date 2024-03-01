using System.Collections.Generic;
using Code.Core.DynamicBatcher.Base;
using UnityEngine;

namespace Code.Core.DynamicBatcher.GPUInstancingBatcher
{
public struct BatchInstancesContainer
{
    public string InstanceId { get; }
    public Mesh Mesh { get; }
    public Material Material { get; }
    public List<IBatchable> Batchables { get; }
    
    public BatchInstancesContainer(string instanceId, Mesh mesh, Material material, int batchablesCapacity = 100)
    {
        InstanceId = instanceId;
        Mesh = mesh;
        Material = material;
        Batchables = new List<IBatchable>(batchablesCapacity);
    }
}
}