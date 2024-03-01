using System.Threading;
using Code.Core.CharactersControlModules.BaseModule;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Code.Core.CharactersControlModules.CommonCharacterModules.CharacterNavigation
{
public interface ICharacterNavigationModule : ICharacterModule
{
    public UniTask InitializeAsync(CancellationToken token);
    public void ShowWorldNavigation(Transform target, string targetId);
    public void ShowWorldNavigation(Transform target, string targetId, Color followerColor);
    public void HideWorldNavigation(string targetId);
    public int CalculateDistance(Vector3 startPoint, Vector3 endPoint);
    public int CalculateDistance(Vector3 startPoint, Vector3 endPoint, out Vector3[] pathPoints);
    public void CalculatePath(Vector3 startPoint, Vector3 middlePoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints);
    public void CalculatePath(Vector3 startPoint, Vector3 endPoint, out NativeArray<Vector3> pathPoints);
    public Vector3 GetClosestNavigationPoint(Vector3 targetPoint);
    int GetDirectDistance(Vector3 startPoint, Vector3 endPoint);
}
}