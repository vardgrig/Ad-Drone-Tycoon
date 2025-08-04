using System;
using System.Collections.Generic;
using GameFlow;

namespace Managers.Track
{
    public interface ITrackManager
    {
        event Action OnTrackCompleted;
        void StartTrack(List<Checkpoint> generatedCheckpoints);
        void OnCheckpointPassed(Checkpoint checkpoint);
        void ClearTrack();
    }
}