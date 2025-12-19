using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    /// <summary>
    /// Script de debug pour visualiser et diagnostiquer les problèmes d'IA NavMesh
    /// </summary>
    public class AIDebugger : MonoBehaviour
    {
        [Header("Debug Options")]
        [Tooltip("Afficher le chemin de navigation")]
        public bool showPath = true;

        [Tooltip("Afficher les informations dans la console")]
        public bool logInfo = true;

        [Tooltip("Couleur du chemin")]
        public Color pathColor = Color.green;

        [Tooltip("Couleur si bloqué")]
        public Color blockedColor = Color.red;

        private NavMeshAgent agent;
        private AIEnemy aiEnemy;
        private float lastLogTime = 0f;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            aiEnemy = GetComponent<AIEnemy>();

            if (agent == null)
            {
                Debug.LogError($"{gameObject.name}: Pas de NavMeshAgent trouvé!");
            }

            if (logInfo)
            {
                LogNavMeshInfo();
            }
        }

        private void Update()
        {
            if (logInfo && Time.time - lastLogTime > 2f)
            {
                lastLogTime = Time.time;
                LogAgentStatus();
            }
        }

        private void OnDrawGizmos()
        {
            if (!showPath || agent == null) return;

            // Dessiner le chemin
            if (agent.hasPath)
            {
                Color color = agent.pathStatus == NavMeshPathStatus.PathComplete ? pathColor : blockedColor;
                Gizmos.color = color;

                Vector3[] corners = agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                    Gizmos.DrawSphere(corners[i], 0.2f);
                }

                // Dessiner la destination
                if (corners.Length > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(corners[corners.Length - 1], 0.3f);
                }
            }

            // Dessiner la position sur le NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, hit.position);
                Gizmos.DrawWireSphere(hit.position, 0.5f);
            }
            else
            {
                // Pas sur le NavMesh !
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1f);
            }

            // Dessiner la vitesse
            if (agent.velocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, agent.velocity);
            }
        }

        private void LogNavMeshInfo()
        {
            Debug.Log("=== DEBUG AI NAVMESH ===");
            
            // Vérifier si on est sur un NavMesh
            NavMeshHit hit;
            bool onNavMesh = NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas);
            
            if (onNavMesh)
            {
                Debug.Log($"✓ {gameObject.name} est sur le NavMesh");
                Debug.Log($"  Distance au NavMesh: {hit.distance:F2}m");
            }
            else
            {
                Debug.LogError($"✗ {gameObject.name} N'EST PAS sur le NavMesh!");
                Debug.LogError($"  → Créez un NavMesh: Window → AI → Navigation → Bake");
            }

            // Info NavMeshAgent
            if (agent != null)
            {
                Debug.Log($"NavMeshAgent configuré:");
                Debug.Log($"  - Speed: {agent.speed}");
                Debug.Log($"  - Angular Speed: {agent.angularSpeed}");
                Debug.Log($"  - Acceleration: {agent.acceleration}");
                Debug.Log($"  - Stopping Distance: {agent.stoppingDistance}");
                Debug.Log($"  - Is Stopped: {agent.isStopped}");
                Debug.Log($"  - Has Path: {agent.hasPath}");
            }

            Debug.Log("========================");
        }

        private void LogAgentStatus()
        {
            if (agent == null) return;

            string status = $"[AI] {gameObject.name}:\n";
            status += $"  Speed: {agent.speed:F1} m/s (Velocity: {agent.velocity.magnitude:F1})\n";
            status += $"  Has Path: {agent.hasPath}\n";
            status += $"  Path Status: {agent.pathStatus}\n";
            status += $"  Is Stopped: {agent.isStopped}\n";
            status += $"  Remaining Distance: {agent.remainingDistance:F1}m\n";

            if (aiEnemy != null)
            {
                status += $"  Is Chasing: {aiEnemy.IsChasing}\n";
                status += $"  Speed Progress: {aiEnemy.SpeedProgress * 100:F0}%";
            }

            Debug.Log(status);
        }

        [ContextMenu("Force Start Chasing")]
        public void ForceStartChasing()
        {
            if (aiEnemy != null)
            {
                Debug.Log("Force le démarrage de la poursuite...");
                // Utiliser la réflexion pour forcer le démarrage
                var method = typeof(AIEnemy).GetMethod("StartChasing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(aiEnemy, null);
                }
            }
        }
    }
}
