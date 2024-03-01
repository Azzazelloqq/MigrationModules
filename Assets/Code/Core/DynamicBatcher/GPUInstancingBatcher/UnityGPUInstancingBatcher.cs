using System.Collections.Generic;
using Code.Core.DynamicBatcher.Base;
using Code.Core.Logger;
using Code.Core.TickHandler;
using UnityEngine;

namespace Code.Core.DynamicBatcher.GPUInstancingBatcher
{
public class UnityGPUInstancingBatcher : IDynamicBatcher
{
    private readonly BatchInstancesContainer[] _batchInstancesContainers;
    private readonly ITickHandler _tickHandler;
    private readonly IInGameLogger _logger;
    private readonly List<Matrix4x4> _matricesToBatch;

    public UnityGPUInstancingBatcher(
        BatchInstancesContainer[] batchInstancesContainers,
        ITickHandler tickHandler,
        IInGameLogger logger,
        int capacity = 500)
    {
        _batchInstancesContainers = batchInstancesContainers;
        _tickHandler = tickHandler;
        _logger = logger;
        
        _matricesToBatch = new List<Matrix4x4>(capacity);
    }

    public void Dispose()
    {
        _tickHandler.FrameLateUpdate -= OnFrameLateUpdate;
    }

    public void StartBatching()
    {
        _tickHandler.FrameLateUpdate += OnFrameLateUpdate;
    }

    public void Register(IBatchable batchable)
    {
        foreach (var batchInstancesContainer in _batchInstancesContainers)
        {
            if (batchInstancesContainer.InstanceId != batchable.InstanceToBatchId)
            {
                continue;
            }
            
            batchInstancesContainer.Batchables.Add(batchable);
            
            return;
        }
        
        _logger.LogError($"Can't find batchable by instance id {batchable.InstanceToBatchId}. Is incorrect id or maybe need inject instance batch data in constructor by {nameof(BatchInstancesContainer)}[]");
    }

    public void Unregister(IBatchable batchable)
    {
        if (_batchInstancesContainers.Length == 0)
        {
            return;
        }
        
        foreach (var batchInstancesContainer in _batchInstancesContainers)
        {
            if (batchInstancesContainer.InstanceId != batchable.InstanceToBatchId)
            {
                continue;
            }
            
            batchInstancesContainer.Batchables.Remove(batchable);
            
            return;
        }
    }

    private void OnFrameLateUpdate(float _)
    {
        if (_batchInstancesContainers.Length == 0)
        {
            return;
        }

        foreach (var batchInstancesContainer in _batchInstancesContainers)
        {
            var material = batchInstancesContainer.Material;
            var mesh = batchInstancesContainer.Mesh;
            var batchables = batchInstancesContainer.Batchables;
            
            foreach (var batchable in batchables)
            {
                if (!batchable.IsVisible)
                {
                    continue;
                }
                
                var localToWorldMatrix = batchable.GetLocalToWorldMatrix();
                _matricesToBatch.Add(localToWorldMatrix);
            }
            
            Graphics.DrawMeshInstanced(mesh, 0, material, _matricesToBatch);
            _matricesToBatch.Clear();
        }
    }
}
}