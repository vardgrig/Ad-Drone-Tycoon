using System.Collections.Generic;
using Extensions;
using GameFlow.Contracts;
using Managers;
using Managers.Track;
using Systems;
using Systems.Pool;
using UnityEngine;
using Zenject;

namespace GameFlow
{
    public class DronePathGenerator : MonoBehaviour
    {
        private IContractService _contractService;
        private ITrackManager _trackManager;
        private PoolManager _poolManager;

        [Header("Path Endpoints")]
        [Tooltip("The transform where the path must begin.")]
        public Transform startPoint;
        [Tooltip("The transform where the path must end.")]
        public Transform endPoint;

        [Header("Generation Settings")]
        [Tooltip("The total number of checkpoints to place along the path.")]
        public int numberOfCheckpoints = 20;

        [Header("Randomness & Shape Control")]
        [Tooltip("Base distance handles are pushed along the start/end direction to create a nice curve.")]
        public float handleTangentLength = 60f;
        [Tooltip("How much random deviation is applied to the handles. Higher values create more varied and wild paths.")]
        public Vector3 randomnessStrength = new(100f, 40f, 100f);

        [Header("Collision Avoidance")]
        [Tooltip("Set this to your environment/obstacle layer.")]
        public LayerMask obstacleLayer;
        [Tooltip("How much space to require around each validation point on the curve.")]
        public float clearanceRadius = 5f;
        [Tooltip("How many times to try generating a valid path before giving up.")]
        public int maxGenerationAttempts = 15;

        [Header("Prefabs")]
        [Tooltip("The Checkpoint prefab that will be spawned by the pool.")]
        public Checkpoint checkpointPrefab;
    
        private readonly List<Checkpoint> _trackCheckpoints = new();
        
        [Inject]
        public void Construct(
            IContractService contractService,
            ITrackManager trackManager,
            PoolManager poolManager)
        {
            _contractService = contractService;
            _trackManager = trackManager;
            _poolManager = poolManager;
            
            Debug.Log("DronePathGenerator has been constructed by Zenject.");
        }

        private void Start()
        {
            if (!startPoint || !endPoint)
            {
                Debug.LogError("StartPoint or EndPoint is not assigned in the DronePathGenerator!");
                return;
            }

            _poolManager.CreatePool(checkpointPrefab, numberOfCheckpoints);
        
            GenerateNewTrack();
        }

        private void GenerateNewTrack()
        {
            _trackManager.ClearTrack();
            _trackCheckpoints.Clear();

            var p0 = startPoint.position;
            var p3 = endPoint.position;
            Vector3 p1 = Vector3.zero, p2 = Vector3.zero;

            var isPathValid = false;
            for (var attempts = 0; attempts < maxGenerationAttempts && !isPathValid; attempts++)
            {
                var startTangent = startPoint.forward * handleTangentLength;
                var endTangent = -endPoint.forward * handleTangentLength;

                var randomOffset1 = GetRandomOffset();
                var randomOffset2 = GetRandomOffset();

                p1 = p0 + startTangent + randomOffset1;
                p2 = p3 + endTangent + randomOffset2;

                if (IsPathCollisionFree(p0, p1, p2, p3))
                {
                    isPathValid = true;
                    Debug.Log($"Successfully generated a valid path on attempt {attempts + 1}.");
                }
            }
        
            if (!isPathValid)
            {
                Debug.LogError("Failed to generate a valid, collision-free path after multiple attempts! Consider adjusting parameters or environment layout.");
                return;
            }

            for (var i = 0; i < numberOfCheckpoints; i++)
            {
                var t = (float)i / (numberOfCheckpoints - 1); 

                var position = Bezier.GetPoint(p0, p1, p2, p3, t);
                var direction = Bezier.GetFirstDerivative(p0, p1, p2, p3, t).normalized;

                var cp = _poolManager.Get<Checkpoint>();
                cp.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
                _trackCheckpoints.Add(cp);
            
                cp.gameObject.SetActive(false);
            }

            _trackManager.StartTrack(_trackCheckpoints);
        }

        private bool IsPathCollisionFree(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var validationPoints = 15;
            for (var i = 1; i < validationPoints; i++)
            {
                var t = (float)i / validationPoints;
                var pointOnCurve = Bezier.GetPoint(p0, p1, p2, p3, t);

                if (Physics.CheckSphere(pointOnCurve, clearanceRadius, obstacleLayer))
                {
                    return false;
                }
            }
            return true; 
        }

        private Vector3 GetRandomOffset()
        {
            var contractData = _contractService.CurrentSession.ContractData;
            var difficultyMultiplier = 1.0f + (contractData.routeDifficulty / 20.0f); 
            var finalStrength = randomnessStrength * difficultyMultiplier;
        
            return new Vector3(
                Random.Range(-finalStrength.x, finalStrength.x),
                Random.Range(-finalStrength.y, finalStrength.y),
                Random.Range(-finalStrength.z, finalStrength.z)
            );
        }
    
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (startPoint == null || endPoint == null) return;

            Random.InitState(0);
        
            var p0 = startPoint.position;
            var p3 = endPoint.position;
            var startTangent = startPoint.forward * handleTangentLength;
            var endTangent = -endPoint.forward * handleTangentLength;

            var randomOffset1 = new Vector3(
                Random.Range(-randomnessStrength.x, randomnessStrength.x), 
                Random.Range(-randomnessStrength.y, randomnessStrength.y), 
                Random.Range(-randomnessStrength.z, randomnessStrength.z));
            
            var randomOffset2 = new Vector3(
                Random.Range(-randomnessStrength.x, randomnessStrength.x), 
                Random.Range(-randomnessStrength.y, randomnessStrength.y), 
                Random.Range(-randomnessStrength.z, randomnessStrength.z));

            var p1 = p0 + startTangent + randomOffset1;
            var p2 = p3 + endTangent + randomOffset2;
        
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p3, p2);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(p1, 1f);
            Gizmos.DrawSphere(p2, 1f);
        
            Gizmos.color = IsPathCollisionFree(p0, p1, p2, p3) ? Color.white : Color.red;
            var lastPoint = p0;
            for (var i = 1; i <= 30; i++)
            {
                var t = (float)i / 30f;
                var newPoint = Bezier.GetPoint(p0, p1, p2, p3, t);
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }
#endif
    }
}