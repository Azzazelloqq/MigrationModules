using System;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.ArrowFollower.Tracker.BaseMVP
{
public interface IArrowUIFollowTrackerPresenter : IPresenter
{
    public event Action<bool> TargetOutOfScreenStateChanged;
    public bool TargetIsOutOfScreen { get; }
    public void Initialize();
    public void StartTrack(Transform target);
    public void StopTrack();
}
}