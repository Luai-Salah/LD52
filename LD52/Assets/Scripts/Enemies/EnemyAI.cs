using Pathfinding;
using UnityEngine;

namespace LD52.Enemies
{
    [RequireComponent(typeof(Seeker))]
    public class EnemyAI : MonoBehaviour
    {
        public bool FlipToTarget { get => m_FlipToTarget; set => m_FlipToTarget = value; }
        
        public bool ReachedEndOfPath { get; private set; }

        public Path Path { get; private set; }
     
        public int CurrentWaypointIndex { get; private set; }
        
        public bool FacingRight { get; private set; }
        
        public Vector3 CurrentWaypoint => Path.vectorPath[CurrentWaypointIndex];
        public Transform Target => m_Target;
        
        /// <summary>
        /// Whether should the path be updated or not.
        /// </summary>
        public bool IsUpdating { get; set; } = true;

        [Header("Basic")]
        [SerializeField] private Transform m_Target;

        [Space]
        [SerializeField] private float m_NextWaypointDistance = 3f;
        [SerializeField] private float m_NextUpdateTime = 0.5f;

        [Space]
        [SerializeField] private bool m_FlipToTarget = true;

        private Seeker m_Seeker;
        private float m_NextWpDistSqr;

        private void Awake()
        {
            m_Target = GameObject.FindGameObjectWithTag("Player").transform;
            m_Seeker = GetComponent<Seeker>();
        }

        /// <summary>
        /// Pathfinding initialization step, needs to happen either in awake or start.
        /// </summary>
        public void OnStart()
        {
            InvokeRepeating(nameof(UpdatePath), 0.0f, m_NextUpdateTime);
            m_NextWpDistSqr = m_NextWaypointDistance * m_NextWaypointDistance;
        }

        /// <summary>
        /// Pathfinding checks and values updates needs to happen before your update loop.
        /// </summary>
        /// <returns>False if it fails that means you need to break/return from your update loop</returns>
        public bool PreUpdate()
        {
            if (Path == null)
                return false;

            if (CurrentWaypointIndex >= Path.vectorPath.Count)
            {
                ReachedEndOfPath = true;
                return false;
            }
            
            ReachedEndOfPath = false;
            return true;
        }

        /// <summary>
        /// Pathfinding checks and value updates needs to happen after your update loop.
        /// </summary>
        public void PostUpdate()
        {
            float distanceSqr = Utility.DistanceSqr(transform.position, CurrentWaypoint);
            if (distanceSqr < m_NextWpDistSqr)
                CurrentWaypointIndex++;
        }

        private void Update()
        {
            if (!FlipToTarget)
                return;
            
            if ((FacingRight && transform.position.x > Target.position.x) ||
                (!FacingRight && transform.position.x < Target.position.x))
            {
                Flip();
            }
        }

        private void UpdatePath()
        {
            if (!m_Target)
                return;

            if (m_Seeker.IsDone() && IsUpdating)
                m_Seeker.StartPath(transform.position, m_Target.position, OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (p.error)
            {
                Debug.LogError(p.errorLog);
                return;
            }

            Path = p;
            CurrentWaypointIndex = 0;
        }
        
        private void Flip()
        {
            Vector3 angles = transform.eulerAngles;
            angles.y += 180f;
            transform.rotation = Quaternion.Euler(angles);

            FacingRight = !FacingRight;
        }
    }
}
