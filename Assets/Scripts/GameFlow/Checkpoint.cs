using Managers;
using Managers.Track;
using UnityEngine;

namespace GameFlow
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private MeshRenderer meshRenderer;

        [Header("Colors")]
        [SerializeField] private Color nextColor = Color.green;
        [SerializeField] private Color upcomingColor = Color.yellow;
        [SerializeField] private Color finalColor = new(1f, 0.5f, 0f);

        public enum CheckpointState
        {
            Next,    
            Upcoming, 
            Final
        }

        private ITrackManager _trackManager;

        // This is called by the TrackManager to give the checkpoint a reference to it
        public void Initialize(ITrackManager manager)
        {
            _trackManager = manager;
        }

        public void SetState(CheckpointState state)
        {
            gameObject.SetActive(true);
            switch (state)
            {
                case CheckpointState.Next:
                    meshRenderer.material.color = nextColor;
                    break;
                case CheckpointState.Upcoming:
                    meshRenderer.material.color = upcomingColor;
                    break;
                case CheckpointState.Final:
                    meshRenderer.material.color = finalColor;
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _trackManager.OnCheckpointPassed(this);
            }
        }
    }
}