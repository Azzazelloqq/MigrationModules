using System;
using UnityEngine;

namespace Code.Core.Objectives
{
[Serializable]
public struct ObjectivesItemData
{
    [field: SerializeField]
    public string StatId { get; private set; }
    [field: SerializeField]
    public string Description { get; private set; }
    [field: SerializeField]
    public int TargetValue { get; private set; }
}
}