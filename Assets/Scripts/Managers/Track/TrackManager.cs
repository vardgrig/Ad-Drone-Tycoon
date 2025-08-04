using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow;
using Systems;
using Systems.Pool;
using UnityEngine;
using Zenject;

namespace Managers.Track
{
    public class TrackManager : ITrackManager
    {
        public event Action OnTrackCompleted;

        private List<Checkpoint> _trackDefinition = new();
        private int _currentCheckpointIndex;
        
        private PoolManager _poolManager;
        
        [Inject]
        private void Construct(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }
        public void StartTrack(List<Checkpoint> generatedCheckpoints)
        {
            _trackDefinition = generatedCheckpoints;
            _currentCheckpointIndex = 0;

            foreach (var cp in _trackDefinition)
            {
                cp.Initialize(this);
            }

            UpdateCheckpointVisuals();
        }

        public void OnCheckpointPassed(Checkpoint checkpoint)
        {
            if (_trackDefinition.Count == 0 || checkpoint != _trackDefinition[_currentCheckpointIndex])
            {
                return;
            }

            _poolManager.Return(checkpoint);

            _currentCheckpointIndex++;

            if (_currentCheckpointIndex >= _trackDefinition.Count)
            {
                Debug.Log("FINISH! You completed the track!");
                OnTrackCompleted?.Invoke();
            }
            else
            {
                UpdateCheckpointVisuals();
            }
        }

        private void UpdateCheckpointVisuals()
        {
            if (_trackDefinition.Count == 0 || _currentCheckpointIndex >= _trackDefinition.Count)
            {
                return;
            }

            var finalCheckpoint = _trackDefinition[^1];
            if (!finalCheckpoint.gameObject.activeInHierarchy)
            {
                finalCheckpoint.gameObject.SetActive(true);
            }
            finalCheckpoint.SetState(Checkpoint.CheckpointState.Final);

            var nextCheckpoint = _trackDefinition[_currentCheckpointIndex];
            if (!nextCheckpoint.gameObject.activeInHierarchy)
            {
                nextCheckpoint.gameObject.SetActive(true);
            }
            nextCheckpoint.SetState(Checkpoint.CheckpointState.Next);

            var upcomingIndex = _currentCheckpointIndex + 1;
            if (upcomingIndex >= _trackDefinition.Count - 1) return;
            
            var upcomingCheckpoint = _trackDefinition[upcomingIndex];
            if (!upcomingCheckpoint.gameObject.activeInHierarchy)
            {
                upcomingCheckpoint.gameObject.SetActive(true);
            }
            upcomingCheckpoint.SetState(Checkpoint.CheckpointState.Upcoming);
        }

        public void ClearTrack()
        {
            foreach (var cp in _trackDefinition
                         .Where(cp => cp && 
                                      cp.gameObject.activeInHierarchy))
            {
                _poolManager.Return(cp);
            }

            _trackDefinition.Clear();
        }
    }
}