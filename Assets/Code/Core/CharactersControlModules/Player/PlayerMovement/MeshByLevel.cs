using System;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement
{
[Serializable]
public struct MeshByLevel
{
    public int Level { get; private set; }
    public Mesh Mesh { get; private set; }
}
}